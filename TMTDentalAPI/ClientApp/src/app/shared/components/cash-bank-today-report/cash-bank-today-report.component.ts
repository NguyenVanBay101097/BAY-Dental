import { Component, OnInit } from '@angular/core';
import { IntlService } from '@progress/kendo-angular-intl';
import { forkJoin } from 'rxjs';
import { AuthService } from 'src/app/auth/auth.service';
import { CashBookService } from 'src/app/cash-book/cash-book.service';

@Component({
  selector: 'app-cash-bank-today-report',
  templateUrl: './cash-bank-today-report.component.html',
  styleUrls: ['./cash-bank-today-report.component.css']
})
export class CashBankTodayReportComponent implements OnInit {

  reportValueCash: any;
  reportValueBank: any;

  constructor(private intlService: IntlService,
    private authService: AuthService,
    private cashBookService: CashBookService
  ) { }

  ngOnInit() {
    this.loadDataMoney();
  }

  loadDataMoney() {
    var companyId = this.authService.userInfo.companyId;
    var dateFrom = this.intlService.formatDate(new Date(), 'yyyy-MM-dd');
    var dateTo = this.intlService.formatDate(new Date(), 'yyyy-MM-ddT23:59');
    let cash = this.cashBookService.getTotal({ resultSelection: "cash", dateFrom: dateFrom, dateTo: dateTo, companyId: companyId });
    let bank = this.cashBookService.getTotal({ resultSelection: "bank", dateFrom: dateFrom, dateTo: dateTo, companyId: companyId });
    forkJoin([cash, bank]).subscribe(results => {
      this.reportValueCash = results[0];
      this.reportValueBank = results[1];
    });
  }

}
