import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export class ZaloOAConfigSave {
    accessToken: string;
    pageId: string;
}

export class ZaloOAConfigUpdate {
    autoSendBirthdayMessage: boolean;
    birthdayMessageContent: string;
}

export class ZaloOAConfigBasic {
    id: string;
    name: string;
    avatar: string;
}

@Injectable()
export class ZaloOAConfigService {
    apiUrl = 'api/ZaloOAConfigs';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    get(): Observable<ZaloOAConfigBasic> {
        return this.http.get<ZaloOAConfigBasic>(this.baseApi + this.apiUrl);
    }

    create(val: ZaloOAConfigSave): Observable<ZaloOAConfigBasic> {
        return this.http.post<ZaloOAConfigBasic>(this.baseApi + this.apiUrl, val);
    }

    update(id: string, val: ZaloOAConfigUpdate) {
        return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
    }

    remove() {
        return this.http.delete(this.baseApi + this.apiUrl);
    }
}