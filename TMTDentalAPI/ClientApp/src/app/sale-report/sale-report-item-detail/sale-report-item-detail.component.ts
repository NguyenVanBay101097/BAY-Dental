import { Component, OnInit, Input, Inject } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { SaleReportItem, SaleReportItemDetail, SaleReportService } from '../sale-report.service';
import { aggregateBy } from '@progress/kendo-data-query';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';

@Component({
  selector: 'app-sale-report-item-detail',
  templateUrl: './sale-report-item-detail.component.html',
  styleUrls: ['./sale-report-item-detail.component.css']
})
export class SaleReportItemDetailComponent implements OnInit {
  @Input() public item: SaleReportItem;
  skip = 0;
  limit = 20;
  pagerSettings: any;
  gridData: GridDataResult;
  details: SaleReportItemDetail[];
  loading = false;

  public total: any;
  public aggregates: any[] = [
    { field: 'productUOMQty', aggregate: 'sum' },
    { field: 'priceTotal', aggregate: 'sum' },
  ];

  constructor(private saleReportService: SaleReportService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;

    this.saleReportService.getReportDetail(this.item).subscribe(res => {
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
    this.limit = event.take;
    this.loadItems();
  }

  loadItems(): void {
    this.gridData = {
      data: this.details.slice(this.skip, this.skip + this.limit),
      total: this.details.length
    };
  }
}

