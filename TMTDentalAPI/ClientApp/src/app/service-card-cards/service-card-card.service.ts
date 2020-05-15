import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ServiceCardCardService {
    apiUrl = 'api/ServiceCardCards';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any) {
        return this.http.get(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
    }

    getHistories(id: string) {
        return this.http.get(this.baseApi + this.apiUrl + '/' + id + '/GetHistories');
    }

    buttonActive(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + '/ButtonActive', ids);
    }

    exportExcel(data: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/ExportExcel', data, { responseType: 'blob' });
    }

    checkCode(val: any) {
        return this.http.get(this.baseApi + this.apiUrl + '/CheckCode', { params: new HttpParams({ fromObject: val }) });
    }
}