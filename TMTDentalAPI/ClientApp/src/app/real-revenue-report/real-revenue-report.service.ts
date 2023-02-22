import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export class RealRevenueReportItem {
    name: string;
    debit: number;
    credit: number;
    balance: number;
    amountResidual: number;
}

export class RealRevenueReportResult {
    debit: number;
    credit: number;
    begin: number;
    end: number;
}

export class RealRevenueReportItemDetail {
    date: string;
    debit: number;
    credit: number;
    balance: number;
    name: string;
    ref: string;
}


export class RealRevenueReportSearch {
    dateFrom: string;
    dateTo: string;
    companyId: string;
}

@Injectable()
export class RealRevenueReportService {
    apiUrl = 'api/RealRevenueReport';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getReport(val: RealRevenueReportSearch): Observable<RealRevenueReportResult> {
        return this.http.post<RealRevenueReportResult>(this.baseApi + this.apiUrl + "/GetReport", val);
    }

    getReportDetail(val: RealRevenueReportItem): Observable<RealRevenueReportItemDetail[]> {
        return this.http.post<RealRevenueReportItemDetail[]>(this.baseApi + this.apiUrl + "/GetReportDetail", val);
    }
}