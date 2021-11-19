import { Component, Inject, OnInit } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';

@Component({
  selector: 'app-res-insurance-histories',
  templateUrl: './res-insurance-histories.component.html',
  styleUrls: ['./res-insurance-histories.component.css']
})
export class ResInsuranceHistoriesComponent implements OnInit {
  gridData: GridDataResult;
  limit: number = 20;
  skip: number = 0;
  pagerSettings: any;
  loading = false;
  dateFrom: Date;
  dateTo: Date;
  constructor(
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit(): void {
  }

  onSearchDateChange(e): void {

  }

  pageChange(e): void {

  }
}
