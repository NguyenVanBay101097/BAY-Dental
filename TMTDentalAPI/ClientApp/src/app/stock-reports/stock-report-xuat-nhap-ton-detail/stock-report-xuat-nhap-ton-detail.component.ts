import { Component, OnInit, Input, Inject } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { StockReportXuatNhapTonItem, StockReportXuatNhapTonItemDetail, StockReportService } from '../stock-report.service';


@Component({
  selector: 'app-stock-report-xuat-nhap-ton-detail',
  templateUrl: './stock-report-xuat-nhap-ton-detail.component.html',
  styleUrls: ['./stock-report-xuat-nhap-ton-detail.component.css']
})

export class StockReportXuatNhapTonDetailComponent implements OnInit {
  @Input() public item: StockReportXuatNhapTonItem;
  skip = 0;
  limit = 20;
  pagerSettings: any;
  gridData: GridDataResult;
  details: StockReportXuatNhapTonItemDetail[];
  loading = false;

  constructor(private reportService: StockReportService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;

    this.reportService.getXuatNhapTonDetail(this.item).subscribe(res => {
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
