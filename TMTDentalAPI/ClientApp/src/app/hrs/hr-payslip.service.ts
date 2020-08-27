import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { identifierModuleUrl } from '@angular/compiler';
import { HrPayrollStructureDisplay } from './hr-payroll-structure.service';

export class HrPayslipPaging {
    offset: number;
    limit: number;
    totalItems: number;
    items: HrPayslipDisplay[];
}

export class HrPayslipLinePaging {
    offset: number;
    limit: number;
    totalItems: number;
    items: HrPayslipLineDisplay[];
}

export class HrPayslipPaged {
    offset: number;
    limit: number;
    search: string;
    payslipId: string;
    state: string;
    dateFrom: any;
    dateTo: any;
}

export class HrPayslipSave {
    structId: string;
    name: string;
    number: string;
    employeeId: string;
    dateFrom: string;
    dateTo: string;
    state: string;
    // lines: HrPayslipLineDisplay[];
}

export class HrPayslipDisplay {
    id: string;
    structId: string;
    struct: HrPayrollStructureDisplay;
    name: string;
    number: string;
    employeeId: string;
    dateFrom: string;
    dateTo: string;
    state: string;
    lines: HrPayslipLineDisplay[];
}

// line
export class HrPayslipLineDisplay {
    id: string;
    name: string;
    code: string;
    quantity: number;
    amount: number;
    total: number;
    slipId: string;
    salaryRuleId: string;
    rate: number;
    categoryId: string;
    sequence: number;
}

export class HrPayslipWorkedDaySave {
    id: string;
    name: string;
    payslipId: string;
    sequence?: number;
    code: string;
    numberOfDays: number;
    numberOfHours: number;
    workEntryTypeId: string;
    amount: number;
}

@Injectable()
export class HrPayslipService {
    apiUrl = 'api/HrPayslips';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any): Observable<HrPayslipPaging> {
        return this.http.get<HrPayslipPaging>(this.baseApi + this.apiUrl, { params: val });
    }

    get(id): Observable<HrPayslipDisplay> {
        return this.http.get<HrPayslipDisplay>(this.baseApi + this.apiUrl + '/' + id);
    }

    create(val: HrPayslipSave): Observable<HrPayslipDisplay> {
        return this.http.post<HrPayslipDisplay>(this.baseApi + this.apiUrl, val);
    }

    update(id: string, val: HrPayslipSave) {
        return this.http.put(this.baseApi + this.apiUrl + '/' + id, val);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + '/' + id);
    }

    ComputeLinePost(val: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/ComputePayslip', val);
    }

    ComputeLinePut(id: string) {
        return this.http.put(this.baseApi + this.apiUrl + '/ComputePayslipLineUpdate/' + id, null);
    }

    getPayslipLinePaged(val: any): Observable<HrPayslipLinePaging> {
        return this.http.get<HrPayslipLinePaging>(this.baseApi + 'api/HrPayslipLines', { params: val });
    }

    CancelCompute(id: string) {
        return this.http.put(this.baseApi + this.apiUrl + '/CancelCompute/' + id, null);
    }

    ConfirmCompute(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + '/ConfirmCompute/', ids);
    }

    GetWorkedDayInfoByEmployee(val: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/OnChangeEmployee', val);
    }

    GetWorkedDayInfoByPayslipId(val: any) {
        return this.http.get(this.baseApi + 'api/HrPayslipWorkedDays', { params: val });
    }
}
