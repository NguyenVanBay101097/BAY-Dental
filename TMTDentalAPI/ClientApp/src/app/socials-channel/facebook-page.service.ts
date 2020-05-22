import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class FacebookPageService {
    apiUrl = 'api/FacebookPages';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any) {
        return this.http.get(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
    }

    get(id: string) {
        return this.http.get(this.baseApi + this.apiUrl + '/' + id);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
    }

    selectPage(id: string) {
        return this.http.post(this.baseApi + this.apiUrl + '/' + id + '/SelectPage', {});
    }

    createFacebookUser() {
        return this.http.post(this.baseApi + this.apiUrl + '/CreateFacebookUser', {});
    }

    getSwitchPage() {
        return this.http.get(this.baseApi + this.apiUrl + '/GetSwitchPage');
    }

    syncUsers(ids: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/SyncUsers', ids);
    }
}