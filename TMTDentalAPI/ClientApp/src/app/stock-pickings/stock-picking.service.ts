import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { ProductSimple } from '../products/product-simple';
import { PartnerSimple } from '../partners/partner-simple';

export class StockPickingDisplay {
    id: string;
    partnerId: string;
    partner: PartnerSimple;
    pickingTypeId: string;
    note: string;
    state: string;
    date: string;
    name: string;
    moveLines: StockMoveDisplay[];
    companyId: string;
    locationId: string;
    locationDestId: string;
}

export class StockPickingBasic {
    id: string;
    partnerId: string;
    partner: PartnerSimple;
    state: string;
    date: string;
    name: string;
}

export class StockPickingPaging {
    offset: number;
    limit: number;
    totalItems: number;
    items: StockPickingBasic[];
}

export class StockPickingPaged {
    offset: number;
    limit: number;
    pickingTypeId: string;
    search: string;
    type: string;
    dateFrom: string;
    dateTo: string;
}

export class StockMoveDisplay {
    id: string;
    name: string;
    productId: string;
    product: ProductSimple;
    sequence: number;
    productUOMQty: number;
}

export class StockPickingDefaultGet {
    defaultPickingTypeId: string;
}

@Injectable()
export class StockPickingService {
    apiUrl = 'api/stockpickings';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any): Observable<StockPickingPaging> {
        return this.http.get<StockPickingPaging>(this.baseApi + this.apiUrl, { params: val });
    }

    get(id): Observable<StockPickingDisplay> {
        return this.http.get<StockPickingDisplay>(this.baseApi + this.apiUrl + "/" + id);
    }

    defaultGet(val: StockPickingDefaultGet): Observable<StockPickingDisplay> {
        return this.http.post<StockPickingDisplay>(this.baseApi + this.apiUrl + "/defaultget", val);
    }

    defaultGetOutgoing(): Observable<StockPickingDisplay> {
        return this.http.get<StockPickingDisplay>(this.baseApi + this.apiUrl + "/DefaultGetOutgoing");
    }

    create(val: StockPickingDisplay): Observable<StockPickingDisplay> {
        return this.http.post<StockPickingDisplay>(this.baseApi + this.apiUrl, val);
    }

    update(id: string, val: StockPickingDisplay) {
        return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
    }

    actionDone(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + "/actiondone", ids);
    }
}