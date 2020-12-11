import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { SaleOrderLineDisplay } from '../sale-orders/sale-order-line-display';

export class SaleReportItem {
    date: string;
    productUOMQty: number;
    priceTotal: number;
    name: string;
}

export class SaleReportItemDetail {
    date: string;
    productUOMQty: number;
    priceTotal: number;
    name: string;
    productName: string;
}

export class SaleReportSearch {
    dateFrom: string;
    dateTo: string;
    groupBy: string;
    search: string;
    isQuotation: boolean;
    state: string;
    companyId: string;
}

export class SaleReportPartnerSearch {
    state: string;
    partnerDisplay: string;
    monthsFrom: number;
    monthsTo: number;
    search: string;
}


export class SaleReportPartnerItem {
    partnerName: string;
    partnerPhone: number;
    orderCount: number;
    lastDateOrder: string;
}

export class SaleReportTopSaleProductSearch {
    dateFrom: string;
    dateTo: string;
    topBy: string;
}

export class SaleReportOldNewPartnerInput {
    dateFrom: string;
    dateTo: string;
}

export class SaleReportOldNewPartnerDetails {
    location: string;
    partnerTotal: number;
    partnerOld: number;
    partnerNew: number;
}

export class SaleReportOldNewPartnerOutput {
    partnerTotal: number;
    partnerOld: number;
    partnerNew: number;
    details: SaleReportOldNewPartnerDetails;
}

@Injectable({ providedIn: 'root' })
export class SaleReportService {
    apiUrl = 'api/SaleReports';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getReport(val: SaleReportSearch): Observable<SaleReportItem[]> {
        return this.http.post<SaleReportItem[]>(this.baseApi + this.apiUrl + "/GetReport", val);
    }

    getReportService(val: SaleReportSearch): Observable<SaleOrderLineDisplay[]> {
        return this.http.post<SaleOrderLineDisplay[]>(this.baseApi + this.apiUrl + '/GetReportService', val);
    }

    getTopSaleProduct(val: SaleReportTopSaleProductSearch): Observable<SaleReportItem[]> {
        return this.http.post<SaleReportItem[]>(this.baseApi + this.apiUrl + "/GetTopSaleProduct", val);
    }

    getReportDetail(val: SaleReportItem): Observable<SaleReportItemDetail[]> {
        return this.http.post<SaleReportItemDetail[]>(this.baseApi + this.apiUrl + "/GetReportDetail", val);
    }

    getReportPartner(val: SaleReportPartnerSearch): Observable<SaleReportPartnerItem[]> {
        return this.http.post<SaleReportPartnerItem[]>(this.baseApi + this.apiUrl + "/GetReportPartner", val);
    }

    exportServiceReportExcelFile(val: any) {
        return this.http.post(
            this.baseApi + this.apiUrl + "/ExportServiceReportExcelFile", val,
            { responseType: "blob" }
        );
    }

    getReportOldNewPartner(val: SaleReportOldNewPartnerInput) {
        return this.http.post<SaleReportOldNewPartnerOutput>(this.baseApi + this.apiUrl + '/GetReportOldNewPartner', val);
    }
}