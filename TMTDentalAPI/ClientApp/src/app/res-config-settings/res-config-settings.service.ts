import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs';

export class ResConfigSettingsDisplay {
    groupDiscountPerSOLine: boolean;
}

export class ResConfigSettingsBasic {
    id: string;
}

export class ResConfigSettingsSave {
    groupDiscountPerSOLine: boolean;
}

@Injectable()
export class ResConfigSettingsService {
    apiUrl = 'api/ResConfigSettings';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    create(val: ResConfigSettingsSave): Observable<ResConfigSettingsBasic> {
        return this.http.post<ResConfigSettingsBasic>(this.baseApi + this.apiUrl, val);
    }

    excute(id: string) {
        return this.http.post(this.baseApi + this.apiUrl + "/" + id + '/Excute', {});
    }

    defaultGet(): Observable<ResConfigSettingsDisplay> {
        return this.http.get<ResConfigSettingsDisplay>(this.baseApi + this.apiUrl + '/DefaultGet');
    }

    insertServiceCardData() {
        return this.http.post(this.baseApi + this.apiUrl + '/InsertServiceCardData', {});
    }

}