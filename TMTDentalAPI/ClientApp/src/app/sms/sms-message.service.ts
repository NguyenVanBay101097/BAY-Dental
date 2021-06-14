import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

export class SmsMessagePaged {
    limit: number;
    offset: number;
    search: string;
    campaignId: string;
    smsAccountId: string;
    state: string;
    dateFrom: string;
    dateTo: string;
}

@Injectable({
    providedIn: 'root'
})
export class SmsMessageService {

    apiUrl: string = 'api/SmsMessages';
    constructor(@Inject('BASE_API') private base_api: string, private http: HttpClient) { }

    create(val) {
        return this.http.post(this.base_api + this.apiUrl, val);
    }

    ActionCreateReminder(val){
        return this.http.post(this.base_api + this.apiUrl, val);
    }

    getPaged(val) {
        return this.http.get(this.base_api + this.apiUrl, { params: val });
    }

    actionSendSms(id: string) {
        return this.http.get(`${this.base_api}${this.apiUrl}/${id}/ActionSendSms`);
    }

    actionCancelSendSMS(vals: string[]) {
        return this.http.post(this.base_api + this.apiUrl + '/ActionCancel', vals)
    }

    SetupSendSmsOrderAutomatic(orderId) {
        return this.http.get(this.base_api + this.apiUrl + '/SetupSendSmsOrderAutomatic/' + orderId);
    }

    actionStartJobAutomatic() {
        return this.http.get(this.base_api + this.apiUrl + '/ActionStartJobAutomatic');
    }

    actionStopJobAutomatic() {
        return this.http.get(this.base_api + this.apiUrl + '/ActionStopJobAutomatic');
    }
}
