import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { PagedResult2 } from '../employee-categories/emp-category';

export class AccountInvoiceReportPaged{
    dateFrom?: any;
    dateTo?: any;
    search: string;
    companyId?: string;
    groupBy: string;
    limit: number;
    offset: number;
}

export class AccountInvoiceReportDetailPaged{
    dateFrom?: any;
    dateTo?: any;
    search: string;
    companyId?: string;
    limit: number;
    offset: number;
    date?: any;
    productId?: string;
    employeeId?: string;
    assistantId?: string;
}

export class AccountInvoiceReportDisplay {
	invoiceDate?: any;
	productName: string;
	productId: string;
	employeeName?: any;
	employeeId?: any;
	assistantName?: any;
	assistantId?: any;
	priceSubTotal: number;
}

export class AccountInvoiceReporDetailtDisplay {
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

    getRevenueReportPaged(val:any ) {
        return this.http.get<PagedResult2<AccountInvoiceReportDisplay>>(this.baseApi + this.apiUrl + "/GetRevenueReportPaged", {params: new HttpParams({fromObject: val})});
    }

    getRevenueReportDetailPaged(val:any ) {
        return this.http.get<PagedResult2<AccountInvoiceReporDetailtDisplay>>(this.baseApi + this.apiUrl + "/GetRevenueReportDetailPaged", {params: new HttpParams({fromObject: val})});
    }
}