import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { Injectable, Inject } from '@angular/core';

export class PurchaseOrderLineOnChangeProduct {
    productId: string;
}

export class PurchaseOrderLineOnChangeProductResult {
    name: string;
    priceUnit: number;
    productUOMId: string;
}

@Injectable()
export class PurchaseOrderLineService {
    apiUrl = 'api/PurchaseOrderLines';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    onChangeProduct(val: PurchaseOrderLineOnChangeProduct): Observable<PurchaseOrderLineOnChangeProductResult> {
        return this.http.post<PurchaseOrderLineOnChangeProductResult>(this.baseApi + this.apiUrl + '/OnChangeProduct', val);
    }
}