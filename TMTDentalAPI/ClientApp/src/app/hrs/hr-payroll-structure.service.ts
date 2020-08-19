import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { identifierModuleUrl } from '@angular/compiler';

export class HrPayrollStructurePaging {
    offset: number;
    limit: number;
    totalItems: number;
    items: HrPayrollStructureDisplay[];
}

export class HrSalaryRulePaging {
    offset: number;
    limit: number;
    totalItems: number;
    items: HrSalaryRuleDisplay[];
}

export class HrPayrollStructurePaged {
    offset: number;
    limit: number;
    filter: string;
    id: string;
}

export class HrPayrollStructureSave {
    name: string;
    active: boolean;
    schedulePay: string;
    note: string;
    regularPay: boolean;
    typeId: string;
    useWorkedDayLines: boolean;
    rules: HrSalaryRuleDisplay[];
}

export class HrPayrollStructureDisplay {
    name: string;
    active: boolean;
    schedulePay: string;
    note: string;
    regularPay: boolean;
    typeId: string;
    typeName: string;
    useWorkedDayLines: boolean;
    rules: HrSalaryRuleDisplay[];
    totalRules: number;
    id: string;
}
// HrSalaryRule

export class HrSalaryRuleDisplay {
    id: string;
    name: string;
    code: string;
    sequence?: number;
    active: boolean;
    companyId?: boolean;
    amountSelect?: string;
    amountFix?: string;
    amountPercentage?: string;
    appearsOnPayslip: boolean;
    note: string;
    amountCodeCompute: string;
    amountPercentageBase: string;
    structId?: string;
}

@Injectable()
export class HrPayrollStructureService {
    apiUrl = 'api/HrPayrollStructures';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any): Observable<HrPayrollStructurePaging> {
        return this.http.get<HrPayrollStructurePaging>(this.baseApi + this.apiUrl, { params: val });
    }

    get(id): Observable<HrPayrollStructureDisplay> {
        return this.http.get<HrPayrollStructureDisplay>(this.baseApi + this.apiUrl + '/' + id);
    }

    create(val: HrPayrollStructureSave): Observable<HrPayrollStructureDisplay> {
        return this.http.post<HrPayrollStructureDisplay>(this.baseApi + this.apiUrl, val);
    }

    update(id: string, val: HrPayrollStructureSave) {
        return this.http.put(this.baseApi + this.apiUrl + '/' + id, val);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + '/' + id);
    }

    GetAllPayrollStructureType(val) {
        return this.http.get<HrPayrollStructurePaging>(this.baseApi + 'api/HrPayrollStructureTypes', { params: val });
    }

    GetListRuleByStructId(StructId: string) {
        return this.http.get<HrSalaryRulePaging>(this.baseApi + 'api/HrSalaryRules', { params: { StructId }});
    }
}
