import { SaleOrderLineBasic } from './../../partners/partner.service';
import { ToothBasic } from './../../teeth/tooth.service';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { SaleOrderLineDisplay } from '../../sale-orders/sale-order-line-display';
import { PagedResult2 } from '../paged-result-2';
import { ProductSimple } from 'src/app/products/product-simple';
import { SaleOrderBasic } from 'src/app/sale-orders/sale-order-basic';

export class SaleOrderLineOnChangeProduct {
    productId: string;
    partnerId: string;
    pricelistId: string;
}

export class SaleOrderLineOnChangeProductResult {
    name: string;
    priceUnit: number;
}

export class SaleOrderLinesPaged {
    offset: number;
    limit: number;
    dateOrderFrom: string;
    dateOrderTo: string;
    OrderId: string;
    isQuotation: boolean;
    isLabo: boolean;
    search: string;
    laboStatus: string;
    laboState: string;
}

export class SaleOrderLinesLaboPaged {
    offset: number;
    limit: number;
    search: string;
    hasAnyLabo: any;
    laboState: string;
    companyId: string;
}

export class SaleOrderLineViewModel {
    id: string;
    name: string;
    state: string;
    orderId: string;
    order: SaleOrderBasic;
    productId: string;
    product: ProductSimple;
    diagnostic: string;
    teeth: ToothBasic[];
    steps: any[]
}

export class ProductBomForSaleOrderLine {
    id: string;
    materialProductName: string;
    materialProductId: string;
    producUOMName: string;
    quantity: number;
    sequence: number;
    requestedQuantity: number;
}

export class SaleOrderLineForProductRequest {
    id: string;
    name: string;
    boms: ProductBomForSaleOrderLine[];
}

export class SmsCareAfterOrderPaged {
    offset: number;
    limit: number;
    search: string;
    dateFrom: string;
    dateTo: string;
    productId: string;
    companyId: string;
}

export class SaleOrderLineHistoryReq {
    partnerId: string;
    companyId: string;
}

export class SaleOrderLinePagging {
    limit: number;
    offset: number;
    totalItems: number;
    items: SaleOrderLineBasic[];
}

@Injectable({ providedIn: 'root' })
export class SaleOrderLineService {
    apiUrl = 'api/SaleOrderLines';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    onChangeProduct(val: SaleOrderLineOnChangeProduct): Observable<SaleOrderLineOnChangeProductResult> {
        return this.http.post<SaleOrderLineOnChangeProductResult>(this.baseApi + this.apiUrl + '/OnChangeProduct', val);
    }

    getPaged(val): Observable<PagedResult2<SaleOrderLinePagging>> {
        return this.http.get<PagedResult2<SaleOrderLinePagging>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
    }

    create(val): Observable<SaleOrderLineDisplay> {
        return this.http.post<SaleOrderLineDisplay>(this.baseApi + this.apiUrl, val);
    }

    getDisplayBySaleOrder(id): Observable<SaleOrderLineDisplay[]> {
        return this.http.get<SaleOrderLineDisplay[]>(this.baseApi + this.apiUrl + '/GetDisplayBySaleOrder/' + id);
    }

    getGrid(val: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/Grid', val);
    }

    cancelOrderLine(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + '/CancelSaleOrderLine', ids);
    }

    getLaboOrders(id: string) {
        return this.http.get(this.baseApi + this.apiUrl + '/' + id + '/GetLaboOrders');
    }

    getTeeth(id: string) {
        return this.http.get(this.baseApi + this.apiUrl + '/' + id + '/GetTeeth');
    }

    getListLineIsLabo(val: any): Observable<PagedResult2<any>> {
        return this.http.get<PagedResult2<any>>(this.baseApi + this.apiUrl + '/GetListLineIsLabo', { params: new HttpParams({ fromObject: val }) });
    }

    applyDiscountOnOrderLine(val) {
        return this.http.post(this.baseApi + this.apiUrl + '/ApplyDiscountOnOrderLine', val);
    }

    applyPromotion(val) {
        return this.http.post(this.baseApi + this.apiUrl + '/ApplyPromotion', val);
    }

    patchIsActive(id, active) {
        return this.http.patch(this.baseApi + this.apiUrl + '/' + id + '/PatchIsActive', { active: active })
    }

    applyPromotionUsageCode(val) {
        return this.http.post(this.baseApi + this.apiUrl + '/ApplyPromotionUsageCode', val);
    }

    getSmsCareAfterOrderManual(val: any): Observable<PagedResult2<any>> {
        return this.http.get<PagedResult2<any>>(this.baseApi + this.apiUrl + '/GetSmsCareAfterOrderManual', { params: new HttpParams({ fromObject: val }) });
    }

    getProductSmsCareAfterOrder(val: any): Observable<ProductSimple[]> {
        return this.http.get<ProductSimple[]>(this.baseApi + this.apiUrl + '/GetProductSmsCareAfterOrder',
            {
                params: new HttpParams({ fromObject: val })
            })
    }

    getHistories(val: any) {
        return this.http.get(this.baseApi + this.apiUrl + '/GetHistory', { params: new HttpParams({ fromObject: val }) });
    }

    update(id, val) {
        return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
    }

    remove(id) {
        return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
    }

    getSaleReportExportExcel(val) {
        return this.http.get(this.baseApi + this.apiUrl + '/GetSaleReportExportExcel', { params: new HttpParams({ fromObject: val }), responseType: 'blob' });
    }

    updateState(id, state) {
        return this.http.put(this.baseApi + this.apiUrl + '/' + id + '/UpdateState?State=' + state, null);
    }

    actionDone(ids) {
        return this.http.post(this.baseApi + this.apiUrl + '/ActionDone', ids);
    }

    actionCancel(ids) {
        return this.http.post(this.baseApi + this.apiUrl + '/ActionCancel', ids);
    }

    actionUnlock(ids) {
        return this.http.post(this.baseApi + this.apiUrl + '/ActionUnlock', ids);
    }

    SaleReportPrint(val) {
        return this.http.get(this.baseApi + 'SaleOrderLine/SaleReportPrint', { params: new HttpParams({ fromObject: val }), responseType: 'text' });
    }

    getSaleReportExportPdf(val) {
        return this.http.get(this.baseApi + this.apiUrl + '/GetSaleReportExportPdf', { params: new HttpParams({ fromObject: val }), responseType: 'blob' });
    }

    updateSteps(val) {
        return this.http.post(this.baseApi + this.apiUrl + '/UpdateSteps', val);
    }

    getTeethList(id) {
        return this.http.get(this.baseApi + this.apiUrl + '/' + id + '/GetTeethList');
    }

    getListServiceCardCardApplyable(id) {
        return this.http.get(this.baseApi + this.apiUrl + '/' + id + '/GetListServiceCardCardApplyable');
    }

    getListCardCardApplyable(id) {
        return this.http.get(this.baseApi + this.apiUrl + '/' + id + '/GetListCardCardApplyable');
    }

    applyServiceCardCard(val) {
        return this.http.post(this.baseApi + this.apiUrl + '/ApplyServiceCardCard', val);
    }

    applyCardCard(val) {
        return this.http.post(this.baseApi + this.apiUrl + '/ApplyCardCard', val);
    }

    debtPayment(id) {
        return this.http.post(this.baseApi + this.apiUrl + "/" + id + '/DebtPayment', null);
    }
}