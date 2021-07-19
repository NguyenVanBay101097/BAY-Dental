import { Component, Input, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { AccountCommonPartnerReportService, ReportPartnerDebitDetailReq, ReportPartnerDebitDetailRes, ReportPartnerDebitRes } from '../account-common-partner-report.service';

@Component({
  selector: 'app-partner-debit-detail-list-report',
  templateUrl: './partner-debit-detail-list-report.component.html',
  styleUrls: ['./partner-debit-detail-list-report.component.css']
})
export class PartnerDebitDetailListReportComponent implements OnInit {

  @Input() public parent: ReportPartnerDebitRes;
  skip = 0;
  limit = 10;
  gridData: GridDataResult;
  details: ReportPartnerDebitDetailRes[];
  loading = false;

  constructor(private reportService: AccountCommonPartnerReportService,
    private intlService: IntlService
    ) { }

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new ReportPartnerDebitDetailReq();
    val.companyId = this.parent.companyId || '';
    val.fromDate = this.parent.dateFrom ? this.intlService.formatDate(new Date(this.parent.dateFrom), 'yyyy-MM-dd') : null;
    val.toDate = this.parent.dateTo ? this.intlService.formatDate(new Date(this.parent.dateTo), 'yyyy-MM-dd') : null;
    val.partnerId = this.parent.partnerId || '';

    this.reportService.ReportPartnerDebitDetail(val).subscribe(res => {
      this.details = res;
      this.loadItems();
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadItems();
  }

  loadItems(): void {
    this.gridData = {
      data: this.details.slice(this.skip, this.skip + this.limit),
      total: this.details.length
    };
  }

}
