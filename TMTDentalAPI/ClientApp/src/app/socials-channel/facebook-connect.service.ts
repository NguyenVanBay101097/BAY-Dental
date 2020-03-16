import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class FacebookConnectService {
    apiUrl = 'api/FacebookConnects';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    get() {
        return this.http.get(this.baseApi + this.apiUrl);
    }

    saveFromUI(data: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/SaveFromUI', data);
    }
}