import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { map } from 'rxjs/operators';
import { CommissionSettlementFilterReport, CommissionSettlementsService } from '../../commission-settlements.service';

@Component({
  selector: 'app-commission-settlement-agent-commission',
  templateUrl: './commission-settlement-agent-commission.component.html',
  styleUrls: ['./commission-settlement-agent-commission.component.css']
})
export class CommissionSettlementAgentCommissionComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  dateFrom: Date;
  dateTo: Date;
  agentId: string;
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  
  constructor(
    private intl: IntlService,
    private commissionSettlementsService: CommissionSettlementsService
  ) { }

  ngOnInit() {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new CommissionSettlementFilterReport();
    val.offset = this.skip;
    val.limit = this.limit;
    val.search = this.search ? this.search : '';
    val.agentId = this.agentId ? this.agentId : '';
    // val.commissionType = this.commissionType ? this.commissionType : '';
    val.dateFrom = this.dateFrom ? this.intl.formatDate(this.dateFrom, 'yyyy-MM-ddTHH:mm:ss') : null;
    val.dateTo = this.dateFrom ? this.intl.formatDate(this.dateTo, 'yyyy-MM-ddTHH:mm:ss') : null;
    val.groupBy = 'employee';
    this.commissionSettlementsService.getReportDetail(val).pipe(
      map((response: any) => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe((res: any) => {
      this.gridData = res;
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    });
  }

  onSearchDateChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.skip = 0;
    // this.loadDataFromApi();
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    // this.loadDataFromApi();
  }

  exportExcelFile(){
    var val = new CommissionSettlementFilterReport();
    val.dateFrom = this.dateFrom ? this.intl.formatDate(this.dateFrom, 'yyyy-MM-ddTHH:mm:ss') : '';
    val.dateTo = this.dateTo ? this.intl.formatDate(this.dateTo, 'yyyy-MM-ddTHH:mm:ss') : '';
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.agentId = this.agentId ? this.agentId : '';
    // val.commissionType = this.commissionType ? this.commissionType : '';
    val.groupBy = 'agent';
    this.commissionSettlementsService.excelCommissionDetailExport(val).subscribe((res: any) => {
      let filename = "Hoa hồng người giới thiệu";
      let newBlob = new Blob([res], {
        type:
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      });
      let data = window.URL.createObjectURL(newBlob);
      let link = document.createElement("a");
      link.href = data;
      link.download = filename;
      link.click();
      setTimeout(() => {
        window.URL.revokeObjectURL(data);
      }, 100);
    })
  }
}
