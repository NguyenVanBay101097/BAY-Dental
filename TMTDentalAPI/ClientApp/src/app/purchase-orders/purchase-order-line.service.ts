import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { UoMDisplay } from '../uoms/uom.service';

export class PurchaseOrderLineOnChangeProduct {
    productId: string;
}

export class PurchaseOrderLineOnChangeProductResult {
    name: string;
    priceUnit: number;
    productUOMId: string;
    productUOM: UoMDisplay;
    productUOMPOId: string;
    productUOMPO: UoMDisplay;
}

@Injectable()
export class PurchaseOrderLineService {
    apiUrl = 'api/PurchaseOrderLines';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    onChangeProduct(val: PurchaseOrderLineOnChangeProduct): Observable<PurchaseOrderLineOnChangeProductResult> {
        return this.http.post<PurchaseOrderLineOnChangeProductResult>(this.baseApi + this.apiUrl + '/OnChangeProduct', val);
    }

    onChangeUOM(val: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/OnChangeUOM', val);
    }
}