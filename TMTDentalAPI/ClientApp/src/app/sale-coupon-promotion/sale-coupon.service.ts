import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { PagedResult2 } from '../core/paged-result-2';

export class SaleCouponPaged {
    limit: number;
    offset: number;
    search: string;
    programId: string;
}

export class SaleCouponBasic {
    id: string;
    code: string;
    programName: string;
    state: string;
    dateExpired: string;
    orderName: string;
    partnerName: string;
}

export class SaleCouponDisplay {
    id: string;
    code: string;
    programId: string;
    programName: string;
    state: string;
    dateExpired: string;
    orderId: string;
    orderName: string;
    partnerId: string;
    partnerName: string;
    saleOrderId: string;
    saleOrderName: string;
}

@Injectable()
export class SaleCouponService {
    apiUrl = 'api/SaleCoupons';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any): Observable<PagedResult2<SaleCouponBasic>> {
        return this.http.get<PagedResult2<SaleCouponBasic>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
    }

    get(id): Observable<SaleCouponDisplay> {
        return this.http.get<SaleCouponDisplay>(this.baseApi + this.apiUrl + "/" + id);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
    }
}