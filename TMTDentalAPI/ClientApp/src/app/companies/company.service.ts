import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';

export class CompanyPaged {
    search: string;
    limit: number;
    offset: number;
}

export class CompanyBasic {
    id: string;
    name: string;
}

export class CompanySimple {
    id: string;
    name: string;
}

export class CompanyDisplay {
    id: string;
    name: string;
    email: string;
    phone: string;
    city: { name: string, code: string };
    district: { name: string, code: string };
    ward: { name: string, code: string };
    street: string;
    logo: string;
}

export class CompanyPaging {
    offset: number;
    limit: number;
    totalItems: number;
    items: CompanyBasic[];
}


@Injectable({ providedIn: 'root' })
export class CompanyService {
    apiUrl = 'api/companies';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any): Observable<CompanyPaging> {
        return this.http.get<CompanyPaging>(this.baseApi + this.apiUrl, { params: val });
    }

    get(id: string): Observable<CompanyDisplay> {
        return this.http.get<CompanyDisplay>(this.baseApi + this.apiUrl + "/" + id);
    }

    create(company: CompanyDisplay) {
        return this.http.post(this.baseApi + this.apiUrl, company);
    }

    update(id: string, company: CompanyDisplay) {
        return this.http.put(this.baseApi + this.apiUrl + "/" + id, company);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
    }
}