import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

export class SmsMessageDetailPaged {
    limit: number;
    offset: number;
    search: string;
    smsCampaignId: string;
}

@Injectable({
    providedIn: 'root'
})
export class SmsMessageDetailService {

    apiUrl: string = 'api/SmsMessageDetails';
    constructor(@Inject('BASE_API') private base_api: string, private http: HttpClient) { }



    getPaged(val) {
        return this.http.get(this.base_api + this.apiUrl, { params: val });
    }

    ReSend(ids) {
        return this.http.post(this.base_api + this.apiUrl + '/ReSend', ids);
    }
}
