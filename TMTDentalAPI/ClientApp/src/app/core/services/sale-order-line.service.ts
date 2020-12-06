import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { SaleOrderLineDisplay } from '../../sale-orders/sale-order-line-display';

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
    Offset: number;
    limit: number;
    isQuotation: boolean;
}

@Injectable({ providedIn: 'root' })
export class SaleOrderLineService {
    apiUrl = 'api/SaleOrderLines';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    onChangeProduct(val: SaleOrderLineOnChangeProduct): Observable<SaleOrderLineOnChangeProductResult> {
        return this.http.post<SaleOrderLineOnChangeProductResult>(this.baseApi + this.apiUrl + '/OnChangeProduct', val);
    }

    get(val: any) {
        return this.http.get(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
    }

    create(val): Observable<SaleOrderLineDisplay> {
        return this.http.post<SaleOrderLineDisplay>(this.baseApi + this.apiUrl, val);
    }

    getDisplayBySaleOrder(id): Observable<SaleOrderLineDisplay[]> {
        return this.http.get<SaleOrderLineDisplay[]>(this.baseApi + this.apiUrl + '/GetDisplayBySaleOrder/' + id);
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
}