import { CommissionReport, CommissionReportDetail, CommissionReportsService, ReportFilterCommissionDetail } from './../commission-reports.service';
import { Component, OnInit, Input } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { aggregateBy } from '@progress/kendo-data-query';
import { IntlService } from '@progress/kendo-angular-intl';

@Component({
  selector: 'app-commission-report-detail',
  templateUrl: './commission-report-detail.component.html',
  styleUrls: ['./commission-report-detail.component.css']
})
export class CommissionReportDetailComponent implements OnInit {
  @Input() public item: CommissionReport;
  skip = 0;
  limit = 10;
  gridData: GridDataResult;
  details: CommissionReportDetail[];
  loading = false;
  @Input() public dateFrom: Date;
  @Input() public dateTo: Date;

  public total: any;
  constructor(private commissionReportService: CommissionReportsService , private intl: IntlService) { }

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    var val = new ReportFilterCommissionDetail();
    val.dateFrom = this.dateFrom ? this.intl.formatDate(this.dateFrom, 'yyyy-MM-ddTHH:mm:ss') : null;
    val.dateTo = this.dateTo ? this.intl.formatDate(this.dateTo, 'yyyy-MM-ddTHH:mm:ss') : null;
    
    if(this.item.userId){
      val.userId = this.item.userId;
    }

    this.loading = true;

    this.commissionReportService.getReportDetail(val).subscribe(res => {
      this.details = res;
      this.total = aggregateBy(this.details);
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
