import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { IRModelBasic } from '../ir-models/ir-model';

export class ResGroupFilter {
    sort: string;
    order: string;
    pageIndex: number;
    pageSize: number;
    filter: string;
}

export class ResGroupBasic {
    id: string;
    name: string;
}

export class ResGroupDisplay {
    id: string;
    name: string;
    modelAccesses: IRModelAccessDisplay[];
}

export class IRModelAccessDisplay {
    id: string;
    name: string;
    model: IRModelBasic;
    permRead: boolean;
    permWrite: boolean;
    permCreate: boolean;
    permUnlink: boolean;
    modelId: string;
}

export class ResGroupPaged {
    offset: number;
    limit: number;
    search: string;
}

export class ResGroupPaging {
    offset: number;
    limit: number;
    totalItems: number;
    items: ResGroupBasic[];
}

@Injectable()
export class ResGroupService {
    apiUrl = 'api/resgroups';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any): Observable<ResGroupPaging> {
        return this.http.get<ResGroupPaging>(this.baseApi + this.apiUrl, { params: val });
    }

    create(group: ResGroupDisplay): Observable<ResGroupDisplay> {
        return this.http.post<ResGroupDisplay>(this.baseApi + this.apiUrl, group);
    }

    update(id: string, group: ResGroupDisplay) {
        return this.http.put(this.baseApi + this.apiUrl + '/' + id, group);
    }

    get(id: string) {
        return this.http.get<ResGroupDisplay>(this.baseApi + this.apiUrl + '/' + id);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + '/' + id);
    }

    defaultGet(): Observable<ResGroupDisplay> {
        return this.http.post<ResGroupDisplay>(this.baseApi + this.apiUrl + '/DefaultGet', {});
    }

    resetSecurityData() {
        return this.http.post(this.baseApi + this.apiUrl + '/ResetSecurityData', {});
    }

    updateModels() {
        return this.http.post(this.baseApi + this.apiUrl + '/UpdateModels', {});
    }
}