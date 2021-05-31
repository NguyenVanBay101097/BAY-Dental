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
    dateCreatedFrom: string;
    dateCreatedTo: string;
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

    get(id: string) {
        return this.http.get(this.baseApi + this.apiUrl + '/' + id);
    }

    register(val: TenantRegisterViewModel) {
        return this.http.post(this.baseApi + this.apiUrl + "/register", val);
    }

    updateDateExpired(val: any) {
        return this.http.post(this.baseApi + this.apiUrl + "/UpdateDateExpired", val);
    }

    extendExpired(val: any) {
        return this.http.post(this.baseApi + this.apiUrl + "/ExtendExpired", val);
    }

    updateInfo(id: string, val: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/' + id + '/UpdateInfo', val);
    }

    exportExcel(paged) {
        return this.http.post(
            this.baseApi + this.apiUrl + "/ExportExcel", paged,
            { responseType: "blob" }
        );
    }
}