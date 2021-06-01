import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';
import { PagedResult2 } from '../employee-categories/emp-category';

export class RevenueTimeReportPaged{
    dateFrom?: any;
    dateTo?: any;
    companyId?: string;
    limit: number;
    offset: number;
}

export class RevenueServiceReportPaged{
    dateFrom?: any;
    dateTo?: any;
    productId: string;
    companyId?: string;
    limit: number;
    offset: number;
}

export class RevenueEmployeeReportPaged{
    dateFrom?: any;
    dateTo?: any;
    companyId?: string;
    limit: number;
    offset: number;
    employeeGroup: boolean;
    employeeId: string;
}

export class RevenueTimeReportDisplay {
	invoiceDate?: any;
	priceSubTotal: number;
}

export class RevenueServiceReportDisplay {
	productName: string;
	productId: string;
	priceSubTotal: number;
}

export class RevenueEmployeeReportDisplay {
	employeeName: string;
	employeeId: string;
	priceSubTotal: number;
}

export class RevenueReportDetailPaged{
    dateFrom?: any;
    dateTo?: any;
    companyId?: string;
    limit: number;
    offset: number;
    date?: any;
    productId?: string;
    employeeGroup: boolean;
    employeeId: string;
    assistantGroup: boolean;
    assistantId: string;
}



export class RevenueReportDetailDisplay {
	invoiceDate: string;
	invoiceOrigin: string;
	partnerName: string;
	employeeName?: any;
	assistantName?: any;
	productName: string;
	priceSubTotal: number;
}

@Injectable()
export class AccountInvoiceReportService {
    apiUrl = 'api/AccountInvoiceReports';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getRevenueTimeReportPaged(val:any ) {
        return this.http.get<PagedResult2<RevenueTimeReportDisplay>>(this.baseApi + this.apiUrl + "/GetRevenueTimeReportPaged", {params: new HttpParams({fromObject: val})});
    }

    getRevenueServiceReportPaged(val:any ) {
        return this.http.get<PagedResult2<RevenueServiceReportDisplay>>(this.baseApi + this.apiUrl + "/GetRevenueServiceReportPaged", {params: new HttpParams({fromObject: val})});
    }

    getRevenueEmployeeReportPaged(val:any ) {
        return this.http.get<PagedResult2<RevenueEmployeeReportDisplay>>(this.baseApi + this.apiUrl + "/GetRevenueEmployeeReportPaged", {params: new HttpParams({fromObject: val})});
    }

    getRevenueReportDetailPaged(val:any ) {
        return this.http.get<PagedResult2<RevenueReportDetailDisplay>>(this.baseApi + this.apiUrl + "/GetRevenueReportDetailPaged", {params: new HttpParams({fromObject: val})});
    }
}