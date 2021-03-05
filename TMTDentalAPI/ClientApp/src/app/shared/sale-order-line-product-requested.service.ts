import { HttpClient, HttpParams } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { PagedResult2 } from "../core/paged-result-2";

export class SaleOrderLineProductRequestedPaged {
    offset: number;
    limit: number;
    search: string;
    saleOrderLineIds: string[];
}

export class SaleOrderLineProductRequestedBasic {
    id: string;
    saleOrderLineId: string;
    productId: string;
    requestedQuantity: number;
}

export class SaleOrderLineProductRequestedBasicCus {
    id: string;
    saleOrderLineId: string;
    productId: string;
    requestedQuantity: number;
    max: number;
}

export class SaleOrderLineProductRequestedSave {
    saleOrderLineId: string;
    productId: string;
    requestedQuantity: number;
}

export class SaleOrderLineProductRequestedDisplay {
    id: string;
    saleOrderLineId: string;
    productId: string;
    requestedQuantity: number;
}

@Injectable({ providedIn: 'root' })
export class SaleOrderLineProductRequestedService {
    apiUrl = 'api/SaleOrderLineProductRequested';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any): Observable<PagedResult2<SaleOrderLineProductRequestedBasic>> {
        return this.http.get<PagedResult2<SaleOrderLineProductRequestedBasic>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
    }
}