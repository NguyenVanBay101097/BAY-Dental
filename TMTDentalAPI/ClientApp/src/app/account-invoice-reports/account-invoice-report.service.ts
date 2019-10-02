import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';

export class AccountInvoiceReportByTimeItem {
    date: string;
    amountTotal: number;
    residual: number;
    dateStr: string;
    dateFrom: string;
    dateTo: string;
}

export class AccountInvoiceReportByTimeDetail {
    number: string;
    invoiceId: string;
    date: string;
    amountTotal: number;
    residual: number;
}

export class AccountInvoiceReportByTimeSearch {
    groupBy: string;
    dateFrom: string;
    dateTo: string;
    monthFrom: string;
    monthTo: string;
    yearFrom: string;
    yearTo: string;
}

export class AccountInvoiceReportHomeSummaryVM {
    totalInvoice: number;
    totalAmount: number;
}

@Injectable()
export class AccountInvoiceReportService {
    apiUrl = 'api/AccountInvoiceReports';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getSummaryByTime(val: AccountInvoiceReportByTimeSearch): Observable<AccountInvoiceReportByTimeItem[]> {
        return this.http.post<AccountInvoiceReportByTimeItem[]>(this.baseApi + this.apiUrl + "/GetSummaryByTime", val);
    }

    getDetailByTime(val: AccountInvoiceReportByTimeItem): Observable<AccountInvoiceReportByTimeDetail[]> {
        return this.http.post<AccountInvoiceReportByTimeDetail[]>(this.baseApi + this.apiUrl + "/GetDetailByTime", val);
    }

    getHomeTodaySummary(): Observable<AccountInvoiceReportHomeSummaryVM> {
        return this.http.post<AccountInvoiceReportHomeSummaryVM>(this.baseApi + this.apiUrl + "/GetHomeTodaySummary", {});
    }
}