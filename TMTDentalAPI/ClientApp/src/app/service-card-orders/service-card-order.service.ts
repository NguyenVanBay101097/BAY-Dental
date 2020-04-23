import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ServiceCardOrderService {
    apiUrl = 'api/ServiceCardOrders';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any) {
        return this.http.get(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
    }

    get(id) {
        return this.http.get(this.baseApi + this.apiUrl + "/" + id);
    }

    create(val: any) {
        return this.http.post(this.baseApi + this.apiUrl, val);
    }

    update(id: string, val: any) {
        return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
    }

    defaultGet() {
        return this.http.get(this.baseApi + this.apiUrl + "/DefaultGet");
    }

    addPartners(id: string, ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + "/" + id + "/AddPartners", ids);
    }

    removePartners(id: string, ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + "/" + id + "/RemovePartners", ids);
    }

    getPartners(id: string) {
        return this.http.get(this.baseApi + this.apiUrl + "/" + id + "/GetPartners");
    }

    actionConfirm(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + "/ActionConfirm", ids);
    }
}