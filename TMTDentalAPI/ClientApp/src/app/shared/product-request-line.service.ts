import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { ProductRequestGetLinePar } from "src/app/sale-orders/product-request";
import { ProductRequestLineDisplay } from "src/app/sale-orders/product-request-line";

@Injectable({ providedIn: 'root' })
export class ProductRequestLineService {
    apiUrl = 'api/ProductRequestLines';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getLine(val: ProductRequestGetLinePar) {
        return this.http.post<ProductRequestLineDisplay>(this.baseApi + this.apiUrl + '/GetlineAble', val);
    }
}