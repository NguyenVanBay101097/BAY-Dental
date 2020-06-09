import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { UserSimple } from './user-simple';
import { CompanyBasic } from '../companies/company.service';
import { ResGroupBasic } from '../res-groups/res-group.service';

export class UserBasic {
    id: string;
    name: string;
    userName: string;
}

export class UserDisplay {
    id: string;
    name: string;
    userName: string;
    passWord: string;
    email: string;
    avatar: string;
    company: CompanyBasic;
    companies: CompanyBasic[];
    groups: ResGroupBasic[];
}

export class UserPaging {
    pageNumber: number;
    pageSize: number;
    totalPages: number;
    totalItems: number;
    items: UserBasic[];
}

export class UserPaged {
    offset: number;
    limit: number;
    searchNameUserName: string;
    search: string;
}

export class UserChangeCurrentCompanyVM {
    currentCompany: CompanyBasic;
    companies: CompanyBasic[];
}

@Injectable()
export class UserService {
    apiUrl = 'api/applicationusers';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    autocomplete(filter: string)
        : Observable<UserSimple[]> {
        let params = new HttpParams()
            .set('filter', filter);
        return this.http.get<UserSimple[]>(this.baseApi + this.apiUrl + "/autocomplete", { params });
    }

    getPaged(val: any): Observable<UserPaging> {
        return this.http.get<UserPaging>(this.baseApi + this.apiUrl, { params: val });
    }

    get(id): Observable<UserDisplay> {
        return this.http.get<UserDisplay>(this.baseApi + this.apiUrl + "/" + id);
    }

    defaultGet(): Observable<UserDisplay> {
        return this.http.post<UserDisplay>(this.baseApi + this.apiUrl + "/defaultget", null);
    }

    create(val: UserDisplay): Observable<UserDisplay> {
        return this.http.post<UserDisplay>(this.baseApi + this.apiUrl, val);
    }

    update(id: string, val: UserDisplay) {
        return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
    }

    autocompleteSimple(val: UserPaged): Observable<UserSimple[]> {
        return this.http.post<UserSimple[]>(this.baseApi + this.apiUrl + "/autocompleteSimple", val);
    }

    getChangeCurrentCompany(): Observable<UserChangeCurrentCompanyVM> {
        return this.http.get<UserChangeCurrentCompanyVM>(this.baseApi + this.apiUrl + "/GetChangeCurrentCompany");
    }

    switchCompany(data: any) {
        return this.http.post(this.baseApi + this.apiUrl + "/SwitchCompany", data);
    }

    actionImport(val) {
        return this.http.post(this.baseApi + this.apiUrl + '/ActionImport', val);
    }
}