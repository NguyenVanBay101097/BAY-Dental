import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { IRModelBasic } from './ir-model';
import { PagedResult2 } from '../core/paged-result-2';

@Injectable()
export class IRModelService {
    apiUrl = 'api/IRModels';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any): Observable<PagedResult2<IRModelBasic>> {
        return this.http.get<PagedResult2<IRModelBasic>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
    }

    create(val: any) {
        return this.http.post(this.baseApi + this.apiUrl, val);
    }

    update(id: string, val: any) {
        return this.http.put(this.baseApi + this.apiUrl + '/' + id, val);
    }

    get(id: string) {
        return this.http.get(this.baseApi + this.apiUrl + '/' + id);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + '/' + id);
    }
}