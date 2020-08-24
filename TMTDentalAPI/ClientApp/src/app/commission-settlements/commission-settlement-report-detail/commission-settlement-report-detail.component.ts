import { Component, OnInit, Input } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { CommissionSettlementReport, CommissionSettlementReportDetailOutput, CommissionSettlementsService } from '../commission-settlements.service';

@Component({
  selector: 'app-commission-settlement-report-detail',
  templateUrl: './commission-settlement-report-detail.component.html',
  styleUrls: ['./commission-settlement-report-detail.component.css']
})
export class CommissionSettlementReportDetailComponent implements OnInit {
  loading = false;
  gridData: GridDataResult;
  skip = 0;
  limit = 10;
  reportDetailResults: CommissionSettlementReportDetailOutput[];

  @Input() public employeeId: string;
  @Input() public filter: CommissionSettlementReport;
  
  constructor(
    private commissionSettlementsService: CommissionSettlementsService,
  ) { }

  ngOnInit() {
    this.filter.employeeId = this.employeeId;
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;

    this.commissionSettlementsService.getReportDetail(this.filter).subscribe(res => {
      this.reportDetailResults = res;
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }
}
