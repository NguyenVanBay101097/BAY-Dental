import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { PermissionTreeViewModel } from 'src/app/roles/role.service';

@Injectable({ providedIn: 'root' })
export class WebService {
    apiUrl = 'api/Web';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    uploadImage(data: any) {
        return this.http.post(this.baseApi + this.apiUrl + "/UploadImage", data);
    }

    uploadImages(data: any) {
        return this.http.post(this.baseApi + this.apiUrl + "/UploadImages", data);
    }

    UploadImageByBase64(uri: any) {
        return this.http.post(this.baseApi + this.apiUrl + "/UploadImageByBase64", {uri: uri});
    }

    impottSampleData(params) {
        return this.http.get(this.baseApi + this.apiUrl + '/ImportSampleData', { params: params })
    }

    removeSampleData() {
        return this.http.get(this.baseApi + this.apiUrl + '/DeleteSampleData');
    }

    getFeatures() {
        return this.http.get<PermissionTreeViewModel[]>(this.baseApi + this.apiUrl + '/Features');
    }

    getExpire() {
        return this.http.get(this.baseApi + this.apiUrl + '/GetExpire');
    }

}
