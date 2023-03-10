import { Component, Input, OnInit } from '@angular/core';
import { IntlService } from '@progress/kendo-angular-intl';
import { forkJoin } from 'rxjs';
import { AuthService } from 'src/app/auth/auth.service';
import { CashBookService } from 'src/app/cash-book/cash-book.service';

@Component({
  selector: 'app-dashboard-cash-bank-report',
  templateUrl: './dashboard-cash-bank-report.component.html',
  styleUrls: ['./dashboard-cash-bank-report.component.css']
})
export class DashboardCashBankReportComponent implements OnInit {

  @Input() getToday = false;

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
    let cash = this.cashBookService.getTotal({ resultSelection: "cash", companyId: companyId });
    let bank = this.cashBookService.getTotal({ resultSelection: "bank", companyId: companyId });
    if (this.getToday) {
      var dateFrom = this.intlService.formatDate(new Date(), 'yyyy-MM-dd');
      var dateTo = this.intlService.formatDate(new Date(), 'yyyy-MM-ddT23:59');
      cash = this.cashBookService.getTotal({ resultSelection: "cash", dateFrom: dateFrom, dateTo: dateTo, companyId: companyId });
      bank = this.cashBookService.getTotal({ resultSelection: "bank", dateFrom: dateFrom, dateTo: dateTo, companyId: companyId });
    }
    forkJoin([cash, bank]).subscribe(results => {
      this.reportValueCash = results[0];
      this.reportValueBank = results[1];
    });
  }
}
