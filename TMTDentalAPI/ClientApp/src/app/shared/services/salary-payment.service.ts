import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { map } from 'rxjs/operators';
import { ODataService } from './odata.service';

@Injectable({ providedIn: 'root' })
export class SalaryPaymentService extends ODataService {
    constructor(http: HttpClient, @Inject('BASE_API') baseUrl: string) { super(http, baseUrl, "SalaryPayment"); }

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

}
