import { HttpClient, HttpParams } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";

export class TenantSaleOrderProcessUpdatePaged {
    search: string;
    limit: number;
    offset: number;
}

@Injectable({providedIn: 'root'})
export class TenantSaleOrderProcessUpdateService {
    apiUrl = 'api/TenantOldSaleOrderProcessUpdate';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any) {
        return this.http.get(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
    }

    loadAll() {
        return this.http.post(this.baseApi + this.apiUrl + '/LoadAll', {});
    }

    processAll() {
        return this.http.post(this.baseApi + this.apiUrl + '/ProcessAll', {});
    }
}