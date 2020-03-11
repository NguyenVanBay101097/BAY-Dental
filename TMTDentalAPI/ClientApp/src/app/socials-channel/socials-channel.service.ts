import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';

export class SocialsChannelBasic {
    id: string;
    pageId: string;
    pageName: string;
    pageAccesstoken: string;
}

export class SocialsChannelDisplay {
    pageId: string;
    accesstoken: string;
}

export class SocialsChannelPaged {
    offset: number;
    limit: number;
    search: string;
}

export class SocialsChannelPaging<T> {
    offset: number;
    limit: number;
    totalItems: number;
    items: T[];
}

@Injectable({providedIn: 'root'})
export class SocialsChannelService {
    apiUrl = 'api/FacebookPages';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }
    
    getPageBasic(val: any): Observable<SocialsChannelPaging<SocialsChannelBasic>> {
        return this.http.get<SocialsChannelPaging<SocialsChannelBasic>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
    }

    get(id): Observable<SocialsChannelBasic> {
        return this.http.get<SocialsChannelBasic>(this.baseApi + this.apiUrl + "/" + id);
    }

    create(data: SocialsChannelDisplay) {
        return this.http.post(this.baseApi + this.apiUrl + '/Create', data);
    }
}