import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs';
import { PrintPaperSizeBasic } from '../config-prints/print-paper-size.service';

export class ResConfigSettingsDisplay {
    groupDiscountPerSOLine: boolean;
    paperSizeId:string;
    printPaperSize: PrintPaperSizeBasic;
}

export class ResConfigSettingsBasic {
    id: string;
}

export class ResConfigSettingsSave {
    groupDiscountPerSOLine: boolean;
    paperSizeId:string;
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