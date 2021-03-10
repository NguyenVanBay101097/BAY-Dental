import { HttpClient, HttpParams } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { ProductRequestGetLinePar } from "src/app/sale-orders/product-request";
import { ProductRequestLineDisplay } from "src/app/sale-orders/product-request-line";
import { PagedResult2 } from "../core/paged-result-2";

@Injectable({ providedIn: 'root' })
export class ProductRequestLineService {
    apiUrl = 'api/ProductRequestLines';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getLine(val: ProductRequestGetLinePar) {
        return this.http.post<ProductRequestLineDisplay>(this.baseApi + this.apiUrl + '/GetlineAble', val);
    }
}