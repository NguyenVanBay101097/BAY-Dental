import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import * as moment from 'moment';
import { Subject } from 'rxjs';
import { AccountFinancialRevenueReportService, FinancialRevenueReportItem, RevenueReportPar } from 'src/app/account-financial-report/account-financial-revenue-report.service';
import { ReportCashBankGeneralLedger } from 'src/app/account-report-general-ledgers/account-report-general-ledger.service';

@Component({
  selector: 'app-financial-revenue-report',
  templateUrl: './financial-revenue-report.component.html',
  styleUrls: ['./financial-revenue-report.component.css']
})
export class FinancialRevenueReportComponent implements OnInit {

  FinancialRevenue: FinancialRevenueReportItem = new FinancialRevenueReportItem();
  filter: RevenueReportPar = new RevenueReportPar();
  @Input() companyChangeSubject: Subject<any>;
  constructor(
    private accFinancialRevenueService: AccountFinancialRevenueReportService
  ) { }

  ngOnInit() {
    var date = new Date(), y = date.getFullYear(), m = date.getMonth();
    this.filter.dateFrom = new Date(y, m, 1);
    this.filter.dateTo = new Date(y, m + 1, 0);

    this.loadReport();

    this.companyChangeSubject.subscribe((v: any) => {
      this.filter.companyId = v? v.id : '';
      this.loadReport();
    });
  }

  loadReport(){
    var val = Object.assign({}, this.filter);
    val.dateFrom = val.dateFrom ? moment(val.dateFrom).format('YYYY/MM/DD') : '';
    val.dateTo = val.dateTo ? moment(val.dateTo).format('YYYY/MM/DD') : '';
    
    this.accFinancialRevenueService.GetRevenueReport(val).subscribe(res => {
      this.FinancialRevenue = res;
    });
  }

  onSearchDateChange(data) {
    this.filter.dateFrom = data.dateFrom;
    this.filter.dateTo = data.dateTo;
    this.loadReport();
  }


}
