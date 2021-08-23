import { Component, Input, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { PartnerOldNewReportDetail } from '../partner-old-new-report.service';
import { SaleReportPartnerItemV3Detail } from '../sale-report.service';

@Component({
  selector: 'app-sale-report-partner-detail',
  templateUrl: './sale-report-partner-detail.component.html',
  styleUrls: ['./sale-report-partner-detail.component.css']
})
export class SaleReportPartnerDetailComponent implements OnInit {
  @Input() public details: PartnerOldNewReportDetail[];
  skip = 0;
  limit = 10;
  pageSizes = [20, 50, 100, 200];
  gridData: GridDataResult;
  loading = false;
  constructor() { }

  ngOnInit() {
    this.loadItems();
  }

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadItems();
  }

  onPageSizeChange(value: number): void {
    this.skip = 0;
    this.limit = value;
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
