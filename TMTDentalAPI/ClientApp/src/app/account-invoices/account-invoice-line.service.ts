import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';

export class AccountInvoiceLineOnChangeProduct {
    invoiceType: string;
    productId: string;
    partnerId: string;
}

export class AccountInvoiceLineOnChangeProductResult {
    priceUnit: number;
    name: string;
    accountId: string;
    uomId: string;
}

@Injectable()
export class AccountInvoiceLineService {
    apiUrl = 'api/accountinvoicelines';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    onChangeProduct(val: AccountInvoiceLineOnChangeProduct)
        : Observable<AccountInvoiceLineOnChangeProductResult> {
        return this.http.post<AccountInvoiceLineOnChangeProductResult>(this.baseApi + this.apiUrl + "/onchangeproduct", val);
    }
}