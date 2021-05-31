import { Component, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { SmsAccountPaged, SmsAccountService } from '../sms-account.service';
import { SmsCampaignPaged, SmsCampaignService } from '../sms-campaign.service';
import { ReportTotalInput, SmsMessageDetailService } from '../sms-message-detail.service';

@Component({
  selector: 'app-sms-report',
  templateUrl: './sms-report.component.html',
  styleUrls: ['./sms-report.component.css']
})
export class SmsReportComponent implements OnInit {

  @ViewChild("smsBrandnameCbx", { static: true }) smsBrandnameCbx: ComboBoxComponent;
  @ViewChild("smsCampaignCbx", { static: true }) smsCampaignCbx: ComboBoxComponent;

  date_reportTotal: Date = new Date();
  filteredSmsBrandname: any[];
  filteredSmsCampaign: any[];
  smsBrandname_reportTotal;
  smsCampaign_reportTotal;
  pieData_reportTotal: any[];

  constructor(
    private intlService: IntlService, 
    private smsAccountService: SmsAccountService, 
    private smsCampaignService: SmsCampaignService, 
    private smsMessageDetailService: SmsMessageDetailService
  ) { }

  ngOnInit() {
    this.loadBrandname();
    this.loadCampaign();

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
  }

  loadCampaign() {
    this.searchCampaign().subscribe(
      (result: any) => {
        if (result && result.items) {
          this.filteredSmsCampaign = result.items
        }
      }
    )
  }

  searchCampaign(search?: string) {
    var val = new SmsCampaignPaged();
    val.limit = 20;
    val.offset = 0;
    val.search = search || '';
    val.combobox = true;
    return this.smsCampaignService.getPaged(val);
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

  searchBrandname(search?: string) {
    var val = new SmsAccountPaged();
    val.limit = 20;
    val.offset = 0;
    val.search = search || '';
    return this.smsAccountService.getPaged(val);
  }

  getReportTotal() {
    var reportTotalInput = new ReportTotalInput();
    reportTotalInput.date = this.intlService.formatDate(this.date_reportTotal, 'd', 'en-US');
    reportTotalInput.smsAccountId = this.smsBrandname_reportTotal ? this.smsBrandname_reportTotal.id : '';
    reportTotalInput.smsCampaignId = this.smsCampaign_reportTotal ? this.smsCampaign_reportTotal.id : '';
    this.smsMessageDetailService.getReportTotal(ReportTotalInput).subscribe(
      (result: any) => {
        console.log(result);
        
      }
    )
  }
}
