import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';


export class StockMoveOnChangeProduct {
    productId: string;
}

export class StockMoveOnChangeProductResult {
    name: string;
}

@Injectable({providedIn: 'root'})
export class StockMoveService {
    apiUrl = 'api/stockmoves';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    onChangeProduct(val: StockMoveOnChangeProduct): Observable<StockMoveOnChangeProductResult> {
        return this.http.post<StockMoveOnChangeProductResult>(this.baseApi + this.apiUrl + "/onchangeproduct", val);
    }
}