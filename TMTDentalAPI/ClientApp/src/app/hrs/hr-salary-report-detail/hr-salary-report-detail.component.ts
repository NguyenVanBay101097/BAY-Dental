import { Component, Input, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { AccountCommonPartnerReportItem, AccountCommonPartnerReportItemDetail, AccountCommonPartnerReportService } from 'src/app/account-common-partner-reports/account-common-partner-report.service';

@Component({
  selector: 'app-hr-salary-report-detail',
  templateUrl: './hr-salary-report-detail.component.html',
  styleUrls: ['./hr-salary-report-detail.component.css']
})
export class HrSalaryReportDetailComponent implements OnInit {

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
    this.reportService.getReportSalaryEmployeeDetail(this.item).subscribe(res => {
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
