import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { PagedResult2 } from '../core/paged-result-2';
import { ProductPriceListBasic } from '../price-list/price-list';

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
    pricelist: ProductPriceListBasic;
}

@Injectable()
export class CardTypeService {
    apiUrl = 'api/CardTypes';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any): Observable<PagedResult2<CardTypeBasic>> {
        return this.http.get<PagedResult2<CardTypeBasic>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
    }

    get(id): Observable<CardTypeDisplay> {
        return this.http.get<CardTypeDisplay>(this.baseApi + this.apiUrl + "/" + id);
    }

    create(val: CardTypeDisplay): Observable<CardTypeDisplay> {
        return this.http.post<CardTypeDisplay>(this.baseApi + this.apiUrl, val);
    }

    update(id: string, val: CardTypeDisplay) {
        return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
    }
}