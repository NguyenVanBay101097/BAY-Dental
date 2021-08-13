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
    this.revenueCashBank = this.dataRevenues[0];
    this.revenueCusDebt = this.dataRevenues[1];
    this.revenueCusAdvance = this.dataRevenues[2];
  }

  loadDataCashbookSeries() {
    this.revenueSeries = [];
    this.cashBookData = this.cashBooks;
    var cashbookThu = { name: "Thực thu", type: "column", data: this.cashBookData.map(s => s.totalThu) };
    var revenue = { name: "Doanh thu", type: "column", data: this.revenues.map(s => s.priceSubTotal) };
    this.revenueSeries.push(revenue, cashbookThu);
    this.cashbookCashBank = this.dataCashBooks[0];
    this.cashbookCusDebt = this.dataCashBooks[1];
    this.cashbookCusAdvance = this.dataCashBooks[2];
  }

  get totalDebit() {
    return this.revenueCashBank.balance + this.revenueCusDebt.debit + this.revenueCusAdvance.debit;
  }

  get ortherThu() {
    return this.totalDataCashBook.totalThu - (this.cashbookCashBank.credit + this.cashbookCusDebt.credit + this.cashbookCusAdvance.credit);
  }

  loadCashbookGroupby() {
    var dateRevenues = this.revenues.map(s => s.invoiceDate);
    var dateCashbooks = this.cashBookData.map(s => s.date);
    this.revenuCateg = this.arrayUnique(dateRevenues.concat(dateCashbooks).sort());
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
    var res = this.groupby == 'groupby:day' ? this.intlService.formatDate(new Date(e.value), 'dd/MM') : this.intlService.formatDate(new Date(e.value), 'MM/yyyy');
    return res;
  };


}
