import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { SaleOrderLineDisplay } from './sale-order-line-display';

@Injectable()
export class SaleOrderLineService {
    apiUrl = 'api/SaleOrderLines';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    onChangeProduct(val: SaleOrderLineDisplay): Observable<SaleOrderLineDisplay> {
        return this.http.post<SaleOrderLineDisplay>(this.baseApi + this.apiUrl + '/OnChangeProduct', val);
    }
}