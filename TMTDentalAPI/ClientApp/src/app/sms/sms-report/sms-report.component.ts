import { Component, OnInit, ViewChild } from '@angular/core';
import { LegendLabelsContentArgs } from '@progress/kendo-angular-charts';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { forkJoin, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, switchMap, tap } from 'rxjs/operators';
import { SmsAccountPaged, SmsAccountService } from '../sms-account.service';
import { SmsCampaignPaged, SmsCampaignService } from '../sms-campaign.service';
import { ReportCampaignPaged, ReportSupplierInput, ReportTotalInput, SmsMessageDetailService } from '../sms-message-detail.service';

@Component({
  selector: 'app-sms-report',
  templateUrl: './sms-report.component.html',
  styleUrls: ['./sms-report.component.css']
})
export class SmsReportComponent implements OnInit {

  @ViewChild("smsBrandnameCbx", { static: true }) smsBrandnameCbx: ComboBoxComponent;
  @ViewChild("smsCampaignCbx", { static: true }) smsCampaignCbx: ComboBoxComponent;
  @ViewChild("smsSupplierCbx", { static: true }) smsSupplierCbx: ComboBoxComponent;

  date_reportTotal: Date = new Date();
  filteredSmsBrandname: any[] = [];
  filteredSmsCampaign: any[] = [];
  filteredSmsSupplier: any[] = [];
  smsBrandname_reportTotal;
  smsCampaign_reportTotal;
  pieData_reportTotal: any[] = [];
  total_reportTotal;
  success_reportTotal;
  fails_reportTotal;
  lineData_reportSupplierSuccess: any[] = [];
  lineData_reportSupplierFails: any[] = [];
  loadingReportCampaign = false;
  limitReportCampaign = 20;
  offsetReportCampaign = 0;
  searchReportCampaign;
  searchUpdateReportCampaign = new Subject<string>();
  dateFromReportCampaign: Date;
  dateToReportCampaign: Date;
  accountId: any;
  gridDataReportCampaign: GridDataResult;

  constructor(
    private intlService: IntlService,
    private smsAccountService: SmsAccountService,
    private smsCampaignService: SmsCampaignService,
    private smsMessageDetailService: SmsMessageDetailService
  ) { }

