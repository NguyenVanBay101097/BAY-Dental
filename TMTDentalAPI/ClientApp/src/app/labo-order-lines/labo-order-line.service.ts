import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { PartnerSimple } from '../partners/partner-simple';
import { ProductSimple } from '../products/product-simple';

export class LaboOrderLineBasic {
    id: string;
    customerId: string;
    customer: PartnerSimple;
    productId: string;
    product: ProductSimple;
    supplierId: string;
    supplier: PartnerSimple;
    quantity: number;
    sentDate: string;
    receivedDate: string;
}

export class LaboOrderLineDisplay {
    id: string;
    name: string;
    customerId: string;
    customer: PartnerSimple;
    productId: string;
    product: ProductSimple;
    supplierId: string;
    supplier: PartnerSimple;
    color: string;
    quantity: number;
    priceUnit: number;
    priceSubtotal: number;
    warrantyCode: string;
    warrantyPeriod: string;
    companyId: string;
    note: string;
    invoiceId: string;
    sentDate: string;
    receivedDate: string;
}

export class LaboOrderLinePaged {
    offset: number;
    limit: number;
    searchCustomer: string;
    searchSupplier: string;
    searchProduct: string;
    sentDateFrom: string;
    sentDateTo: string;
    receivedDateFrom: string;
    receivedDateTo: string;
}

export class LaboOrderLinePaging {
    offset: number;
    limit: number;
    totalItems: number;
    items: LaboOrderLineBasic[];
}

export class LaboOrderLineDefaultGet {
    invoiceId: string;
    dotKhamId: string;
}

export class LaboOrderLineOnChangeProduct {
    productId: string;
}

export class LaboOrderLineOnChangeProductResult {
    priceUnit: number;
}

@Injectable()
export class LaboOrderLineService {
    apiUrl = 'api/laboorderlines';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any): Observable<LaboOrderLinePaging> {
        return this.http.get<LaboOrderLinePaging>(this.baseApi + this.apiUrl, { params: val });
    }

    create(line: LaboOrderLineDisplay) {
        return this.http.post(this.baseApi + this.apiUrl, line);
    }

    update(id: string, line: LaboOrderLineDisplay) {
        return this.http.put(this.baseApi + this.apiUrl + '/' + id, line);
    }

    get(id: string): Observable<LaboOrderLineDisplay> {
        return this.http.get<LaboOrderLineDisplay>(this.baseApi + this.apiUrl + '/' + id);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + '/' + id);
    }

    defaultGet(val: LaboOrderLineDefaultGet): Observable<LaboOrderLineDisplay> {
        return this.http.post<LaboOrderLineDisplay>(this.baseApi + this.apiUrl + '/defaultget', val);
    }

    autocomplete(val: LaboOrderLinePaged): Observable<LaboOrderLineBasic[]> {
        return this.http.post<LaboOrderLineBasic[]>(this.baseApi + this.apiUrl + '/autocomplete', val);
    }

    onChangeProduct(val: LaboOrderLineOnChangeProduct): Observable<LaboOrderLineOnChangeProductResult> {
        return this.http.post<LaboOrderLineOnChangeProductResult>(this.baseApi + this.apiUrl + '/onchangeproduct', val);
    }
}