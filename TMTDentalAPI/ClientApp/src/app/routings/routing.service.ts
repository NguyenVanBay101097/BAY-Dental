import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { ProductSimple } from '../products/product-simple';

export class RoutingDisplay {
    id: string;
    name: string;
    product: ProductSimple;
    productId: string;
    lines: RoutingLineDisplay[];
}

export class RoutingLineDisplay {
    id: string;
    product: ProductSimple;
    productId: string;
    sequence: number;
    note: string;
}

export class RoutingSimple {
    id: string;
    name: string;
}

export class RoutingBasic {
    id: string;
    name: string;
    product: object;
    productId: string;
}

export class RoutingPaging {
    offset: number;
    limit: number;
    totalItems: number;
    items: RoutingBasic[];
}

export class RoutingPaged {
    offset: number;
    limit: number;
    search: string;
    productId: string;
}

@Injectable()
export class RoutingService {
    apiUrl = 'api/routings';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any): Observable<RoutingPaging> {
        return this.http.get<RoutingPaging>(this.baseApi + this.apiUrl, { params: val });
    }

    get(id): Observable<RoutingDisplay> {
        return this.http.get<RoutingDisplay>(this.baseApi + this.apiUrl + "/" + id);
    }

    defaultGet(val: RoutingDisplay): Observable<RoutingDisplay> {
        return this.http.post<RoutingDisplay>(this.baseApi + this.apiUrl + "/defaultget", val);
    }

    create(val: RoutingDisplay): Observable<RoutingDisplay> {
        return this.http.post<RoutingDisplay>(this.baseApi + this.apiUrl, val);
    }

    update(id: string, val: RoutingDisplay) {
        return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
    }

    autocompleteSimple(val: RoutingPaged): Observable<RoutingSimple[]> {
        return this.http.post<RoutingSimple[]>(this.baseApi + this.apiUrl + '/AutocompleteSimple', val);
    }
}