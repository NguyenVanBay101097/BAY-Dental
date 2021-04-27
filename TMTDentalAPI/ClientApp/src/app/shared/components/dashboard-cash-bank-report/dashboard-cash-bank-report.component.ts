import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-dashboard-cash-bank-report',
  templateUrl: './dashboard-cash-bank-report.component.html',
  styleUrls: ['./dashboard-cash-bank-report.component.css']
})
export class DashboardCashBankReportComponent implements OnInit {

  reportValueCash: any;
  reportValueBank: any;

  constructor() { }

  ngOnInit() {
  }

}
