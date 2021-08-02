import { EmployeeBasic, EmployeeDisplay } from 'src/app/employees/employee';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { identifierModuleUrl } from '@angular/compiler';
import { HrPayrollStructureDisplay } from './hr-payroll-structure.service';
import { validator } from 'fast-json-patch';

export class HrPayslipPaging {
    offset: number;
    limit: number;
    totalItems: number;
    items: HrPayslipBasic[];
}

export class HrPayslipLinePaging {
    offset: number;
    limit: number;
    totalItems: number;
    items: HrPayslipLineDisplay[];
}

export class HrPayslipBasic{
    id: string;
    number: string;
    employeeName: string;
    state: string;
    dateFrom: string;
    dateTo: string;
    payslipRunName: string;
    name: string;
    totalAmount: number;
}

export class HrPayslipPaged {
    offset: number;
    limit: number;
    search: string;
    payslipId: string;
    state: string;
    dateFrom: any;
    dateTo: any;
    employeeId: any;
    payslipRunId: string;
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
    employee: EmployeeDisplay;
    dateFrom: string;
    dateTo: string;
    state: string;
    lines: HrPayslipLineDisplay[];
}
export class HrPayslipSaveDefaultValue {
    id = null;
    name = null;
    number = null;
    employeeId = null;
    employee = null;
    employeeNameSearch = null;
    state = 'draft';
    companyId = null;
    daySalary = null;
    workedDay = null;
    totalBasicSalary = null;
    overTimeHour = null;
    overTimeHourSalary = null;
    overTimeDay = null;
    overTimeDaySalary  = null;
    allowance  = null;
    otherAllowance  = null;
    rewardSalary = null;
    holidayAllowance = null;
    totalSalary = null;
    commissionSalary = null;
    amercementMoney = null;
    taxNSocialInsurance = null;
    advancePayment = null;
    netSalary = null;
    isCheck = false;
    isHidden = false;
    salaryPayment = null;
    accountPayment = null;
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

    computeSheet(val: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/ComputeSheet', val);
    }

    actionCancel(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + '/ActionCancel', ids);
    }

    actionDone(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + '/ActionDone', ids);
    }

    onChangeEmployee(val: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/OnChangeEmployee', val);
    }

    defaultget(val) {
        return this.http.post(this.baseApi + this.apiUrl + '/DefaultGet', val);
    }

    getWorkedDaysLines(id) {
        return this.http.get(this.baseApi + this.apiUrl + '/' + id + '/WorkedDaysLines');
    }

    getLines(id) {
        return this.http.get(this.baseApi + this.apiUrl + '/' + id + '/Lines');
    }
}
