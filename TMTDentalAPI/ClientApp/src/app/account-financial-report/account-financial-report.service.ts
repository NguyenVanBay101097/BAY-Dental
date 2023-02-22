import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export class AccountFinancialReportBasic {
  name: string;
  id: string
}

@Injectable({
  providedIn: 'root'
})
export class AccountFinancialReportService {

  constructor(private http: HttpClient, @Inject("BASE_API") private base_api: string) { }
  apiUrl = "api/AccountFinancialReports";

  getProfitAndLossReport(): Observable<AccountFinancialReportBasic> {
    return this.http.get<AccountFinancialReportBasic>(this.base_api + this.apiUrl + '/GetProfitAndLossReport')
  }
}
