import { Injectable, Inject } from '@angular/core';
import { HttpParams, HttpClient } from '@angular/common/http';

@Injectable({
    providedIn: 'root'
})

export class FacebookConfigPageService {
    apiUrl = 'api/FacebookConfigPages';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    syncData(id: string) {
        return this.http.get(this.baseApi + this.apiUrl + "/" + id + '/SyncData');
    }

    getConversations(pageId: string, data: any) {
        return this.http.get(this.baseApi + this.apiUrl + "/" + pageId + '/Conversations', { params: new HttpParams({ fromObject: data }) });
    }
}
