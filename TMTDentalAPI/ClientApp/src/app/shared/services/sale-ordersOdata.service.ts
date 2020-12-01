import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { map } from 'rxjs/operators';
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
    
    public getDotKhamStepByOrderLine(id: string) {
        return this.getFunction(id, 'GetDotKhamStepByOrderLine');
    }
}
