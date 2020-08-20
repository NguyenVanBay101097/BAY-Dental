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

export class HrPayslipPaged {
    offset: number;
    limit: number;
    search: string;
}

export class HrPayslipSave {
    structId: string;
    name: string;
    number: string;
    employeeId: string;
    dateFrom: string;
    dateTo: string;
    state: string;
    lines: HrPayslipLineDisplay[];
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
        return this.http.post(this.baseApi + this.apiUrl + '/ComputeSp', val);
    }

    ComputeLinePut( id: string, val: any) {
        return this.http.put(this.baseApi + this.apiUrl + '/ComputeSp/' + id, val);
    }
}
