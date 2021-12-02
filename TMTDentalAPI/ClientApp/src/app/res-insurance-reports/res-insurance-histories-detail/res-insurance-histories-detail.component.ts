import { Component, Inject, Input, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import * as moment from 'moment';
import { map } from 'rxjs/operators';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { InsuranceHistoryInComeDetailFilter, InsuranceHistoryInComeDetailItem } from '../res-insurance-report.model';
import { ResInsuranceReportService } from '../res-insurance-report.service';

@Component({
  selector: 'app-res-insurance-histories-detail',
  templateUrl: './res-insurance-histories-detail.component.html',
  styleUrls: ['./res-insurance-histories-detail.component.css']
})
export class ResInsuranceHistoriesDetailComponent implements OnInit {
  @Input() paymentId: string;
  @Input() dateFrom: string;
  @Input() dateTo: string;
  gridData: GridDataResult;
  limit: number = 20;
  skip: number = 0;
  items: InsuranceHistoryInComeDetailItem[];
  pagerSettings: any;
  constructor(
    private resInsuranceReportService: ResInsuranceReportService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) {
    this.pagerSettings = config.pagerSettings
  }

  ngOnInit(): void {
    this.loadDataFromApi();
  }

  loadDataFromApi(): void {
    let val = new InsuranceHistoryInComeDetailFilter();
    val.dateFrom = this.dateFrom ? moment(this.dateFrom).format('YYYY-MM-DD') : '';
    val.dateTo = this.dateTo ? moment(this.dateTo).format('YYYY-MM-DD') : '';
    val.paymentId = this.paymentId || '';
    this.resInsuranceReportService.getHistoryInComeDetails(val).subscribe((res: any) => {
      this.items = res;
      this.loadItems();
    }, (error) => console.log(error));
  }

  loadItems(): void {
    this.gridData = {
      data: this.items.slice(this.skip, this.skip + this.limit),
      total: this.items.length
    };
  }

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadItems();
  }
}
