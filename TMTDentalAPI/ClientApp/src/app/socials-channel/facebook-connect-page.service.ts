import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class FacebookConnectPageService {
    apiUrl = 'api/FacebookConnectPages';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    addConnect(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + '/AddConnect', ids);
    }

    removeConnect(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + '/RemoveConnect', ids);
    }
}