  ngOnInit() {
    this.loadBrandname();
    this.loadCampaign();
    this.loadSupplier();
    this.getReportTotal();
    this.getReportCampaign();
    // this.getReportSupplier();

    this.smsBrandnameCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.smsBrandnameCbx.loading = true)),
      switchMap(value => this.searchBrandname(value))
    ).subscribe((result: any) => {
      this.filteredSmsBrandname = result.items;
      this.smsBrandnameCbx.loading = false;
    });

    this.smsCampaignCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.smsCampaignCbx.loading = true)),
      switchMap(value => this.searchCampaign(value))
    ).subscribe((result: any) => {
      this.filteredSmsCampaign = result.items;
      this.smsCampaignCbx.loading = false;
    });

    this.searchUpdateReportCampaign.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((value) => {
        this.offsetReportCampaign = 0;
        this.getReportCampaign();
      });

  }

  loadCampaign() {
    this.searchCampaign().subscribe(
      (result: any) => {
        if (result && result.items) {
          this.filteredSmsCampaign = result.items;
        }
      }
    )
  }

  loadBrandname() {
    this.searchBrandname().subscribe(
      (result: any) => {
        if (result && result.items) {
          this.filteredSmsBrandname = result.items
          console.log(this.filteredSmsBrandname);

        }
      }
    )
  }

  loadSupplier() {
    this.searchSupplier().subscribe(
      (result: any) => {
        this.filteredSmsSupplier = result;
        if (result && result[0]) {
          this.accountId = result[0].id;
          this.getReportSumarySupplier();
        }
      }
    )
  }

  searchCampaign(search?: string) {
    var val = new SmsCampaignPaged();
    val.limit = 20;
    val.offset = 0;
    val.search = search || '';
    val.combobox = false;
    return this.smsCampaignService.getPaged(val);
  }

  searchBrandname(search?: string) {
    var val = new SmsAccountPaged();
    val.limit = 20;
    val.offset = 0;
    val.search = search || '';
    return this.smsAccountService.getPaged(val);
  }

  searchSupplier(search?: string) {
    return this.smsAccountService.smsSupplierAutocomplete(search || '');
  }

  getReportTotal() {
    var reportTotalInput = new ReportTotalInput();
    reportTotalInput.date = this.intlService.formatDate(this.date_reportTotal, 'd', 'en-US');
    reportTotalInput.smsAccountId = this.smsBrandname_reportTotal ? this.smsBrandname_reportTotal.id : '';
    reportTotalInput.smsCampaignId = this.smsCampaign_reportTotal ? this.smsCampaign_reportTotal.id : '';
    this.smsMessageDetailService.getReportTotal(reportTotalInput).subscribe(
      (result: any) => {
        this.pieData_reportTotal = result;
        this.pieData_reportTotal = this.pieData_reportTotal.map(x => ({ ...x, color: (x.state == "success" ? "DeepSkyBlue" : "DarkSlateGrey") }))
        const success = this.pieData_reportTotal.find(x => x.state == "success");
        const fails = this.pieData_reportTotal.find(x => x.state == "fails");
        this.success_reportTotal = success ? success.total : 0;
        this.fails_reportTotal = fails ? fails.total : 0;
        this.total_reportTotal = this.success_reportTotal + this.fails_reportTotal;
      }, (error) => {
        console.log(error);

      }
    )
  }

  labelContentReportTotal(args: LegendLabelsContentArgs): string {
    return `${args.dataItem.stateDisplay} : ${args.dataItem.total}`;
  }

  pageChangeReportTotal(event) {
    this.offsetReportCampaign = event.skip;
    this.getReportCampaign();
  }

  dateChangeReportCampaign(event) {
    this.dateFromReportCampaign = event.dateFrom;
    this.dateToReportCampaign = event.dateTo;
    this.offsetReportCampaign = 0;
    this.getReportCampaign();
  }

  getReportCampaign() {
    this.loadingReportCampaign = true;
    var reportCampaignPaged = new ReportCampaignPaged();
    reportCampaignPaged.limit = this.limitReportCampaign;
    reportCampaignPaged.offset = this.offsetReportCampaign;
    reportCampaignPaged.search = this.searchReportCampaign || "";
    reportCampaignPaged.dateFrom = this.dateFromReportCampaign ? this.intlService.formatDate(this.dateFromReportCampaign, "yyyy-MM-dd") : "";
    reportCampaignPaged.dateTo = this.dateToReportCampaign ? this.intlService.formatDate(this.dateToReportCampaign, "yyyy-MM-ddT23:59") : "";
    this.smsMessageDetailService.getReportCampaign(reportCampaignPaged).pipe(
      map(
        (response: any) =>
          <GridDataResult>{
            data: response.items,
            total: response.totalItems,
          }
      )
    ).subscribe(
      (result: any) => {
        this.gridDataReportCampaign = result;
        this.loadingReportCampaign = false;
      }, (error) => {
        console.log(error);
        this.loadingReportCampaign = false;
      }
    )
  }

  getReportSumarySupplier() {
    var requestSuccess = this.smsMessageDetailService.getReportSupplierSumaryChart({ accountId: this.accountId || null, state: "success" });
    var requestFails = this.smsMessageDetailService.getReportSupplierSumaryChart({ accountId: this.accountId || null, state: "fails" });
    forkJoin(requestSuccess, requestFails).subscribe((result: any) => {
      this.lineData_reportSupplierSuccess = result[0] || [];
      this.lineData_reportSupplierFails = result[1] || [];
    })
  }

  onChangeSupplier(event) {
    this.accountId = event ? event.currentTarget.value : null;
    this.getReportSumarySupplier();
  }
}
