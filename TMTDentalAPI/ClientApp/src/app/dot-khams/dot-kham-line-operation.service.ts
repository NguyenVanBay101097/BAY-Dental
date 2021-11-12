import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

@Injectable()
export class DotKhamLineOperationService {
    apiUrl = 'api/dotkhamlineoperations';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    markDone(id: string) {
        return this.http.post(this.baseApi + this.apiUrl + `/${id}/MarkDone`, null);
    }

    startOperation(id: string) {
        return this.http.post(this.baseApi + this.apiUrl + `/${id}/StartOperation`, null);
    }
}