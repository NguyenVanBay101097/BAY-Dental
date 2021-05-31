import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';
import { ProductSimple } from '../products/product-simple';
import { RoutingSimple } from '../routings/routing.service';
import { UserSimple } from '../users/user-simple';
import { Observable } from 'rxjs';

export class DotKhamLineBasic {
    id: string;
    name: string;
    productId: string;
    product: ProductSimple;
    userId: string;
    user: UserSimple;
    sequence: number;
    state: string;
}

export class DotKhamLinePaging {
    limit: number;
    offset: number;
    search: string;
    dateFrom: any;
    dateTo: any;
}

export class DotKhamLinePagedResult {
    limit: number;
    offset: number;
    items: DotKhamLineBasic[];
    totalItems: number;
}

export class DotKhamLineDisplay {
    id: string;
    dotKhamId: string;
    productId: string;
    product: ProductSimple;
    routingId: string;
    routing: RoutingSimple;
    userId: string;
    user: UserSimple;
    sequence: number;
    state: string;
    dateStart: string;
    dateEnd: string;
    operations: any[];
}

export class DotKhamLineChangeRouting {
    id: string;
    routingId: string;
}

@Injectable()
export class DotKhamLineService {
    apiUrl = 'api/dotkhamlines';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(page){
        return this.http.get<DotKhamLinePagedResult>(this.baseApi + this.apiUrl, {params: new HttpParams({fromObject: page})});
    }

    get(id): Observable<DotKhamLineDisplay> {
        return this.http.get<DotKhamLineDisplay>(this.baseApi + this.apiUrl + "/" + id);
    }

    create(val: DotKhamLineDisplay): Observable<DotKhamLineDisplay> {
        return this.http.post<DotKhamLineDisplay>(this.baseApi + this.apiUrl, val);
    }

    update(id: string, val: DotKhamLineDisplay) {
        return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
    }

    markDone(id: string) {
        return this.http.post(this.baseApi + this.apiUrl + `/${id}/MarkDone`, null);
    }

    changeRouting(val: DotKhamLineChangeRouting) {
        return this.http.post(this.baseApi + this.apiUrl + `/ChangeRouting`, val);
    }

    // createDKLineAlongInvoice(ids: string[]) {
    //     return this.http.post(this.baseApi + this.apiUrl + '/CreateDKLineSteps', ids);
    // }
}