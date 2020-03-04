import { Injectable, Inject } from '@angular/core';
import { HttpParams, HttpClient } from '@angular/common/http';

@Injectable({
    providedIn: 'root'
})

export class FacebookOAuthService {
    apiUrl = 'api/FacebookOAuth';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    exchangeToken(access_token: string) {
        return this.http.get(this.baseApi + this.apiUrl + '/ExchangeToken', { params: new HttpParams({ fromObject: { access_token } }) });
    }
}
