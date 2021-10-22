import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';

export class ServiceCardCardFilter {
    partnerId: string;
    productId: string;
    state: string;
}

export class ServiceCardCardSave{
    cardTypeId: string;
    partnerId: string;
    barcode: string;
}

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

    create(val: ServiceCardCardSave){
        return this.http.post(this.baseApi + this.apiUrl, val);
    }
    
    update(id: string, val){
        return this.http.put(this.baseApi + this.apiUrl + '/' + id, val);
    }

    delete(id: string){
        return this.http.delete(this.baseApi + this.apiUrl + '/' + id);
    }

    buttonActive(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + '/ButtonActive', ids);
    }
    
    buttonLock(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + '/ButtonLock', ids);
    }
    
    buttonCancel(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + '/ButtonCancel', ids);
    }

    exportExcel(data: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/ExportExcel', data, { responseType: 'blob' });
    }

    checkCode(val: any) {
        return this.http.get(this.baseApi + this.apiUrl + '/CheckCode', { params: new HttpParams({ fromObject: val }) });
    }
        
}