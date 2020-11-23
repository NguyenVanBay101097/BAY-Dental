import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { map } from 'rxjs/operators';
import { ODataService } from './odata.service';

export class SalaryPaymentSave {
    CompanyId: string;
    Name: string;
    Date: Date;
    JournalId: string;
    Journal: any;
    EmployeeId: string;
    Employee: any;
    State: string;
    Type: string;
    Amount: number;
    Reason: string;
}

@Injectable({ providedIn: 'root' })
export class SalaryPaymentService extends ODataService {
    constructor(http: HttpClient, @Inject('BASE_API') baseUrl: string) { super(http, baseUrl, "SalaryPayments"); }

    public getView(state: any, options?: any): void {
        this.fetch(this.tableName, state, options || {})
            .pipe(
                map((data: GridDataResult) => {

                    return data;
                })
            )
            .subscribe((x: any) => {
                super.next(x);
            });
    }

}
