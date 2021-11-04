import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { PagedResult2 } from '../core/paged-result-2';
import { ProductPriceListBasic } from '../price-list/price-list';
import { ProductPricelistItems } from '../service-card-types/service-card-type.service';

export class CardTypePaged {
    limit: number;
    offset: number;
    search: string;
}

export class CardTypeBasic {
    id: string;
    name: string;
}

export class CardTypeDisplay {
    id: string;
    name: string;
    basicPoint: number;
    pricelistId: string;
    color: string;
    productPricelistItems: ProductPricelistItems[] = [];
}

export class CardTypeSave {
    name: string;
    basicPoint: number;
    productPricelistItems: any;
    color: string;
}

@Injectable()
export class CardTypeService {
    apiUrl = 'api/CardTypes';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any): Observable<PagedResult2<CardTypeBasic>> {
        return this.http.get<PagedResult2<CardTypeBasic>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
    }

    get(id): Observable<any> {
        return this.http.get<any>(this.baseApi + this.apiUrl + "/" + id);
    }

    create(val: CardTypeDisplay): Observable<CardTypeDisplay> {
        return this.http.post<CardTypeDisplay>(this.baseApi + this.apiUrl, val);
    }

    createCardType(val: any): Observable<any> {
        return this.http.post<CardTypeDisplay>(this.baseApi + this.apiUrl, val);
    }

    update(id: string, val: any) {
        return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
    }
}