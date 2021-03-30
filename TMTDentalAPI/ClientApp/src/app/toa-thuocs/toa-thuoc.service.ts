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
    reExaminationDate: string;
    lines: [];
    saleOrderId: string;
}

export class ToaThuocDisplayFromUI {
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
    lines: [];
    saleOrderId: string;
    reExaminationDate: string;
    saveSamplePrescription: boolean;
    nameSamplePrescription: string;
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

export class ToaThuocLineSave {
    id: string;
    product: ProductSimple;
    productId: string;
    numberOfTimes: number;
    amountOfTimes: number;
    quantity: number;
    unit: string;
    numberOfDays: number;
    useAt: string;
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

export class ToaThuocVM {
    dd: string;
    name: string;
    date: string;
    partnerName: string;
    saleOrderId: string;
    saleOrderName: string;
    employeeName: string;
    diagnostic: string;
    partnerId: string;
}

export class ToaThuocPaged {
    limit: number;
    offset: number;
    search: string;
    partnerId: string;
    saleOrderId: string;
    dateFrom: string;
    dateTo: string;
}

export class ToaThuocPaging {
    offset: number;
    limit: number;
    totalItems: number;
    items: ToaThuocVM[];
}

@Injectable({ providedIn: 'root' })
export class ToaThuocService {
    apiUrl = 'api/toathuocs';
    apiPrintUrl = 'ToaThuoc';
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
        return this.http.put(this.baseApi + this.apiUrl + `/${id}`, val);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
    }

    actionConfirm(id: string) {
        return this.http.post(this.baseApi + this.apiUrl + `/${id}/ActionConfirm`, null);
    }

    // getPrint(id: string) {
    //     return this.http.get(this.baseApi + this.apiUrl + `/${id}/Print`);
    // }

    getPrint(id: string) {
        return this.http.get(this.baseApi + this.apiPrintUrl + '/Print' + `/${id}`);
    }

    getFromUI(id): Observable<ToaThuocDisplay> {
        return this.http.get<ToaThuocDisplay>(this.baseApi + this.apiUrl + `/${id}/GetFromUI`);
    }

    createFromUI(val: ToaThuocDisplayFromUI): Observable<ToaThuocDisplayFromUI> {
        return this.http.post<ToaThuocDisplayFromUI>(this.baseApi + this.apiUrl + "/CreateFromUI", val);
    }

    updateFromUI(id: string, val: ToaThuocDisplayFromUI) {
        return this.http.put(this.baseApi + this.apiUrl + `/${id}/UpdateFromUI`, val);
    }
}