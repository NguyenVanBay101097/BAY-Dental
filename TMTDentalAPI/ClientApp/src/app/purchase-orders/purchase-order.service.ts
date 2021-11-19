import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedResult2 } from '../core/paged-result-2';
import { PartnerSimple } from '../partners/partner-simple';
import { ProductSimple } from '../products/product-simple';
import { UoMDisplay } from '../uoms/uom.service';

export class PurchaseOrderPaged {
    limit: number;
    offset: number;
    search: string;
    type: string;
    partnerId: string;
    dateFrom: string;
    dateTo: string;
    state: string;
}

export class PurchaseOrderBasic {
    id: string;
    partnerName: string;
    name: string;
    dateOrder: string;
    state: string;
    amountTotal: string;
    type: string;
}

export class PurchaseOrderDisplay {
    id: string;
    name: string;
    state: string;
    partnerId: string;
    partner: PartnerSimple;
    dateOrder: string;
    amountTotal: number;
    datePlanned: string;
    pickingTypeId: string;
    orderLines: PurchaseOrderLineDisplay[];
    type: string;
    amountResidual: number;
}

export class PurchaseOrderSave {
    partnerId: string;
    journalId: string;
    dateOrder: string;
    amountPayment: number;
    orderLines: PurchaseOrderLineSave[];
    type: string;
    note: string;
}

export class PurchaseOrderLineSave {
    id: string;
    name: string;
    productId: string;
    productQty: number;
    priceUnit: number;
    oldPriceUnit: number;
    priceSubtotal: number;
    state: string;
    uomFactor: number;
    productUOMId: string;
    productUOMPOId: string;
    discount: number;
}

export class PurchaseOrderLineDisplay {
    id: string;
    name: string;
    productId: string;
    product: ProductSimple;
    productQty: number;
    priceUnit: number;
    oldPriceUnit: number;
    priceSubtotal: number;
    state: string;
    uomFactor: number;
    productUOMId: string;
    productUOM: UoMDisplay;
    productUOMPO: UoMDisplay;
    productUOMPOId: string;
    discount: number;
}

@Injectable()
export class PurchaseOrderService {
    apiUrl = 'api/PurchaseOrders';
    apiPrintUrl = 'PurchaseOrder';

    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any): Observable<PagedResult2<PurchaseOrderBasic>> {
        return this.http.get<PagedResult2<PurchaseOrderBasic>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
    }

    get(id) {
        return this.http.get(this.baseApi + this.apiUrl + "/" + id);
    }

    create(val) {
        return this.http.post(this.baseApi + this.apiUrl, val);
    }

    update(id: string, val) {
        return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
    }

    defaultGet(val: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/DefaultGet', val);
    }

    getRefundByOrder(id) {
        return this.http.get(this.baseApi + this.apiUrl + "/" + id + '/GetRefundByOrder');
    }

    // getPrint(id: string) {
    //     return this.http.get(this.baseApi + this.apiPrintUrl+ '/Print' + `?id=${id}`, { responseType: 'text' });
    // }
    getPrint(id: string) {
        return this.http.get(this.baseApi + this.apiUrl + `/${id}/Print`);
    }

    buttonConfirm(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + '/ButtonConfirm', ids);
    }

    buttonCancel(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + '/ButtonCancel', ids);
    }

    unlink(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + '/Unlink', ids);
    }

    exportExcelFile(val: any) {
        return this.http.get(this.baseApi + this.apiUrl + "/ExportExcelFile", {
            responseType: "blob",
            params: val,
        });
    }
}