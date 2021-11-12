import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { ODataService } from './odata.service';

@Injectable({ providedIn: 'root' })
export class ProductsOdataService extends ODataService {
    constructor(http: HttpClient, @Inject('BASE_API') baseUrl: string) { super(http, baseUrl, "Products"); }

    public getFetch(state, options?: any) {
        options = options || {};
        return this.fetch(this.tableName, state, options);
    }
}
