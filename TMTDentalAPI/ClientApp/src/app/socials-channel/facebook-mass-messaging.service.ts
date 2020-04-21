import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';

export class FacebookMassMessagingPaged {
    limit: number;
    offset: number;
    search: string;
}

export class ActionStatisticsPaged {
    id: string;
    offset: number;
    limit: number;
    type: string;
}

export class TagStatisticsPaged {
    id: string;
    type: string;
    tagIds: string[];
}

export class AudienceFilterItem {
    type: string;
    name: string;
    formula_type: string;
    formula_value: string;
    formula_display: string;
}

@Injectable({ providedIn: 'root' })
export class FacebookMassMessagingService {
    apiUrl = 'api/FacebookMassMessagings';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any) {
        return this.http.get(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
    }

    get(id) {
        return this.http.get(this.baseApi + this.apiUrl + "/" + id);
    }

    defaultGet() {
        return this.http.get(this.baseApi + this.apiUrl + "/DefaultGet");
    }

    create(val: any) {
        return this.http.post(this.baseApi + this.apiUrl, val);
    }

    update(id: string, val: any) {
        return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
    }

    actionSend(ids: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/ActionSend', ids);
    }

    actionCancel(ids: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/ActionCancel', ids);
    }

    setScheduleDate(data: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/SetScheduleDate', data);
    }

    getActionStatistics(id, val: any) {
        return this.http.get(this.baseApi + this.apiUrl + "/" + id + '/ActionStatistics',  { params: new HttpParams({ fromObject: val }) });
    }

    setTagStatistics(data: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/TagStatistics', data);
    }
}