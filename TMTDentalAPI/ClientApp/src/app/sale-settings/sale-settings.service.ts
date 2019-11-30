import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';

export class SaleSettingsDisplay {
    id: string;
    name: string;
}

@Injectable()
export class SaleSettingsService {
    apiUrl = 'api/SaleSettings';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getSetting(): Observable<SaleSettingsDisplay> {
        return this.http.get<SaleSettingsDisplay>(this.baseApi + this.apiUrl);
    }

    update(val: SaleSettingsDisplay) {
        return this.http.put(this.baseApi + this.apiUrl, val);
    }
}