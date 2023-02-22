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

export class PartnerMap {
    id: string; // map id
    partnerId: string;
    partnerName: string;
    partnerPhone: string; 
    partnerEmail: string;
}

export class CheckPartner {
    PageId: string;
    PSId: string;
}

export class MapPartner {
    PartnerId: string;
    PageId: string;
    PSId: string;
}

@Injectable({providedIn: 'root'})
export class SocialsChannelService {
    apiUrl = 'api/FacebookPages';
    apiUrl2 = 'api/PartnerMapPSIDFacebookPage';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }
    
    getPageBasic(val: any): Observable<SocialsChannelPaging<SocialsChannelBasic>> {
        return this.http.get<SocialsChannelPaging<SocialsChannelBasic>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
    }

    get(id): Observable<SocialsChannelBasic> {
        return this.http.get<SocialsChannelBasic>(this.baseApi + this.apiUrl + "/" + id);
    }

    create(data: SocialsChannelDisplay) {
        return this.http.post(this.baseApi + this.apiUrl, data);
    }

    checkPartner(val: any): Observable<any>{
        return this.http.post(this.baseApi + this.apiUrl2 + '/CheckPartner', val);
    }

    mapPartner(val: any): Observable<any>{
        return this.http.post(this.baseApi + this.apiUrl2, val);
    }

    unlinkPartner(val: any): Observable<any>{
        return this.http.post(this.baseApi + this.apiUrl2 + '/Unlink', val);
    }
}