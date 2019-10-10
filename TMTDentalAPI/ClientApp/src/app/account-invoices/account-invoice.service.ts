import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { AccountInvoiceLineDisplay } from './account-invoice-line-display';
import { PartnerSimple } from '../partners/partner-simple';
import { UserSimple } from '../users/user-simple';
import { DotKhamBasic } from '../dot-khams/dot-khams';

export class AccountInvoiceDisplay {
    id: string;
    name: string;
    type: string;
    number: string;
    comment: string;
    state: string;
    dateOrder: string;
    partnerId: string;
    partner: PartnerSimple;
    invoiceLines: AccountInvoiceLineDisplay[];
    amountTotal: number;
    journalId: string;
    reconciled: boolean;
    residual: number;
    accountId: string;
    userId: string;
    user: UserSimple;
    companyId: string;
}

export class AccountInvoicePrint {
    companyName: string;
    companyAddress: string;
    companyPhone: string;
    companyEmail: string;
    partnerRef: string;
    partnerName: string;
    partnerAddress: string;
    parterPhone: string;
    dateInvoice: string;
    number: string;
    invoiceLines: AccountInvoiceLinePrint[];
    amountTotal: number;
}

export class AccountInvoiceLinePrint {
    productName: string;
    quantity: number;
    priceUnit: number;
    discount: number;
    priceSubtotal: number;
    sequence: number;
}

export class PaymentInfoContent {
    name: string;
    journalName: string;
    amount: number;
    date: string;
    paymentId: string;
    moveId: string;
    ref: string;
    accountPaymentId: string;
}

export class AccountInvoiceCbx {
    id: string;
    number: string;
}

export class AccountInvoiceBasic {
    id: string;
    name: string;
    type: string;
    number: string;
    state: string;
    dateInvoice: string;
    partnerId: string;
    partner: object;
    amountTotal: number;
    residual: number;
    userId: string;
    user: Object;
    comment: string;
}

export class AccountInvoicePaging {
    offset: number;
    limit: number;
    totalItems: number;
    items: AccountInvoiceBasic[];
}

export class AccountInvoicePaged {
    offset: number;
    limit: number;
    searchPartnerNamePhone: string;
    searchNumber: string;
    search: string;
    dateInvoiceFrom: string;
    dateInvoiceTo: string;
    dateOrderFrom: string;
    dateOrderTo: string;
    partnerId: string;
    type: string;
    userId: string;
    state: string;
}

@Injectable()
export class AccountInvoiceService {
    apiUrl = 'api/accountinvoices';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any): Observable<AccountInvoicePaging> {
        return this.http.get<AccountInvoicePaging>(this.baseApi + this.apiUrl, { params: val });
    }

    get(id): Observable<AccountInvoiceDisplay> {
        return this.http.get<AccountInvoiceDisplay>(this.baseApi + this.apiUrl + "/" + id);
    }

    defaultGet(val: AccountInvoiceDisplay): Observable<AccountInvoiceDisplay> {
        return this.http.post<AccountInvoiceDisplay>(this.baseApi + this.apiUrl + "/defaultget", val);
    }

    create(val: AccountInvoiceDisplay): Observable<AccountInvoiceDisplay> {
        return this.http.post<AccountInvoiceDisplay>(this.baseApi + this.apiUrl, val);
    }

    update(id: string, val: AccountInvoiceDisplay) {
        return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
    }

    invoiceOpen(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + "/invoiceopen", ids);
    }

    actionCancel(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + "/ActionCancel", ids);
    }

    actionCancelDraft(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + "/ActionCancelDraft", ids);
    }

    getPaymentInfoJson(id: string): Observable<PaymentInfoContent[]> {
        return this.http.get<PaymentInfoContent[]>(this.baseApi + this.apiUrl + `/${id}/GetPaymentInfoJson`);
    }

    getOpenPaid(search?: string): Observable<AccountInvoiceCbx[]> {
        let params = new HttpParams()
            .set('search', search);
        return this.http.get<AccountInvoiceCbx[]>(this.baseApi + this.apiUrl + '/GetOpenPaid', { params });
    }

    getDotKhamList(id: string): Observable<DotKhamBasic[]> {
        return this.http.get<DotKhamBasic[]>(this.baseApi + this.apiUrl + `/${id}/GetDotKhamList`);
    }

    getPrint(id: string): Observable<AccountInvoicePrint> {
        return this.http.get<AccountInvoicePrint>(this.baseApi + this.apiUrl + "/" + id + "/print");
    }

}