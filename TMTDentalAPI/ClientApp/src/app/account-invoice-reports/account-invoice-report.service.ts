import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';
import { PagedResult2 } from '../employee-categories/emp-category';

export class RevenueTimeReportPar{
    dateFrom?: any;
    dateTo?: any;
    companyId?: string;
}

export class RevenueServiceReportPar{
    dateFrom?: any;
    dateTo?: any;
    productId: string;
    companyId?: string;
}

export class RevenueEmployeeReportPar{
    dateFrom?: any;
    dateTo?: any;
    companyId?: string;
    groupBy: string;
    groupById: string;
}

export class RevenuePartnerReportPar{
    dateFrom?: any;
    dateTo?: any;
    companyId?: string;
    search?: string;
}

export class RevenueTimeReportDisplay {
	invoiceDate?: any;
	priceSubTotal: number;
    dateFrom?: any;
    dateTo?: any;
    companyId?: string;
}

export class RevenueServiceReportDisplay {
	productName: string;
	productId: string;
	priceSubTotal: number;
    dateFrom?: any;
    dateTo?: any;
    companyId?: string;
}

export class RevenueEmployeeReportDisplay {
	employeeName: string;
	employeeId: string;
	priceSubTotal: number;
    dateFrom?: any;
    dateTo?: any;
    companyId?: string;
    groupBy: string;
    toDetailEmployeeId: string;
}

export class RevenuePartnerReportDisplay {
	productName: string;
	productId: string;
	priceSubTotal: number;
    dateFrom?: any;
    dateTo?: any;
    companyId?: string;
    search?: string;
}

export class RevenueReportDetailPaged{
    dateFrom?: any;
    dateTo?: any;
    companyId?: string;
    limit: number;
    offset: number;
    productId?: string;
    employeeId?: string;
    assistantId?: string;
    partnerId?: string;
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

export class SumRevenueReportPar{
    dateFrom?: any;
    dateTo?: any;
    companyId?: string;
    partnerId?: string;
}
@Injectable()
export class AccountInvoiceReportService {
    apiUrl = 'api/AccountInvoiceReports';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getRevenueTimeReport(val:any ) {
        return this.http.get<RevenueTimeReportDisplay[]>(this.baseApi + this.apiUrl + "/GetRevenueTimeReportPaged", {params: new HttpParams({fromObject: val})});
    }

    getRevenueServiceReport(val:any ) {
        return this.http.get<RevenueServiceReportDisplay[]>(this.baseApi + this.apiUrl + "/GetRevenueServiceReportPaged", {params: new HttpParams({fromObject: val})});
    }

    getRevenueEmployeeReport(val:any ) {
        return this.http.get<RevenueEmployeeReportDisplay[]>(this.baseApi + this.apiUrl + "/GetRevenueEmployeeReportPaged", {params: new HttpParams({fromObject: val})});
    }

    getRevenueReportDetailPaged(val:any ) {
        return this.http.get<PagedResult2<RevenueReportDetailDisplay>>(this.baseApi + this.apiUrl + "/GetRevenueReportDetailPaged", {params: new HttpParams({fromObject: val})});
    }

    getSumRevenueReport(val:any ) {
        return this.http.get(this.baseApi + this.apiUrl + "/SumRevenueReport", {params: new HttpParams({fromObject: val})});
    }
    getPrintRevenueTimeReport(val:any ) {
        return this.http.post(this.baseApi + "AccountInvoiceReport/PrintRevenueTimeReport",val,{ responseType: "text" });
        
    }
    getPrintRevenueServiceReport(val:any ) {
        return this.http.post(this.baseApi + "AccountInvoiceReport/PrintRevenueServiceReport",val,{ responseType: "text" });
        
    }
    getPrintRevenueEmployeeReport(val:any ) {
        return this.http.post(this.baseApi + "AccountInvoiceReport/PrintRevenueEmployeeReport",val,{ responseType: "text" });
        
    }

    getRevenuePartnerReport(val:any ) {
        return this.http.get<RevenueServiceReportDisplay[]>(this.baseApi + this.apiUrl + "/GetRevenuePartnerReportPaged", {params: new HttpParams({fromObject: val})});
    }

    getPrintRevenuePartnerReport(val:any ) {
        return this.http.post(this.baseApi + "AccountInvoiceReport/PrintRevenuePartnerReport",val,{ responseType: "text" });
        
    }

    getRevenueTimeReportPdf(val:any ) {
        return this.http.get(this.baseApi + this.apiUrl + "/GetRevenueTimeReportPdf", {params: new HttpParams({fromObject: val}),  responseType: 'blob'  });
    }

    getRevenueServiceReportPdf(val:any ) {
        return this.http.get(this.baseApi + this.apiUrl + "/GetRevenueServiceReportPdf", {params: new HttpParams({fromObject: val}),  responseType: 'blob'  });
    }

    getRevenueEmployeeReportPdf(val:any ) {
        return this.http.get(this.baseApi + this.apiUrl + "/GetRevenueEmployeeReportPdf", {params: new HttpParams({fromObject: val}),  responseType: 'blob'  });
    }

    getRevenuePartnerReportPdf(val:any ) {
        return this.http.get(this.baseApi + this.apiUrl + "/GetRevenuePartnerReportPdf", {params: new HttpParams({fromObject: val}),  responseType: 'blob'  });
    }
}