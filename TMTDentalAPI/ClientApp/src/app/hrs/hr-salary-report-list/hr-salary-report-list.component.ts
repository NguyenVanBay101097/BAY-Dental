import { Component, OnInit } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { AccountCommonPartnerReportItem } from 'src/app/account-common-partner-reports/account-common-partner-report.service';
import { PartnerSimple } from 'src/app/partners/partner-simple';

@Component({
  selector: 'app-hr-salary-report-list',
  templateUrl: './hr-salary-report-list.component.html',
  styleUrls: ['./hr-salary-report-list.component.css']
})
export class HrSalaryReportListComponent implements OnInit {
  loading = false;
  items: AccountCommonPartnerReportItem[];
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  dateFrom: Date;
  dateTo: Date;
  searchPartner: PartnerSimple;
  resultSelection: string;

  search: string;
  searchUpdate = new Subject<string>();

  public total: any;
  public aggregates: any[] = [
    { field: 'end', aggregate: 'sum' },
  ];

  constructor() { }

  ngOnInit() {
  }

}
