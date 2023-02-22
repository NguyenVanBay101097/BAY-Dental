import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs';

export class ChannelSocial {
    id: string;
    pageId: string;
    pageName: string;
    avatar: string;
    type: string;
}

export class ChannelSocialPaging {
    offset: number;
    limit: number;
    totalItems: number;
    items: ChannelSocial[];
}

export class MultiUserProfilesVm{
    pageId: string;
    userIds: string[];
}

@Injectable({ providedIn: 'root' })
export class FacebookPageService {
    apiUrl = 'api/FacebookPages';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any): Observable<ChannelSocialPaging> {
        return this.http.get<ChannelSocialPaging>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
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

    syncNumberPhoneUsers(ids: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/SyncNumberPhoneUsers', ids);
    }

    syncPhoneForMultiUsers(val: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/SyncPhoneForMultiUsers', val);
    }

    syncPartners(ids: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/SyncPartnersForNumberPhone', ids);
    }

    syncPartnerForMultiUsers(val: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/SyncPartnersForMultiUser', val);
    }

    refreshPage(val: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/RefreshSocialChannel', val);
    }
}