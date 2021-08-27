import { KeyValue } from '@angular/common';
import { Component, Input, OnInit, SimpleChanges } from '@angular/core';
import { IntlService } from '@progress/kendo-angular-intl';
import { AccountInvoiceReportService, RevenueReportFilter, RevenueReportItem } from 'src/app/account-invoice-reports/account-invoice-report.service';
import { CashBookReportItem } from 'src/app/cash-book/cash-book.service';
import { RevenueReportService } from 'src/app/revenue-report/revenue-report.service';

@Component({
  selector: 'app-sale-dashboard-invoice-report',
  templateUrl: './sale-dashboard-invoice-report.component.html',
  styleUrls: ['./sale-dashboard-invoice-report.component.css']
})
export class SaleDashboardInvoiceReportComponent implements OnInit {
  @Input() groupby: string;
  @Input() revenues: RevenueReportItem[] = [];
  @Input() cashBooks: any;
  @Input() dataRevenues: any[] = [];
  @Input() dataCashBooks: any;
  @Input() totalDataCashBook: any;
  cashBookData: CashBookReportItem[] = [];
  revenuCateg: any[] = [];
  public revenueCashBank: any;
  public revenueDebt: any;
  public revenueAdvance: any;
  public revenueCusDebt: any;
  public revenueCusAdvance: any;
  public cashbookCashBank: any;
  public cashbookCusDebt: any;
  public cashbookCusAdvance: any;
  revenueSeries: any[] = [];
  revenue: any;
  realIncome: any;


  constructor(private revenueReportService: AccountInvoiceReportService,
    private intlService: IntlService) { }

  ngOnChanges(changes: SimpleChanges): void {
    this.loadCashbookGroupby();
    this.loadRevenueSeries();
    this.loadDataCashbookSeries();
  }

  ngOnInit() {
  }

  loadRevenueSeries() {
    if (this.dataRevenues) {
      this.revenueCashBank = this.dataRevenues[0];
      this.revenueCusDebt = this.dataRevenues[1];
      this.revenueCusAdvance = this.dataRevenues[2];
    }
  }

  loadDataCashbookSeries() {
    this.revenueSeries = [];
    if (this.cashBooks && this.revenues && this.dataCashBooks) {
      this.cashBookData = this.cashBooks;
      this.realIncome = this.cashBookData;
      this.revenue = this.revenues;    
      this.cashbookCashBank = this.dataCashBooks[0];
      this.cashbookCusDebt = this.dataCashBooks[1];
      this.cashbookCusAdvance = this.dataCashBooks[2];
    }

  }

  get totalDebit() {
    if(this.dataCashBooks){
      return (this.revenueCashBank.balance ? this.revenueCashBank.balance : 0) + (this.revenueCusDebt.debit || 0) + (this.revenueCusAdvance.debit || 0);
    }
   
    return 0;
  }

  get ortherThu() {
    if (this.totalDataCashBook && this.cashbookCashBank) {
      return (this.totalDataCashBook.totalThu || 0) - ((this.cashbookCashBank.credit ? this.cashbookCashBank.credit : 0) + (this.cashbookCusDebt.credit || 0) + (this.cashbookCusAdvance.credit || 0));
    }

    return 0;
  }

  loadCashbookGroupby() {
    if (this.revenues && this.cashBookData) {
      var dateRevenues = this.revenues.map(s => this.groupby == 'groupby:day' ? this.intlService.formatDate(new Date(s.invoiceDate), 'dd/MM/yyyy') : this.intlService.formatDate(new Date(s.invoiceDate), 'MM/yyyy'));
      var dateCashbooks = this.cashBookData.map(s => this.groupby == 'groupby:day' ? this.intlService.formatDate(new Date(s.date), 'dd/MM/yyyy') : this.intlService.formatDate(new Date(s.date), 'MM/yyyy'));
      this.revenuCateg = this.arrayUnique(dateRevenues.concat(dateCashbooks).sort());
    }
  }

  
  arrayUnique(array) {
    var a = array.concat();
    for (var i = 0; i < a.length; ++i) {
      for (var j = i + 1; j < a.length; ++j) {
        if (a[i] === a[j])
          a.splice(j--, 1);
      }
    }
    return a;
  }



  public labelContent = (e: any) => {
    var res = e.value;
    return res;
  };


}
