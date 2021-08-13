import { filter } from 'rxjs/operators';
import { Component, Input, OnInit, SimpleChanges } from '@angular/core';
import { Router } from '@angular/router';
import { IntlService, load } from '@progress/kendo-angular-intl';
import { CashBookReportFilter, CashBookReportItem, CashBookService } from 'src/app/cash-book/cash-book.service';

@Component({
  selector: 'app-sale-dashboard-cashbook-report',
  templateUrl: './sale-dashboard-cashbook-report.component.html',
  styleUrls: ['./sale-dashboard-cashbook-report.component.css']
})
export class SaleDashboardCashbookReportComponent implements OnInit {
  @Input() groupby: string;
  @Input() cashBooks: any;
  @Input() dataCashBooks: any;
  @Input() totalDataCashBook: any;
  cashBookData: CashBookReportItem[] = [];
  cashbookThu: any[] = [];
  cashbookChi: any[] = [];
  cashbookTotal: any[] = [];
  cashbookCategs: any[] = [];
  cashbookSeries: any[] = [];
  public cashbookCashBank: any;
  public cashbookCusDebt: any;
  public cashbookCusAdvance: any;
  public cashbookSuppRefund: any;
  public cashbookSupp: any;
  public cashbookCusSalary: any;
  public cashbookAgentCommission: any;
  public totalCashbook: any;

  constructor(private cashBookService: CashBookService,
    private router: Router,
    private intlService: IntlService) { }

  ngOnChanges(changes: SimpleChanges): void {
    this.loadDataApi();
  }

  ngOnInit() {
    this.loadDataApi();
  }

  loadDataApi() {
    this.cashBookData = this.cashBooks;
    this.cashbookSeries = [];
    this.loadCashbookGroupby();
    this.loadCashbookSeries();
    this.loadDataCashbookSeries();
  }

  public labelContent = (e: any) => {
    var res = this.groupby == 'groupby:day' ? this.intlService.formatDate(new Date(e.value), 'dd/MM') : this.intlService.formatDate(new Date(e.value), 'MM/yyyy');
    return res;
  };

  loadCashbookSeries() {
    var cashbookThu = { name: "Thu", type: "column", data: this.cashBookData.map(s => s.totalThu) };
    var cashbookChi = { name: "Chi", type: "column", data: this.cashBookData.map(s => s.totalChi) };
    var cashbookTotalAmount = { name: "Tồn sổ quỹ", type: "line", data: this.cashBookData.map(s => s.totalAmount) };
    this.cashbookSeries.push(cashbookThu, cashbookChi, cashbookTotalAmount);
  }

  loadDataCashbookSeries() {
    this.cashbookCashBank = this.dataCashBooks[0];
    this.cashbookCusDebt = this.dataCashBooks[1];
    this.cashbookCusAdvance = this.dataCashBooks[2];
    this.cashbookSupp = this.dataCashBooks[3];
    this.cashbookCusSalary = this.dataCashBooks[4];
    this.cashbookAgentCommission = this.dataCashBooks[5];
    this.totalCashbook = this.totalDataCashBook;
  }

  get ortherThu() {
    return this.totalCashbook.totalThu - (this.dataCashBooks.reduce((total, val) => total += val.credit, 0));
  }

  get ortherChi() {
    return this.totalCashbook.totalChi - (this.cashbookSupp.debit + this.cashbookCusAdvance.debit + this.cashbookCusSalary.debit + this.cashbookAgentCommission.debit);
  }

  loadCashbookGroupby() {
    this.cashbookCategs = this.cashBookData.map(s => s.date);
  }

  redirectTo() {
    return this.router.navigateByUrl("cash-book/tab-cabo");
  }

}
