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

export class SalaryPaymentSaveDefault {
    Date = null;
    JournalId = null;
    EmployeeId = null;
    Employee = null;
    State = null;
    Type = null;
    Amount = null;
    Reason = null;
}

export class SalaryPaymentIds {
   Ids: string[];
}

export class SalaryPaymentSalary {
    MultiSalaryPayments: any[];
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

    public getIdSP(id: any) {
        var obj = {
            $expand: "Employee,Journal",
        };
        return this.get(id, obj);
    }

    public actionConfirm(ids:any){
        var val = new SalaryPaymentIds();
        val.Ids = ids;
        return this.http.post(`${this.BASE_URL}${this.tableName}` + '/ActionConfirm', val );
    }

    public actionMultiSalaryPayment(val:any){
        var res = new SalaryPaymentSalary();
        res.MultiSalaryPayments = val;
        return this.http.post(`${this.BASE_URL}${this.tableName}` + '/CreateMultiSalaryPayment', res );
    }

    public onPrint(ids:any){
        var val = new SalaryPaymentIds();
        val.Ids = ids;
        return this.http.post(`${this.BASE_URL}${this.tableName}` + '/PrintSalaryPayment', val );
    }

    public defaulCreateBy(val: any) {
        return this.http.post(`${this.BASE_URL}${this.tableName}/DefaulCreateBy`, val);
    }

    public createMultiSalaryPayment(val: any[]) {
        const v = {
            vals: val
        };
        return this.http.post(`${this.BASE_URL}${this.tableName}/CreateMultiSalaryPayment`, v);
    }

}
