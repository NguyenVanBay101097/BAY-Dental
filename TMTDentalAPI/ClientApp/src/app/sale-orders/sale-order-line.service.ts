import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { SaleOrderLineDisplay } from './sale-order-line-display';

export class SaleOrderLineOnChangeProduct {
    productId: string;
    partnerId: string;
    pricelistId: string;
}

export class SaleOrderLineOnChangeProductResult {
    name: string;
    priceUnit: number;
}

@Injectable()
export class SaleOrderLineService {
    apiUrl = 'api/SaleOrderLines';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    onChangeProduct(val: SaleOrderLineOnChangeProduct): Observable<SaleOrderLineOnChangeProductResult> {
        return this.http.post<SaleOrderLineOnChangeProductResult>(this.baseApi + this.apiUrl + '/OnChangeProduct', val);
    }

    create(val): Observable<SaleOrderLineDisplay> {
        return this.http.post<SaleOrderLineDisplay>(this.baseApi + this.apiUrl, val);
    }
}