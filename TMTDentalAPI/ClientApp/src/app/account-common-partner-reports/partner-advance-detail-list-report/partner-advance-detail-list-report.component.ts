import { Component, Inject, Input, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { AccountCommonPartnerReportService, ReportPartnerAdvanceDetail, ReportPartnerAdvanceDetailFilter } from '../account-common-partner-report.service';

@Component({
  selector: 'app-partner-advance-detail-list-report',
  templateUrl: './partner-advance-detail-list-report.component.html',
  styleUrls: ['./partner-advance-detail-list-report.component.css']
})
export class PartnerAdvanceDetailListReportComponent implements OnInit {
  @Input() item: any;
  gridData: GridDataResult;
  loading = false;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  details: ReportPartnerAdvanceDetail[];
  
  constructor(
    private intlService: IntlService,
    private reportService: AccountCommonPartnerReportService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }


  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    var val = new ReportPartnerAdvanceDetailFilter();
    val.companyId = this.item.companyId || '';
    val.dateFrom = this.item.dateFrom ? this.intlService.formatDate(new Date(this.item.dateFrom), 'yyyy-MM-dd') : '';
    val.dateTo = this.item.dateTo ? this.intlService.formatDate(new Date(this.item.dateTo), 'yyyy-MM-dd') : '';
    val.partnerId = this.item.partnerId || '';

    this.reportService.reportPartnerAdvanceDetail(val).subscribe(res => {
      this.details = res;
      this.loadItems();
    }, err => {
      console.log(err);
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
