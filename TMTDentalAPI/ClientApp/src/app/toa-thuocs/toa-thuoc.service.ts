import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { ProductSimple } from '../products/product-simple';

export class ToaThuocDefaultGet {
    dotKhamId: string;
}

export class ToaThuocBasic {
    id: string;
    name: string;
    date: string;
}

export class ToaThuocDisplay {
    id: string;
    name: string;
    partnerId: string;
    partner: object;
    date: string;
    dotKhamId: string;
    dotKham: object;
    userId: string;
    user: object;
    note: string;
    state: string;
    companyId: string;
    lines: []
}

export class ToaThuocSave {
    id: string;
    partnerId: string;
    date: string;
    note: string;
    dotKhamId: string;
    userId: string;
    companyId: string;
    lines: []
}

export class ToaThuocLineDisplay {
    id: string;
    productId: string;
    product: ProductSimple;
    quantity: number;
    note: string;
}

export class ToaThuocLineDefaultGet {
    productId: string;
}

export class ToaThuocPrint {
    companyName: string;
    companyAddress: string;
    companyPhone: string;
    companyEmail: string;
    partnerName: string;
    partnerAddress: string;
    partnerPhone: string;
    partnerAge: string;
    partnerGender: string;
    date: string;
    note: string;
    lines: ToaThuocLinePrint[];
}

export class ToaThuocLinePrint {
    productName: string;
    quantity: number;
    sequence: number;
}

export class ToaThuocPaged {
    limit: number;
    offset: number;
    search: string;
    partnerId: string;
}

export class ToaThuocPaging {
    offset: number;
    limit: number;
    totalItems: number;
    items: [];
}

@Injectable()
export class ToaThuocService {
    apiUrl = 'api/toathuocs';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    defaultGet(val: ToaThuocDefaultGet): Observable<ToaThuocDisplay> {
        return this.http.post<ToaThuocDisplay>(this.baseApi + this.apiUrl + "/defaultget", val);
    }

    lineDefaultGet(val: ToaThuocLineDefaultGet): Observable<ToaThuocLineDisplay> {
        return this.http.post<ToaThuocLineDisplay>(this.baseApi + this.apiUrl + "/LineDefaultGet", val);
    }

    getPaged(val: any): Observable<ToaThuocPaging> {
        return this.http.get<ToaThuocPaging>(this.baseApi + this.apiUrl, { params: val });
    }

    get(id): Observable<ToaThuocDisplay> {
        return this.http.get<ToaThuocDisplay>(this.baseApi + this.apiUrl + "/" + id);
    }

    create(val: ToaThuocDisplay): Observable<ToaThuocDisplay> {
        return this.http.post<ToaThuocDisplay>(this.baseApi + this.apiUrl, val);
    }

    update(id: string, val: ToaThuocDisplay) {
        return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
    }

    actionConfirm(id: string) {
        return this.http.post(this.baseApi + this.apiUrl + `/${id}/ActionConfirm`, null);
    }

    getPrint(id: string): Observable<ToaThuocPrint> {
        return this.http.get<ToaThuocPrint>(this.baseApi + this.apiUrl + `/${id}/Print`);
    }
}