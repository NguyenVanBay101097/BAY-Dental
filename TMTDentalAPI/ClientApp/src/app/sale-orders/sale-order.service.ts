import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { SaleOrderPaging } from './sale-order-paging';
import { SaleOrderDisplay } from './sale-order-display';
import { SaleOrderLineDisplay } from './sale-order-line-display';
import { SaleOrderLineDefaultGet } from './sale-order-line-default-get';

@Injectable()
export class SaleOrderService {
    apiUrl = 'http://localhost:50396/api/saleorders';
    constructor(private http: HttpClient) { }

    getPaging(pageIndex: number, pageSize: number)
        : Observable<SaleOrderPaging> {
        let params = new HttpParams()
            .set('pageNumber', (pageIndex + 1).toString())
            .set('pageSize', pageSize.toString());
        return this.http.get<SaleOrderPaging>(this.apiUrl, { params });
    }

    get(id): Observable<SaleOrderDisplay> {
        return this.http.get<SaleOrderDisplay>(this.apiUrl + "/" + id);
    }

    defaultGet(): Observable<SaleOrderDisplay> {
        return this.http.post<SaleOrderDisplay>(this.apiUrl + "/defaultget", {});
    }

    defaultLineGet(val: SaleOrderLineDefaultGet): Observable<SaleOrderLineDisplay> {
        return this.http.post<SaleOrderLineDisplay>(this.apiUrl + "/defaultlineget", val);
    }

    create(product: SaleOrderDisplay) {
        return this.http.post(this.apiUrl, product);
    }

    update(id: string, product: SaleOrderDisplay) {
        return this.http.put(this.apiUrl + "/" + id, product);
    }

    delete(id: string) {
        return this.http.delete(this.apiUrl + "/" + id);
    }
}