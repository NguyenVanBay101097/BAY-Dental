import { Component, Input, OnInit } from '@angular/core';
import { AccountCommonPartnerReport, AccountCommonPartnerReportService } from 'src/app/account-common-partner-reports/account-common-partner-report.service';

@Component({
  selector: 'app-partner-overview-report',
  templateUrl: './partner-overview-report.component.html',
  styleUrls: ['./partner-overview-report.component.css']
})
export class PartnerOverviewReportComponent implements OnInit {
  @Input() accountCommonPartnerReport: AccountCommonPartnerReport = new AccountCommonPartnerReport();

  constructor(

  ) { }

  ngOnInit() {

  }
}