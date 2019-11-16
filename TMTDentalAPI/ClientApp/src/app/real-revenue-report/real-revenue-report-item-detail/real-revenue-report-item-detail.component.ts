import { Component, OnInit, Input } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { aggregateBy } from '@progress/kendo-data-query';
import { RealRevenueReportItem, RealRevenueReportItemDetail, RealRevenueReportService } from '../real-revenue-report.service';

@Component({
  selector: 'app-real-revenue-report-item-detail',
  templateUrl: './real-revenue-report-item-detail.component.html',
  styleUrls: ['./real-revenue-report-item-detail.component.css']
})
export class RealRevenueReportItemDetailComponent implements OnInit {
  @Input() public item: RealRevenueReportItem;
  skip = 0;
  limit = 10;
  gridData: GridDataResult;
  details: RealRevenueReportItemDetail[];
  loading = false;

  public total: any;
  public aggregates: any[] = [
    { field: 'debit', aggregate: 'sum' },
    { field: 'credit', aggregate: 'sum' },
    { field: 'balance', aggregate: 'sum' }
  ];

  constructor(private realRevenueReportService: RealRevenueReportService) { }

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;

    this.realRevenueReportService.getReportDetail(this.item).subscribe(res => {
      this.details = res;
      this.total = aggregateBy(this.details, this.aggregates);
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

