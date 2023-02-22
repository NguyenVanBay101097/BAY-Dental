import { Component, OnInit, Input, Inject } from '@angular/core';
import { AccountCommonPartnerReportService, AccountCommonPartnerReportItemDetail, AccountCommonPartnerReportItem } from '../account-common-partner-report.service';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';

@Component({
  selector: 'app-account-common-partner-report-detail',
  templateUrl: './account-common-partner-report-detail.component.html',
  styleUrls: ['./account-common-partner-report-detail.component.css']
})
export class AccountCommonPartnerReportDetailComponent implements OnInit {
  @Input() public item: AccountCommonPartnerReportItem;
  skip = 0;
  limit = 20;
  pagerSettings: any;
  gridData: GridDataResult;
  details: AccountCommonPartnerReportItemDetail[];
  loading = false;

  constructor(
    private reportService: AccountCommonPartnerReportService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

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
