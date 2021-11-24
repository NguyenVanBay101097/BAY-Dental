import { Component, Input, OnInit } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import * as moment from 'moment';
import { map } from 'rxjs/operators';
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
  items: InsuranceHistoryInComeDetailItem[];
  constructor(
    private resInsuranceReportService: ResInsuranceReportService
  ) { }

  ngOnInit(): void {
    this.loadDataFromApi();
  }

  loadDataFromApi(): void {
    let val = new InsuranceHistoryInComeDetailFilter();
    val.dateFrom = this.dateFrom ? moment(this.dateFrom).format('YYYY-MM-DD') : '';
    val.dateTo = this.dateTo ? moment(this.dateTo).format('YYYY-MM-DD') : '';
    val.paymentId = this.paymentId || '';
    this.resInsuranceReportService.getHistoryInComeDebtDetails(val).subscribe((res: any) => {
      console.log(res);
      this.items = res;
      this.loadItems();
    }, (error) => console.log(error));
  }

  loadItems(): void {
    this.gridData = {
      data: this.items,
      total: this.items.length
    };
  }

}
