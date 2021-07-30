import { Component, OnInit, ViewChild } from '@angular/core';
import { LegendLabelsContentArgs } from '@progress/kendo-angular-charts';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { forkJoin, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, switchMap, tap } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/auth.service';
import { SmsAccountPaged, SmsAccountService } from '../sms-account.service';
import { SmsCampaignPaged, SmsCampaignService } from '../sms-campaign.service';
import { ReportCampaignPaged, ReportSupplierInput, ReportTotalInput, SmsMessageDetailService } from '../sms-message-detail.service';

@Component({
  selector: 'app-sms-report',
  templateUrl: './sms-report.component.html',
  styleUrls: ['./sms-report.component.css'],
  host: {
    class: "o_action o_view_controller",
  },
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
  reportChartLinesAccounts: any[] = [];
  pieData_reportTotal: any[] = [];
  total_reportTotal;
  success_reportTotal;
  error_reportTotal;
  cancel_reportTotal;
  outgoing_reportTotal;
  lineData_reportSupplierSuccess: any[] = [];
  lineData_reportSupplierFails: any[] = [];
  loadingReportCampaign = false;
  limitReportCampaign = 20;
  offsetReportCampaign = 0;
  searchReportCampaign;
  searchUpdateReportCampaign = new Subject<string>();
  dateFromReportCampaign: Date;
  dateToReportCampaign: Date;
  accountProvider: any;
  gridDataReportCampaign: GridDataResult;

  constructor(
    private intlService: IntlService,
    private smsAccountService: SmsAccountService,
    private smsCampaignService: SmsCampaignService,
    private smsMessageDetailService: SmsMessageDetailService,
    private authService: AuthService
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
        }
      }
    )
  }

  loadSupplier() {
    this.searchSupplier().subscribe(
      (result: any) => {
        this.filteredSmsSupplier = result;
        if (result && result[0]) {
          this.accountProvider = result[0].provider;
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
    val.companyId = this.authService.userInfo.companyId;
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
        this.pieData_reportTotal = this.pieData_reportTotal.map(x => (
          {
            ...x,
            color: (
              x.state == "sent" ? "#007bff" : (x.state == "canceled" ? "#ff0000" : (x.state == "error" ? "#ffab00" : "#020202"))
            )
          })
        )

        const success = this.pieData_reportTotal.find(x => x.state == "sent");
        const error = this.pieData_reportTotal.find(x => x.state == "error");
        const canceled = this.pieData_reportTotal.find(x => x.state == "canceled");
        const outgoing = this.pieData_reportTotal.find(x => x.state == "outgoing");
        this.success_reportTotal = success ? success.total : 0;
        this.error_reportTotal = error ? error.total : 0;
        this.cancel_reportTotal = canceled ? canceled.total : 0;
        this.outgoing_reportTotal = outgoing ? outgoing.total : 0;
        this.total_reportTotal = this.success_reportTotal + this.error_reportTotal + this.cancel_reportTotal + this.outgoing_reportTotal;
        console.log(this.pieData_reportTotal);

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
    var requestSent = this.smsMessageDetailService.getReportSupplierSumaryChart({ provider: this.accountProvider || null, state: 'sent' });
    var requestError = this.smsMessageDetailService.getReportSupplierSumaryChart({ provider: this.accountProvider || null, state: 'error' });
    var requestCancel = this.smsMessageDetailService.getReportSupplierSumaryChart({ provider: this.accountProvider || null, state: 'canceled' });
    var requestOutgoing = this.smsMessageDetailService.getReportSupplierSumaryChart({ provider: this.accountProvider || null, state: 'outgoing' });
    forkJoin(requestSent, requestError, requestCancel, requestOutgoing).subscribe((results: any[]) => {
      this.reportChartLinesAccounts = results ? results : [];
      console.log(this.reportChartLinesAccounts);
      
    })
  }

  onChangeSupplier(event) {
    this.accountProvider = event ? event.currentTarget.value : null;
    this.getReportSumarySupplier();
  }
}
