import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';

export class StockReportXuatNhapTonItem {
    productId: string;
    productName: string;
    productCode: string;
    begin: number;
    import: number;
    export: number;
    end: number;
    dateFrom: string;
    dateTo: string;
}

export class StockReportXuatNhapTonItemDetail {
    date: string;
    begin: number;
    import: number;
    export: number;
    end: number;
    movePickingName: string;
    movePickingId: string;
    movePickingTypeId: string;
}

export class StockReportXuatNhapTonSearch {
    dateFrom: string;
    dateTo: string;
    productId: string;
    productCategId: string;
    search: string;
}

@Injectable()
export class StockReportService {
    apiUrl = 'api/StockReports';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getXuatNhapTonSummary(val: StockReportXuatNhapTonSearch): Observable<StockReportXuatNhapTonItem[]> {
        return this.http.post<StockReportXuatNhapTonItem[]>(this.baseApi + this.apiUrl + "/XuatNhapTonSummary", val);
    }

    getXuatNhapTonDetail(val: StockReportXuatNhapTonItem): Observable<StockReportXuatNhapTonItemDetail[]> {
        return this.http.post<StockReportXuatNhapTonItemDetail[]>(this.baseApi + this.apiUrl + "/XuatNhapTonDetail", val);
    }
    
    exportExcel(paged) {
        return this.http.post(this.baseApi + this.apiUrl + "/ExportExcelFile", paged,{ responseType: "blob" });
    }
}