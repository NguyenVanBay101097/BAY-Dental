import { Injectable, Inject } from '@angular/core';
import { HttpParams, HttpClient } from '@angular/common/http';
import { FacebookConfigSave } from './facebook-config-save';
import { Observable } from 'rxjs';
import { FacebookConfigBasic } from './facebook-config-basic';
import { FacebookConfigDisplay } from './facebook-config-display';
import { FacebookConfigConnectSave } from './facebook-config-connect-save';

@Injectable({
    providedIn: 'root'
})

export class FacebookConfigService {
    apiUrl = 'api/FacebookConfigs';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    get(id: string): Observable<FacebookConfigDisplay> {
        return this.http.get<FacebookConfigDisplay>(this.baseApi + this.apiUrl + "/" + id);
    }

    create(val: FacebookConfigSave): Observable<FacebookConfigBasic> {
        return this.http.post<FacebookConfigBasic>(this.baseApi + this.apiUrl, val);
    }

    connect(val: FacebookConfigConnectSave): Observable<FacebookConfigBasic> {
        return this.http.post<FacebookConfigBasic>(this.baseApi + this.apiUrl + '/Connect', val);
    }
}
