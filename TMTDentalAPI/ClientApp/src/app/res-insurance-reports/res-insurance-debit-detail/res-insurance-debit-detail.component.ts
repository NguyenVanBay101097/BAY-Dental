import { Component, Inject, Input, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import * as moment from 'moment';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { InsuranceDebtDetailFilter, InsuranceDebtDetailItem } from '../res-insurance-report.model';
import { ResInsuranceReportService } from '../res-insurance-report.service';

@Component({
  selector: 'app-res-insurance-debit-detail',
  templateUrl: './res-insurance-debit-detail.component.html',
  styleUrls: ['./res-insurance-debit-detail.component.css']
})
export class ResInsuranceDebitDetailComponent implements OnInit {
  @Input() item: any;
  gridData: GridDataResult;
  limit: number = 20;
  skip: number = 0;
  items: InsuranceDebtDetailItem[];
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
    if(this.item){
      let val = new InsuranceDebtDetailFilter();
      val.dateFrom = this.item.dateFrom ? moment(this.item.dateFrom).format('YYYY-MM-DD') : '';
      val.dateTo = this.item.dateTo ? moment(this.item.dateTo).format('YYYY-MM-DD') : '';
      val.paymentId = this.item.paymentId || '';
      this.resInsuranceReportService.getInsuranceDebtDetailReport(val).subscribe((res) => {
        this.items = res;
        // this.loadItems();
      }, (error) => console.log(error));
    }
   
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
