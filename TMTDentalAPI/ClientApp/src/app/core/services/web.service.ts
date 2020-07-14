import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class WebService {
    apiUrl = 'api/Web';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    uploadImage(data: any) {
        return this.http.post(this.baseApi + this.apiUrl + "/UploadImage", data);
    }

    impottSampleData(params) {
        return this.http.get(this.baseApi + this.apiUrl + '/ImportSampleData', { params: params })
    }
}