import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';


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

@Injectable({ providedIn: 'root' })
export class ServiceCardTypeService {
    apiUrl = 'api/ServiceCardTypes';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPreferentialCards(val: any){
        
    }

    getMemberCards(val: any){
        
    }

    getPaged(val: any) {
        return this.http.get(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
    }

    get(id) {
        return this.http.get(this.baseApi + this.apiUrl + "/" + id);
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
}