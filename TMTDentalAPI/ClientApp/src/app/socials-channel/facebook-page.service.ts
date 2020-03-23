import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class FacebookPageService {
    apiUrl = 'api/FacebookPages';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any) {
        return this.http.get(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
    }

    selectPage(id: string) {
        return this.http.get(this.baseApi + this.apiUrl + '/' + id);
    }

    getSwitchPage() {
        return this.http.get(this.baseApi + this.apiUrl + '/GetSwitchPage');
    }
}