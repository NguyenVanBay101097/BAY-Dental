import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { PagedResult2 } from '../core/paged-result-2';
import { PartnerSimple } from '../partners/partner-simple';
import { LaboOrderLineDisplay } from '../labo-order-lines/labo-order-line.service';
import { DotKhamBasic } from '../dot-khams/dot-khams';
import { SaleOrderBasic } from '../sale-orders/sale-order-basic';

export class LaboOrderPaged {
    limit: number;
    offset: number;
    search: string;
    partnerId: string;
    dateOrderFrom: string;
    dateOrderTo: string;
    datePlannedFrom: string;
    datePlannedTo: string;
    state: string;
    saleOrderId: string;
}

export class LaboOrderBasic {
    id: string;
    partnerName: string;
    customerName: string;
    name: string;
    dateOrder: string;
    state: string;
    amountTotal: string;
    saleOrderId: string;
    saleOrderName: string;
}

export class LaboOrderDefaultGet {
    dotKhamId: string;
    saleOrderId: string;
}

export class LaboOrderDisplay {
    id: string;
    name: string;
    state: string;
    partnerRef: string;
    partnerId: string;
    partner: PartnerSimple;
    saleOrder: SaleOrderBasic;
    dateOrder: string;
    amountTotal: number;
    datePlanned: string;
    dotKhamId: string;
    orderLines: LaboOrderLineDisplay[];
    dotKham: DotKhamBasic;
}

@Injectable()
export class LaboOrderService {
    apiUrl = 'api/LaboOrders';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any): Observable<PagedResult2<LaboOrderBasic>> {
        return this.http.get<PagedResult2<LaboOrderBasic>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
    }

    get(id): Observable<LaboOrderDisplay> {
        return this.http.get<LaboOrderDisplay>(this.baseApi + this.apiUrl + "/" + id);
    }

    create(val: LaboOrderDisplay): Observable<LaboOrderDisplay> {
        return this.http.post<LaboOrderDisplay>(this.baseApi + this.apiUrl, val);
    }

    update(id: string, val: LaboOrderDisplay) {
        return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
    }

    defaultGet(val: any): Observable<LaboOrderDisplay> {
        return this.http.post<LaboOrderDisplay>(this.baseApi + this.apiUrl + '/DefaultGet', val);
    }

    getPrint(id) {
        return this.http.get(this.baseApi + this.apiUrl + "/" + id + '/GetPrint');
    }

    buttonConfirm(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + '/ButtonConfirm', ids);
    }

    buttonCancel(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + '/ButtonCancel', ids);
    }

    unlink(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + '/Unlink', ids);
    }
}