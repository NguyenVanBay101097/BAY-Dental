import { HttpClient } from '@angular/common/http';
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
    userCampaign: boolean;
    companyId: string;
}

@Injectable({
    providedIn: 'root'
})
export class SmsCampaignService {

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

    getDefaultCampaign() {
        return this.http.get(this.base_api + this.apiUrl + '/GetDefaultCampaign')
    }

    getDefaultCampaignBirthday() {
        return this.http.get(this.base_api + this.apiUrl + '/GetDefaultCampaignBirthday')
    }

    getDefaultThanksCustomer() {
        return this.http.get(this.base_api + this.apiUrl + '/GetDefaultThanksCustomer')
    }

    getDefaultCareAfterOrder() {
        return this.http.get(this.base_api + this.apiUrl + '/GetDefaultCareAfterOrder')
    }

    getPaged(val: any) {
        return this.http.get(this.base_api + this.apiUrl + '/GetPaged', { params: val })
    }

    delete(id: string) {
        return this.http.delete(this.base_api + this.apiUrl + '/' + id);
    }

    get(id: string) {
        return this.http.get(this.base_api + this.apiUrl + '/' + id);
    }
}
