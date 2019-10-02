import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';


export class StockMoveOnChangeProduct {
    productId: string;
}

export class StockMoveOnChangeProductResult {
    name: string;
}

@Injectable()
export class StockMoveService {
    apiUrl = 'api/stockmoves';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    onChangeProduct(val: StockMoveOnChangeProduct): Observable<StockMoveOnChangeProductResult> {
        return this.http.post<StockMoveOnChangeProductResult>(this.baseApi + this.apiUrl + "/onchangeproduct", val);
    }
}