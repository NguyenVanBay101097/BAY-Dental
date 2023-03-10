import { HttpClient, HttpParams } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { AccountJournalSimple } from "../account-journals/account-journal.service";
import { EmployeeSimple } from "../employees/employee";


export class SalaryPaymentPaged {
    search: string;
    limit: number;
    offset: number;
    companyId: string;
}

export class SalaryPaymentDisplay {
    id: string;
    date: string;
    state: string;
    name: string;
    type: string;
    employeeId: string;
    employee: EmployeeSimple;
    journalId: string;
    journal: AccountJournalSimple;
    amount: number;
    reason: string;
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
    HrPayslipId = null;
}

@Injectable({ providedIn: 'root' })
export class SalaryPaymentService {
    apiUrl = "api/SalaryPayments";
    apiPrintUrl = "SalaryPayment";
    constructor(
        private http: HttpClient,
        @Inject("BASE_API") private baseApi: string
    ) { }

    getPaged(val: any) {
        return this.http.get(this.baseApi + this.apiUrl, {
            params: new HttpParams({ fromObject: val }),
        });
    }

    get(id: string) {
        return this.http.get(this.baseApi + this.apiUrl + "/" + id);
    }

    defaultGet(type: string) {
        return this.http.get(
            this.baseApi + this.apiUrl + "/DefaultGet?type=" + type);
    }

    create(val: any) {
        return this.http.post(this.baseApi + this.apiUrl, val);
    }

    update(id: string, val: any) {
        return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
    }

    actionConfirm(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + "/ActionConfirm", ids);
    }

    getPrint(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + "/Print", ids);
    }

    // getPrint(ids: string[]) {
    //     return this.http.post(this.baseApi + this.apiPrintUrl + "/Print", ids , { responseType: 'text' });
    // }

    public actionMultiSalaryPayment(val: any) {
        return this.http.post(this.baseApi + this.apiUrl + "/CreateMultiSalaryPayment", val);
    }

    public defaulCreateBy(val: any) {
        return this.http.post(this.baseApi + this.apiUrl + "/DefaulCreateBy", val);
    }
}
