import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs';

export class TenantRegisterViewModel {
    name: string;
    email: string;
    phone: string;
    companyName: string;
    hostName: string;
    username: string;
    password: string;
}

export class TenantBasic {
    id: string;
    name: string;
    email: string;
    companyName: string;
    hostName: string;
}

export class TenantPaged {
    search: string;
    limit: number;
    offset: number;
}


export class TenantPaging {
    offset: number;
    limit: number;
    totalItems: number;
    items: TenantBasic[];
}

@Injectable()
export class TenantService {
    apiUrl = 'api/tenants';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any): Observable<TenantPaging> {
        return this.http.get<TenantPaging>(this.baseApi + this.apiUrl, { params: val });
    }

    register(val: TenantRegisterViewModel) {
        return this.http.post(this.baseApi + this.apiUrl + "/register", val);
    }
}