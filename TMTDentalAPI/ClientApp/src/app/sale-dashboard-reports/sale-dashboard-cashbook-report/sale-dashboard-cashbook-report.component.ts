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
    if (this.cashBooks) {
      this.cashBookData = this.cashBooks;
      this.cashbookSeries = [];
      this.loadCashbookGroupby();
      this.loadDataCashbookSeries();
    }
  }

  public labelContent = (e: any) => {
    var res = this.groupby == 'groupby:day' ? this.intlService.formatDate(new Date(e.value), 'dd/MM') : this.intlService.formatDate(new Date(e.value), 'MM/yyyy');
    return res;
  };


  loadDataCashbookSeries() {
    if (this.dataCashBooks && this.totalDataCashBook) {
      this.cashbookCashBank = this.dataCashBooks[0];
      this.cashbookCusDebt = this.dataCashBooks[1];
      this.cashbookCusAdvance = this.dataCashBooks[2];
      this.cashbookSupp = this.dataCashBooks[3];
      this.cashbookCusSalary = this.dataCashBooks[4];
      this.cashbookAgentCommission = this.dataCashBooks[5];
      this.totalCashbook = this.totalDataCashBook;
    }

  }

  get ortherThu() {
    if (this.totalCashbook && this.dataCashBooks) {
      return (this.totalCashbook.totalThu ? this.totalCashbook.totalThu : 0) - (this.dataCashBooks.reduce((total, val) => total += val.credit, 0));
    }

    return 0;
  }

  get ortherChi() {
    if (this.totalCashbook) {
      return (this.totalCashbook.totalChi ? this.totalCashbook.totalChi : 0) - (this.cashbookSupp.debit + this.cashbookCusAdvance.debit + this.cashbookCusSalary.debit + this.cashbookAgentCommission.debit);
    }

    return 0;

  }

  get totalThu() {
    return (this.cashbookCashBank ? this.cashbookCashBank.credit : 0) + (this.cashbookCusAdvance ? this.cashbookCusAdvance.credit : 0) + (this.cashbookCusDebt ? this.cashbookCusDebt.credit : 0) + (this.cashbookSupp ? this.cashbookSupp.credit : 0) + this.ortherThu;
  }

  get totalChi() {
    return (this.cashbookSupp ? this.cashbookSupp.debit : 0) + (this.cashbookCusAdvance ? this.cashbookCusAdvance.debit : 0) + (this.cashbookCusSalary ? this.cashbookCusSalary.debit : 0) + (this.cashbookAgentCommission ? this.cashbookAgentCommission.debit : 0) + this.ortherChi;
  }

  loadCashbookGroupby() {
    if (this.cashBookData) {
      this.cashbookCategs = this.cashBookData.map(s => s.date);
    }
  }

  redirectTo() {
    return this.router.navigateByUrl("cash-book/tab-cabo");
  }

}
