import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';

export class SaleReportItem {
    date: string;
    productUOMQty: number;
    priceTotal: number;
    name: string;
}

export class SaleReportSearch {
    dateFrom: string;
    dateTo: string;
    groupBy: string;
    search: string;
}

@Injectable()
export class SaleReportService {
    apiUrl = 'api/SaleReports';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getReport(val: SaleReportSearch): Observable<SaleReportItem[]> {
        return this.http.post<SaleReportItem[]>(this.baseApi + this.apiUrl + "/GetReport", val);
    }
}