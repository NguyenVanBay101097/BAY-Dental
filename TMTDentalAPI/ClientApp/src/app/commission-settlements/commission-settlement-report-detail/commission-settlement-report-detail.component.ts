import { Component, OnInit, Input } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { CommissionSettlementReport, CommissionSettlementReportDetailOutput, CommissionSettlementsService, CommissionSettlementReportOutput } from '../commission-settlements.service';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-commission-settlement-report-detail',
  templateUrl: './commission-settlement-report-detail.component.html',
  styleUrls: ['./commission-settlement-report-detail.component.css']
})
export class CommissionSettlementReportDetailComponent implements OnInit {
  loading = false;
  gridData: GridDataResult;
  skip = 0;
  limit = 5;
  reportDetailResults: CommissionSettlementReportDetailOutput[];

  @Input() public item: CommissionSettlementReportOutput;

  constructor(
    private commissionSettlementsService: CommissionSettlementsService,
  ) { }

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var filter = new CommissionSettlementReport();
    filter.offset = this.skip;
    filter.limit = this.limit;
    filter.employeeId = this.item.employeeId;
    filter.companyId = this.item.companyId;
    filter.dateFrom = this.item.dateFrom;
    filter.dateTo = this.item.dateTo;

    this.commissionSettlementsService.getReportDetail(filter).pipe(
      map((response: any) => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe((res: any) => {
      this.reportDetailResults = res;
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }
}
