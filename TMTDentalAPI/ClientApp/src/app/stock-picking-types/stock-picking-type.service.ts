import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';

export class StockPickingTypePaged {
    search: string;
    limit: number;
    offset: number;
}

export class StockPickingTypeBasic {
    id: string;
    name: string;
    code: string;
}

export class StockPickingTypePaging {
    offset: number;
    limit: number;
    totalItems: number;
    items: StockPickingTypeBasic[];
}

@Injectable()
export class StockPickingTypeService {
    apiUrl = 'api/stockpickingtypes';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any): Observable<StockPickingTypePaging> {
        return this.http.get<StockPickingTypePaging>(this.baseApi + this.apiUrl, { params: val });
    }

    getBasic(id: string): Observable<StockPickingTypeBasic> {
        return this.http.get<StockPickingTypeBasic>(this.baseApi + this.apiUrl + "/" + id + '/GetBasic', {});
    }
}