import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';

export class AccountCommonPartnerReportItem {
    partnerId: string;
    partnerName: string;
    partnerRef: string;
    partnerPhone: string;
    begin: number;
    debit: number;
    credit: number;
    end: number;
    dateFrom: string;
    dateTo: string;
    resultSelection: string;
}

export class AccountCommonPartnerReportItemDetail {
    date: string;
    name: string;
    ref: string;
    moveName: string;
    begin: number;
    debit: number;
    credit: number;
    end: number;
}

export class AccountCommonPartnerReportSearch {
    fromDate: string;
    toDate: string;
    partnerId: string;
    resultSelection: string;
    display: string;
    search: string;
}

@Injectable()
export class AccountCommonPartnerReportService {
    apiUrl = 'api/AccountCommonPartnerReports';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getSummary(val: AccountCommonPartnerReportSearch): Observable<AccountCommonPartnerReportItem[]> {
        return this.http.post<AccountCommonPartnerReportItem[]>(this.baseApi + this.apiUrl + "/GetSummary", val);
    }

    getDetail(val: AccountCommonPartnerReportItem): Observable<AccountCommonPartnerReportItemDetail[]> {
        return this.http.post<AccountCommonPartnerReportItemDetail[]>(this.baseApi + this.apiUrl + "/GetDetail", val);
    }
}