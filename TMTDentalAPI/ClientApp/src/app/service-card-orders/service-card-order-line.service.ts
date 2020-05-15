import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ServiceCardOrderLineService {
    apiUrl = 'api/ServiceCardOrderLines';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    onChangeProduct(data: any) {
        return this.http.post(this.baseApi + this.apiUrl + "/OnChangeProduct", data);
    }
}