import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { map } from 'rxjs/operators';
import { ODataService } from './odata.service';

export class PartnerImageBasic {
    id: string;
    name: string;
    date: any = new Date();
    note: string;
    uploadId: string;
    dotKhamId: string;
    partnerId: string;
}


@Injectable({ providedIn: 'root' })
export class PartnersService extends ODataService {
    constructor(http: HttpClient, @Inject('BASE_API') baseUrl: string) { super(http, baseUrl, "Partners"); }

    public getView(state: any, options?: any): void {
        this.fetch(this.tableName + '/GetView', state, options || {})
            .pipe(
                map((data: GridDataResult) => {
                    data.data = data.data.map(x => {
                        x.Tags = x.Tags ? JSON.parse(x.Tags) : [];
                        return x;
                    });
                    return data;
                })
            )
            .subscribe((x: any) => {
                super.next(x);
            });
    }

    public getDisplay(id: string) {
        return this.http.get(`${this.BASE_URL}${this.tableName}(${id})/GetDisplay`);
    }

    public getViewByPhone(phone: string) {
        return this.fetch(this.tableName, {
            filter: {
                logic: 'and',
                filters: [
                    { field: 'phone', operator: 'eq', value: phone }
                ]
            },
        });
    }

    public getSaleOrderByPartner(val: any) {
        return this.getFunction(val.id, val.func, val.options)
    }

    public getSaleOrderLineByPartner(val: any) {
        return this.getFunction(val.id, val.func)
    }
}
