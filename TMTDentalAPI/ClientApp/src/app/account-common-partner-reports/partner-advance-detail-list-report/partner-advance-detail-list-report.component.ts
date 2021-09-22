import { Component, Inject, Input, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';

@Component({
  selector: 'app-partner-advance-detail-list-report',
  templateUrl: './partner-advance-detail-list-report.component.html',
  styleUrls: ['./partner-advance-detail-list-report.component.css']
})
export class PartnerAdvanceDetailListReportComponent implements OnInit {
  @Input('parent') parent: any;
  gridData: GridDataResult;
  loading = false;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  // details: ReportPartnerDebitDetailRes[];
  
  constructor(
    private intlService: IntlService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }


  ngOnInit() {
  }

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    // this.loadItems();
  }
}
