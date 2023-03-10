import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { ODataService } from './odata.service';

@Injectable({ providedIn: 'root' })
export class SaleOrdersOdataService extends ODataService {
    constructor(http: HttpClient, @Inject('BASE_API') baseUrl: string) { super(http, baseUrl, "SaleOrders"); }

    public getDisplay(id: string) {
        return this.getFunction(id, 'GetDisplay');
    }

    defaultGet(p?: any) {
        return this.http.get(`${this.BASE_URL}${this.tableName}/DefaultGet`, { params: new HttpParams({ fromObject: p || {} }) });
    }

    public getSaleOrderLines(val) {
        return this.getFunction(val.id, val.func, val.options);
    }

    public getDotKhamStepByOrderLine(id: any) {
        return this.getFunction(id, 'GetDotKhamStepByOrderLine');
    }

    public createDotkham(id: any, val: any) {
        return this.http.post(`${this.BASE_URL}${this.tableName}(${id})` + '/CreateDotKham', val);
    }

    getDotKhamListIds(id: any) {
        return this.getFunction(id, 'GetDotKhamListIds');
    }

    actionConvertToOrder(id: string) {
        return this.postAction('ActionConvertToOrder', null, id);
    }

    actionDone(ids: string[]) {
        return this.postAction('OnChangePartner',{Ids: ids});
    }

    actionConfirm(ids: string[]) {
        return this.postAction('OnChangePartner',{Ids: ids});
    }

    onChangePartner(val: any) {
        return this.postAction('OnChangePartner',val);

    }

    postAction(action: string, val: any, id = null) {
        return this.http.post(`${this.BASE_URL}${this.tableName}${id?`(${id})`:''}` + `/${action}`, val);
    }
}
