import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { IRRulePaging } from './ir-rule';


@Injectable()
export class IRRuleService {
    apiUrl = 'api/IRRules';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(params: HttpParams): Observable<IRRulePaging> {
        return this.http.get<IRRulePaging>(this.baseApi + this.apiUrl, { params: params });
    }
}