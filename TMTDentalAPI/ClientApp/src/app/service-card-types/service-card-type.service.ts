import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';
import { PagedResult2 } from '../employee-categories/emp-category';


export class ServiceCardTypeObj {
    name: string;
    period: string;
    nbrPeriod: number;
    productPricelistItems: ProductPricelistItems[] = [];
}
export class ProductPricelistItems {
    categId: string;
    productId: string;
    computePrice: string;
    percentPrice: number;
    fixedAmountPrice: number;
}

export class ProductPricelistItemDisplay {
    
}

export class ServiceCardTypeBasic {
    id: string;
    name: string;
    period: string;
    nbrPeriod: number;
    productPricelistId: string[];
}

@Injectable({ providedIn: 'root' })
export class ServiceCardTypeService {
    apiUrl = 'api/ServiceCardTypes';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPreferentialCards(val: any){
        return this.http.get<PagedResult2<ServiceCardTypeBasic>>(this.baseApi + this.apiUrl, {params: new HttpParams({fromObject: val})});
    }

    getMemberCards(val: any){
        
    }

    getPaged(val: any) {
        return this.http.get(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
    }

    get(id) {
        return this.http.get<any>(this.baseApi + this.apiUrl + "/" + id);
    }

    create(val: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/Create', val);
    }

    update(id: string, val: any) {
        return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
    }

    autoComplete(search: string) {
        return this.http.get(this.baseApi + this.apiUrl + "/AutoComplete", { params: {search} });
    }
}