import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { PagedResult2 } from '../core/paged-result-2';
import { PartnerSimple } from '../partners/partner-simple';
import { DotKhamBasic } from '../dot-khams/dot-khams';
import { ProductSimple } from '../products/product-simple';

export class PurchaseOrderPaged {
    limit: number;
    offset: number;
    search: string;
    type: string;
    partnerId: string;
    dateOrderFrom: string;
    dateOrderTo: string;
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
    orderLines: PurchaseOrderLineDisplay[];
    type: string;
}

export class PurchaseOrderLineDisplay {
    id: string;
    name: string;
    productId: string;
    product: ProductSimple;
    productQty: number;
    priceUnit: number;
    priceSubtotal: number;
    state: string;
    productUOMId: string;
    discount: number;
}

@Injectable()
export class PurchaseOrderService {
    apiUrl = 'api/PurchaseOrders';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any): Observable<PagedResult2<PurchaseOrderBasic>> {
        return this.http.get<PagedResult2<PurchaseOrderBasic>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
    }

    get(id): Observable<PurchaseOrderDisplay> {
        return this.http.get<PurchaseOrderDisplay>(this.baseApi + this.apiUrl + "/" + id);
    }

    create(val: PurchaseOrderDisplay): Observable<PurchaseOrderDisplay> {
        return this.http.post<PurchaseOrderDisplay>(this.baseApi + this.apiUrl, val);
    }

    update(id: string, val: PurchaseOrderDisplay) {
        return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
    }

    defaultGet(val: any): Observable<PurchaseOrderDisplay> {
        return this.http.post<PurchaseOrderDisplay>(this.baseApi + this.apiUrl + '/DefaultGet', val);
    }

    getPrint(id) {
        return this.http.get(this.baseApi + this.apiUrl + "/" + id + '/GetPrint');
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
}