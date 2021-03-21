import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';

export class AccountCommonPartnerReportItem {
    partnerId: string;
    partnerName: string;
    partnerRef: string;
    partnerPhone: string;
    begin: number;
    basicSalary: number;
    debit: number;
    credit: number;
    end: number;
    dateFrom: string;
    dateTo: string;
    resultSelection: string;
}

export class AccountCommonPartnerReportItemDetail {
    date: string;
    name: string;
    ref: string;
    moveName: string;
    begin: number;
    debit: number;
    credit: number;
    end: number;
}

export class AccountCommonPartnerReportSearch {
    fromDate: string;
    toDate: string;
    partnerId: string;
    resultSelection: string;
    display: string;
    search: string;
    companyId: string;
}

export class AccountCommonPartnerReportSearchV2 {
    constructor() {
        this.partnerIds = [];
    }
    fromDate: string;
    toDate: string;
    partnerIds: string[];
    resultSelection: string;
    companyId: string;
}

export class AccountCommonPartnerReport {
    partnerId: string;
    debit: number;
    credit: number;
    initialBalance: number;
    countSaleOrder: number;
}

export class AccountMoveBasic {
    id: string;
    name: string;
    state: string;
    partnerName: string;
    type: string;
    invoiceOrigin: string;
    amountTotal: number;
    amountTotalSigned: number;
    amountResidual: number;
    amountResidualSigned: number;
    invoiceDate: string;
    invoiceUserName: string;
}

@Injectable()
export class AccountCommonPartnerReportService {
    apiUrl = 'api/AccountCommonPartnerReports';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getSummary(val: AccountCommonPartnerReportSearch): Observable<AccountCommonPartnerReportItem[]> {
        return this.http.post<AccountCommonPartnerReportItem[]>(this.baseApi + this.apiUrl + "/GetSummary", val);
    }

    getSummaryPartner(val: AccountCommonPartnerReportSearchV2): Observable<AccountCommonPartnerReport> {
        return this.http.post<AccountCommonPartnerReport>(this.baseApi + this.apiUrl + "/GetSummaryPartner", val);
    }

    getReportSalaryEmployee(val: AccountCommonPartnerReportSearch): Observable<AccountCommonPartnerReportItem[]> {
        return this.http.post<AccountCommonPartnerReportItem[]>(this.baseApi + this.apiUrl + "/GetSalaryReportEmployee", val);
    }

    getListReportPartner(val: AccountCommonPartnerReportSearch): Observable<AccountMoveBasic[]> {
        return this.http.post<AccountMoveBasic[]>(this.baseApi + this.apiUrl + "/GetListReportPartner", val);
    }

    getReportSalaryEmployeeDetail(val: AccountCommonPartnerReportItem): Observable<AccountCommonPartnerReportItemDetail[]> {
        return this.http.post<AccountCommonPartnerReportItemDetail[]>(this.baseApi + this.apiUrl + "/ReportSalaryEmployeeDetail", val);
    }

    getDetail(val: AccountCommonPartnerReportItem): Observable<AccountCommonPartnerReportItemDetail[]> {
        return this.http.post<AccountCommonPartnerReportItemDetail[]>(this.baseApi + this.apiUrl + "/GetDetail", val);
    }
}
