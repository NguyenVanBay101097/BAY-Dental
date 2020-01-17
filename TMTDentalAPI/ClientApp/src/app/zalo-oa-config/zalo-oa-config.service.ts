import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';

export class ZaloOAConfigSave {
    accessToken: string;
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

    remove() {
        return this.http.delete(this.baseApi + this.apiUrl);
    }
}