import { Component, OnInit, Input } from '@angular/core';
import { AccountCommonPartnerReportService, AccountCommonPartnerReportItemDetail, AccountCommonPartnerReportItem } from '../account-common-partner-report.service';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';

@Component({
  selector: 'app-account-common-partner-report-detail',
  templateUrl: './account-common-partner-report-detail.component.html',
  styleUrls: ['./account-common-partner-report-detail.component.css']
})
export class AccountCommonPartnerReportDetailComponent implements OnInit {
  @Input() public item: AccountCommonPartnerReportItem;
  skip = 0;
  limit = 10;
  gridData: GridDataResult;
  details: AccountCommonPartnerReportItemDetail[];
  loading = false;

  constructor(private reportService: AccountCommonPartnerReportService) { }

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;

    this.reportService.getDetail(this.item).subscribe(res => {
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
