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
