import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { PagedResult2 } from '../core/paged-result-2';
import { SaleOrderBasic } from './sale-order-basic';
import { SaleOrderDisplay } from './sale-order-display';
import { DotKhamBasic, DotKhamDisplay } from '../dot-khams/dot-khams';
import { AccountRegisterPaymentDefaultGet, AccountRegisterPaymentDisplay } from '../account-payments/account-register-payment.service';
import { AccountPaymentPaged, AccountPaymentBasic } from '../account-payments/account-payment.service';
import { PaymentInfoContent } from '../account-invoices/account-invoice.service';
import { LaboOrderBasic, LaboOrderDisplay } from '../labo-orders/labo-order.service';
import { SaleOrderLineBasic } from '../partners/partner.service';
import { SaleOrderLineDisplay } from './sale-order-line-display';

export class SaleOrderPaged {
    limit: number;
    offset: number;
    search: string;
    partnerId: string;
    dateOrderFrom: string;
    dateOrderTo: string;
    state: string;
    isQuotation: boolean;
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

export class DiscountDefault{
    saleOrderId: string;
    discountType: string;
    discountPercent: string;
    discountFixed: string;

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

    defaultGet(p?: any): Observable<SaleOrderDisplay> {
        return this.http.get<SaleOrderDisplay>(this.baseApi + this.apiUrl + '/DefaultGet', { params: new HttpParams({ fromObject: p || {} }) });
    }

    onChangePartner(val: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/OnChangePartner', val);
    }

    actionConfirm(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + '/ActionConfirm', ids);
    }

    actionConvertToOrder(id: string) {
        return this.http.post(this.baseApi + this.apiUrl + "/" + id + '/ActionConvertToOrder', {});
    }

    checkPromotion(id: string) {
        return this.http.get(this.baseApi + this.apiUrl + "/" + id + '/CheckPromotion');
    }

    actionInvoiceCreateV2(id: string) {
        return this.http.post(this.baseApi + this.apiUrl + "/" + id + '/ActionInvoiceCreateV2', {});
    }

    actionCancel(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + '/ActionCancel', ids);
    }

    actionDone(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + '/ActionDone', ids);
    }

    applyCoupon(data: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/ApplyCoupon', data);
    }

    applyServiceCards(data: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/ApplyServiceCards', data);
    }

    applyPromotion(id: string) {
        return this.http.post(this.baseApi + this.apiUrl + '/' + id + '/ApplyPromotion', {});
    }

    applyDiscountDefault(data: DiscountDefault) {
        return this.http.post(this.baseApi + this.apiUrl + '/ApplyDiscountDefault', data);
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

    getPayments(id): Observable<PaymentInfoContent[]> {
        return this.http.get<PaymentInfoContent[]>(this.baseApi + this.apiUrl + `/${id}/GetPayments`);
    }

    getInvoices(id) {
        return this.http.get(this.baseApi + this.apiUrl + `/${id}/GetInvoices`);
    }

    getServiceBySaleOrderId(id): Observable<SaleOrderLineDisplay[]> {
        return this.http.get<SaleOrderLineDisplay[]>(this.baseApi + this.apiUrl + '/' + id + '/GetServiceBySaleOrderId');
    }

    getTreatmentBySaleOrderId(id): Observable<DotKhamDisplay[]> {
        return this.http.get<DotKhamDisplay[]>(this.baseApi + this.apiUrl + '/' + id + '/GetTreatmentBySaleOrderId');
    }

    getLaboBySaleOrderId(id): Observable<LaboOrderDisplay[]> {
        return this.http.get<LaboOrderDisplay[]>(this.baseApi + this.apiUrl + '/' + id + '/GetLaboBySaleOrderId');
    }
}