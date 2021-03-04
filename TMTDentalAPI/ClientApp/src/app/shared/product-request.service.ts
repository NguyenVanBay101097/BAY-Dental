import { HttpClient, HttpParams } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { ProductRequestBasic, ProductRequestDefaultGet, ProductRequestDisplay, ProductRequestSave } from "src/app/sale-orders/product-request";
import { PagedResult2 } from "../core/paged-result-2";

@Injectable({ providedIn: 'root' })
export class ProductRequestService {
    apiUrl = 'api/ProductRequests';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any): Observable<PagedResult2<ProductRequestBasic>> {
        return this.http.get<PagedResult2<ProductRequestBasic>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
    }

    get(id): Observable<ProductRequestDisplay> {
        return this.http.get<ProductRequestDisplay>(this.baseApi + this.apiUrl + "/" + id);
    }

    defaultGet(val: ProductRequestDefaultGet): Observable<ProductRequestDisplay> {
        return this.http.post<ProductRequestDisplay>(this.baseApi + this.apiUrl + '/DefaultGet', val );
    }

    create(val: ProductRequestSave): Observable<ProductRequestBasic> {
        return this.http.post<ProductRequestBasic>(this.baseApi + this.apiUrl, val);
    }

    update(id: string, val: ProductRequestSave) {
        return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
    }

    actionConfirm(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + '/ActionConfirm', ids);
    }

    actionCancel(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + '/ActionCancel', ids);
    }

    actionDone(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + '/ActionDone', ids);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
    }
}