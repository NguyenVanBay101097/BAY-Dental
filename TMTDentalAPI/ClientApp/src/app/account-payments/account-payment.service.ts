import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedResult2 } from '../core/paged-result-2';

export class AccountPaymentBasic {
    id: string;
    partnerName: string;
    paymentDate: string;
    journalName: string;
    state: string;
    name: string;
    amount: number;
}

export class AccountPaymentSave {
    paymentDate: string;
    communication: string;
    journalId: string;
    partnerType: string;
    amount: number;
    paymentType: string;
    partnerId: string;
    hrPayslipId: string;
}

export class AccountPaymentPaged {
    offset: number;
    limit: number;
    search: string;
    phieuThuChi: boolean;
    partnerType: string;
    state: string;
    paymentDateFrom: string;
    resultSelection: string;
    paymentDateTo: string;
    saleOrderId: string;
    partnerId: string;
    companyId: string;
    paymentType: string;
    journalType: string;
}

export class AccountPaymentDisplay {
    id: string;
    partnerId: string;
    partner: Object;
    partnerType: string;
    paymentDate: string;
    journalId: string;
    journal: Object;
    state: string;
    name: string;
    paymentType: string;
    amount: number;
    communication: string;
}

export class AccountPaymentSupplierDefaultGetRequest {
    partnerId: string;
    invoiceIds: string[];
}

@Injectable({ providedIn: 'root' })
export class AccountPaymentService {
    apiUrl = 'api/AccountPayments';
    apiPrintUrl = 'AccountPayments';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any): Observable<PagedResult2<AccountPaymentBasic>> {
        return this.http.get<PagedResult2<AccountPaymentBasic>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
    }

    get(id: string): Observable<AccountPaymentDisplay> {
        return this.http.get<AccountPaymentDisplay>(this.baseApi + this.apiUrl + '/' + id);
    }

    create(val: any): Observable<AccountPaymentBasic> {
        return this.http.post<AccountPaymentBasic>(this.baseApi + this.apiUrl, val);
    }

    createMultipleAndConfirmUI(vals: any): Observable<AccountPaymentBasic[]> {
        return this.http.post<AccountPaymentBasic[]>(this.baseApi + this.apiUrl + '/CreateMultipleAndConfirmUI', vals);
    }

    update(id, val) {
        return this.http.put(this.baseApi + this.apiUrl + '/' + id, val);
    }

    actionCancel(ids: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/ActionCancel', ids);
    }

    unlink(ids: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/Unlink', ids);
    }

    defaultGet(ids: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/DefaultGet', ids);
    }

    insurancePaymentDefaultGet(ids: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/InsurancePaymentDefaultGet', ids);
    }

    saleDefaultGet(ids: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/SaleDefaultGet', ids);
    }

    thuChiDefaultGet(val: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/ThuChiDefaultGet', val);
    }

    salaryPaymentDefaultGet() {
        return this.http.get(this.baseApi + this.apiUrl + '/SalaryPaymentDefaultGet');
    }

    purchaseDefaultGet(ids: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/PurchaseDefaultGet', ids);
    }

    serviceCardOrderDefaultGet(ids: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/ServiceCardOrderDefaultGet', ids);
    }

    post(ids: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/Post', ids);
    }

    // getPrint(id: string) {
    //     return this.http.get(this.baseApi + this.apiUrl + '/' + id + '/GetPrint');
    // }

    getPrint(id: string) {
        return this.http.get(this.baseApi + this.apiPrintUrl + '/Print' + `?id=${id}`, { responseType: 'text' });
    }

    supplierDefaultGet(val: AccountPaymentSupplierDefaultGetRequest) {
        return this.http.post(this.baseApi + this.apiUrl + '/SupplierDefaultGet', val);
    }

    exportExcelFile(val: any) {
        return this.http.post(this.baseApi + this.apiUrl + "/ExportExcelFile", val, { responseType: "blob" });
    }
}