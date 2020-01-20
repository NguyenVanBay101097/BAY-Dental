import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';

export class RevenueReportResult {
    debit: number;
    credit: number;
    balance: number;
    details: RevenueReportResultDetails[];
}

export class RevenueReportResultDetails {
    debit: number;
    credit: number;
    balance: number;
    name: string;
}

export class RevenueReportSearch {
    dateFrom: string;
    dateTo: string;
    groupBy: string;
    search: string;
}

@Injectable()
export class RevenueReportService {
    apiUrl = 'api/RevenueReport';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getReport(val: any): Observable<RevenueReportResult> {
        return this.http.get<RevenueReportResult>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
    }
}