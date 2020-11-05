import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
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

export class AccountCommonPartnerReport {
    partnerId: string;
    debit: number;
    credit: number;
    initialBalance: number;
    countSaleOrder: number;
}

@Injectable()
export class AccountCommonPartnerReportService {
    apiUrl = 'api/AccountCommonPartnerReports';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getSummary(val: AccountCommonPartnerReportSearch): Observable<AccountCommonPartnerReportItem[]> {
        return this.http.post<AccountCommonPartnerReportItem[]>(this.baseApi + this.apiUrl + "/GetSummary", val);
    }

    getSummaryByPartner(id: string): Observable<AccountCommonPartnerReport> {
        const headerSettings: { [name: string]: string | string[]; } = {};
        headerSettings['Content-Type'] = 'application/json';
        headerSettings['Authorization'] = 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJjZTk1YzgyNi05MjQ4LTRkN2EtODAwMC04MDdmZjZkNTkzMGMiLCJ1bmlxdWVfbmFtZSI6ImZveGp1ZG85NSIsImNvbXBhbnlfaWQiOiIyOTg3ZmRmMS04ZWUzLTRhODctMDQ4MS0wOGQ4MzQzMDY5OWEiLCJ1c2VyX3Jvb3QiOiJUcnVlIiwibmJmIjoxNjA0NDAzMzA0LCJleHAiOjE2MDUwMDgxMDQsImlhdCI6MTYwNDQwMzMwNH0.rO8EITO37Z_v9OG4rlkDM3AqkzhDe8wYfUCKvGvSb_g';
        const newHeader = new HttpHeaders(headerSettings);
        return this.http.get<AccountCommonPartnerReport>(this.baseApi + this.apiUrl + "/GetSummaryByPartner/" + id, { headers: newHeader });
    }

    getDetail(val: AccountCommonPartnerReportItem): Observable<AccountCommonPartnerReportItemDetail[]> {
        return this.http.post<AccountCommonPartnerReportItemDetail[]>(this.baseApi + this.apiUrl + "/GetDetail", val);
    }
}