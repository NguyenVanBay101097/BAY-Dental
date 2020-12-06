import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { ODataService } from './odata.service';

@Injectable({ providedIn: 'root' })
export class SaleOrderLinesOdataService extends ODataService {
    constructor(http: HttpClient, @Inject('BASE_API') baseUrl: string) { super(http, baseUrl, "SaleOrderLines"); }

    getTeethList(id: any) {
        return this.getFunction(id, 'GetTeethList');
    }
}
