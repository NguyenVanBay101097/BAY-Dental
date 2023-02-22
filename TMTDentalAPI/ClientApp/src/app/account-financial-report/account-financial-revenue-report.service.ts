import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export class RevenueReportPar{
  dateFrom: any;
  dateTo: any;
  companyId: string;
}

export class FinancialRevenueReportItem {
  name: string;
  parentId: string;
  childs: FinancialRevenueReportItem[];
  level: number;
  sequence: number;
  type: string;
  balance: number;
}

@Injectable({
  providedIn: 'root'
})
export class AccountFinancialRevenueReportService { // báo cáo nguồn thu

  constructor(private http: HttpClient, @Inject("BASE_API") private base_api: string) { }
  apiUrl = "api/AccountFinancialRevenueReports";

  GetRevenueReport(val:RevenueReportPar): Observable<any> {
    return this.http.post<FinancialRevenueReportItem>(this.base_api + this.apiUrl + '/GetRevenueReport',val)
  }
}
