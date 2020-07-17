import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';

export class ReportCashBankGeneralLedger {
    resultSelection: string;
    dateFrom: string;
    dateTo: string;
    companyId: string;
}

@Injectable({providedIn: 'root'})
export class AccountReportGeneralLedgerService {
    apiUrl = 'api/AccountReportGeneralLedgers';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getCashBankReport(val: ReportCashBankGeneralLedger) {
        return this.http.post(this.baseApi + this.apiUrl + "/GetCashBankReport", val);
    }
}