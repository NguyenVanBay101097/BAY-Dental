import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export class AccountLineItem {
  name: string;
  balance: number;
  type: string;
  level: number;
  accountType: string;
  debit: number;
  credit: number;
}

export class AccoutingReport {
  constructor() {
    this.filterCmp = "filter_no";
  }
  filterCmp: string;
  enableFilter: boolean;
  accountReportId: string;
  labelFilter: string;
  dateFromCmp: string;
  dateToCmp: string;
  debitCredit: boolean;
  dateFrom: string;
  companyId: string;
  dateTo: string;
  targetMove: string;
}

@Injectable({
  providedIn: 'root'
})
export class AccountFinancialReportService {

  constructor(private http: HttpClient, @Inject("BASE_API") private base_api: string) { }
  apiUrl = "api/AccountFinancialReports";

  getAccountLinesItem(val): Observable<AccountLineItem[]> {
    return this.http.post<AccountLineItem[]>(this.base_api + this.apiUrl + '/GetAccountMoveLines', val)
  }
}
