import { Component, Inject, Input, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { PartnerOldNewReportDetail } from '../partner-old-new-report.service';

@Component({
  selector: 'app-sale-report-partner-detail',
  templateUrl: './sale-report-partner-detail.component.html',
  styleUrls: ['./sale-report-partner-detail.component.css']
})
export class SaleReportPartnerDetailComponent implements OnInit {
  @Input() public details: PartnerOldNewReportDetail[];
  skip = 0;
  limit = 20;
  pagerSettings: any;
  gridData: GridDataResult;
  loading = false;
  constructor(
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.loadItems();
  }

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadItems();
  }

  computeType(type) {
    switch (type) {
      case "KHM":
        return "Khách mới"
      case "KHC":
        return "Khách cũ"
      default:
        break;
    }
  }

  loadItems(): void {
    this.gridData = {
      data: this.details.slice(this.skip, this.skip + this.limit),
      total: this.details.length
    };
  }

}
