import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';

export class ListPagedBirthdayCustomerRequest {
    offset: number;
    limit: number;
    search: string;
    companyId: string;
}

@Injectable({ providedIn: 'root' })
export class BirthdayCustomerService {
    apiUrl = 'api/BirthdayCustomers';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getListPaged(val: any) {
        return this.http.get(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
    }
}