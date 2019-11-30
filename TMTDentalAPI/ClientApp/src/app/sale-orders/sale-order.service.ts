import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { PagedResult2 } from '../core/paged-result-2';
import { SaleOrderBasic } from './sale-order-basic';
import { SaleOrderDisplay } from './sale-order-display';
import { DotKhamBasic } from '../dot-khams/dot-khams';
import { AccountRegisterPaymentDefaultGet, AccountRegisterPaymentDisplay } from '../account-payments/account-register-payment.service';
import { AccountPaymentPaged, AccountPaymentBasic } from '../account-payments/account-payment.service';
import { PaymentInfoContent } from '../account-invoices/account-invoice.service';

export class SaleOrderPaged {
    limit: number;
    offset: number;
    search: string;
    partnerId: string;
    dateOrderFrom: string;
    dateOrderTo: string;
    state: string;
}

export class AccountPaymentFilter {
    saleOrderId: string;
    partnerType: string;
    journalId: string;
    state: string;
    dateFrom: string;
    dateTo: string;
    partnerId: string;
}

@Injectable()
export class SaleOrderService {
    apiUrl = 'api/SaleOrders';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any): Observable<PagedResult2<SaleOrderBasic>> {
        return this.http.get<PagedResult2<SaleOrderBasic>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
    }

    get(id): Observable<SaleOrderDisplay> {
        return this.http.get<SaleOrderDisplay>(this.baseApi + this.apiUrl + "/" + id);
    }

    create(val: SaleOrderDisplay): Observable<SaleOrderDisplay> {
        return this.http.post<SaleOrderDisplay>(this.baseApi + this.apiUrl, val);
    }

    update(id: string, val: SaleOrderDisplay) {
        return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
    }


    getPrint(id: string) {
        return this.http.get(this.baseApi + this.apiUrl + "/" + id + '/GetPrint');
    }

    defaultGet(): Observable<SaleOrderDisplay> {
        return this.http.get<SaleOrderDisplay>(this.baseApi + this.apiUrl + '/DefaultGet');
    }

    onChangePartner(val: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/OnChangePartner', val);
    }

    actionConfirm(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + '/ActionConfirm', ids);
    }

    actionCancel(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + '/ActionCancel', ids);
    }

    actionDone(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + '/ActionDone', ids);
    }

    unlink(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + '/Unlink', ids);
    }

    getDotKhamList(id: string): Observable<DotKhamBasic[]> {
        return this.http.get<DotKhamBasic[]>(this.baseApi + this.apiUrl + `/${id}/GetDotKhamList`);
    }

    getDefaultRegisterPayment(id: string) {
        return this.http.get(this.baseApi + 'api/Partners/' + id + '/GetDefaultRegisterPayment');
    }

    defaultGetInvoice(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + '/DefaultGetInvoice', ids)
    }

    defaultValGet(val: AccountRegisterPaymentDefaultGet): Observable<AccountRegisterPaymentDisplay> {
        return this.http.post<AccountRegisterPaymentDisplay>(this.baseApi + "api/accountregisterpayments/defaultget", val);
    }

    defaultOrderGet(val: AccountRegisterPaymentDefaultGet): Observable<AccountRegisterPaymentDisplay> {
        return this.http.post<AccountRegisterPaymentDisplay>(this.baseApi + "api/accountregisterpayments/OrderDefaultGet", val);
    }

    getPaymentBasicList(val): Observable<AccountPaymentBasic[]> {
        return this.http.get<AccountPaymentBasic[]>(this.baseApi + "api/AccountPayments/GetPaymentBasicList", { params: val });
    }

    getAccountPaymentReconcicles(id): Observable<PaymentInfoContent[]> {
        return this.http.get<PaymentInfoContent[]>(this.baseApi + `api/AccountInvoices/${id}/GetSaleOrderPaymentInfoJson`);
    }
}