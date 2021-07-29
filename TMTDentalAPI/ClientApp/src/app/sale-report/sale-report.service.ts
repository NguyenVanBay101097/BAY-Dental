import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { SaleOrderLineDisplay } from '../sale-orders/sale-order-line-display';
import { CheckPermissionService } from '../shared/check-permission.service';
import { PagedResult2 } from '../employee-categories/emp-category';

export class SaleReportItem {
    date: string;
    productUOMQty: number;
    priceTotal: number;
    name: string;
}

export class SaleReportPartnerItemV3 {
    weekStart: string;
    weekEnd: string;
    weekOfYear: number;
    year: number;
    totalNewPartner: number;
    totalOldPartner: number;
    lines: SaleReportPartnerItemV3Detail[];
}

export class SaleReportPartnerItemV3Detail {
    date: string;
    partnerId: string;
    partnerName: string;
    orderName: string;
    countLine: number;
    type: string;
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

export class SaleReportPartnerSearchV3 {
    dateTo: string;
    dateFrom: string;
    companyId: string;
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
    customerTotal: number;
    customerNew: number;
    customerOld: number;
}

export class GetSummarySaleReportRequest {
    partnerId: string;
    companyId: string;
}

export class ServiceReportReq {
	dateFrom: any;
	dateTo: any;
	employeeId: string;
	companyId: string;
	state: string;
	search: string; 
    active?: boolean;
}

export class ServiceReportRes {
	date: string;
	name?: any;
	quantity: number;
	totalAmount: number;
    productId: string;
}

export class ServiceReportDetailReq {
	dateFrom: any;
	dateTo: any;
	employeeId: string;
	companyId: string;
	state: string;
	search: string; 
    productId: string;
    offset: number;
    limit: number;
    active?: boolean;
}

export class ServiceReportDetailRes {
	date: string;
	orderPartnerName: string;
	name: string;
	employeeName?: any;
	teeth: any[];
	productUOMQty: number;
	priceSubTotal: number;
	state: string;
	orderName: string;
    isActive: boolean;
}


@Injectable({ providedIn: 'root' })
export class SaleReportService {
    apiUrl = 'api/SaleReports';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string, 
        private checkPermissionService: CheckPermissionService) { }

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

    getReportPartnerV3(val: SaleReportPartnerSearchV3): Observable<SaleReportPartnerItemV3[]> {
        return this.http.post<SaleReportPartnerItemV3[]>(this.baseApi + this.apiUrl + "/GetReportPartner", val);
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

    getSummary(val: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/GetSummary', val);
    }

    getServiceReportByTime(val: ServiceReportReq){
        return this.http.post<ServiceReportRes[]>(this.baseApi + this.apiUrl + '/GetServiceReportByTime', val);
    }

    getServiceReportByService(val: ServiceReportReq){
        return this.http.post<ServiceReportRes[]>(this.baseApi + this.apiUrl + '/GetServiceReportByService', val);
    }

    getServiceReportDetailPaged(val: any){
        return this.http.get<PagedResult2<ServiceReportDetailRes>>(this.baseApi + this.apiUrl + '/GetServiceReportDetailPaged', {params: new HttpParams({fromObject: val})});

    }
}