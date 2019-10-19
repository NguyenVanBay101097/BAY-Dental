import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { IRModelPaging } from './ir-model';


@Injectable()
export class IRModelService {
    apiUrl = 'api/IRModels';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(params: HttpParams): Observable<IRModelPaging> {
        return this.http.get<IRModelPaging>(this.baseApi + this.apiUrl, { params: params });
    }
}