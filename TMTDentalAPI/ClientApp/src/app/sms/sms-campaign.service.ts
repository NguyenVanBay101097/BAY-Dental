import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

export class SmsCampaignBasic {
    id: string;
    name: string;
    typeDate: string;
    dateStart: string;
    dateEnd: string;
    state: string;
    limitMessage: string;
}

export class SmsCampaignPaged {
    limit: number;
    offset: number;
    search: string;
    state: string;
}

@Injectable({
    providedIn: 'root'
})
export default class SmsCampaignService {

    apiUrl: string = 'api/SmsCampaigns';
    constructor(@Inject('BASE_API') private base_api: string, private http: HttpClient) { }

    create(val) {
        return this.http.post(this.base_api + this.apiUrl, val);
    }

    update(id: string, val) {
        return this.http.put(this.base_api + this.apiUrl + '/' + id, val);
    }

    getDefaultCampaignAppointmentReminder() {
        return this.http.get(this.base_api + this.apiUrl + '/GetDefaultCampaignAppointmentReminder')
    }

    getDefaultCampaignBirthday() {
        return this.http.get(this.base_api + this.apiUrl + '/GetDefaultCampaignBirthday')
    }

    getPaged(val: any) {
        return this.http.get(this.base_api + this.apiUrl + '/GetPaged', { params: val })
    }

    delete(id: string) {
        return this.http.delete(this.base_api + this.apiUrl + '/' + id);
    }
}
