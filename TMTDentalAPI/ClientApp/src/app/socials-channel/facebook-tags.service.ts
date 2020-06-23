import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';

export class FacebookTagsPaged {
    offset: number;
    limit: number;
    search: string;
}


export class FacebookTagBasic {
    id: string;
    name: string;
}

@Injectable({ providedIn: 'root' })
export class FacebookTagsService {
    apiUrl = 'api/FacebookTags';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getTags(val: any) {
        return this.http.get(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
    }

    create(val: any) {
        return this.http.post(this.baseApi + this.apiUrl, val);
    }
}