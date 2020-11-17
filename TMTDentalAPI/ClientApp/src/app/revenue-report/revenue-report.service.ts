import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { RealRevenueReportResult } from '../real-revenue-report/real-revenue-report.service';

export class RevenueReportResult {
    debit: number;
    credit: number;
    balance: number;
    company: any;
    details: RevenueReportResultDetails[];
}

export class RevenueReportResultDetails {
    debit: number;
    credit: number;
    balance: number;
    name: string;
    month: number;
    day: number;
}

export class RevenueReportSearch {
    dateFrom: string;
    dateTo: string;
    groupBy: string;
    search: string;
    companyId: string;
}

@Injectable()
export class RevenueReportService {
    apiUrl = 'api/RevenueReport';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getReport(val: any): Observable<RevenueReportResult> {
        return this.http.get<RevenueReportResult>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
    }
}