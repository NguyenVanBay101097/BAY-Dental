import { EmployeeCategoryBasic } from '../employee-categories/emp-category';
import { ChamCongBasic } from '../time-keeping/time-keeping.service';
import { HrPayrollStructureTypeSimple } from '../hrs/hr-payroll-structure-type.service';

export class EmployeePaged {
    offset: number;
    limit: number;
    search: string;
    position: string;
    isDoctor: boolean;
    isAssistant: boolean;
    ids: string[];
}

export class EmployeeBasic {
    id: string;
    name: string;
    phone: string;
    ref: string;
    email: boolean;
    categoryId: string;
    category: EmployeeCategoryBasic;
    chamCongs: ChamCongBasic[];
}

export class EmployeeDisplay extends EmployeeBasic {
    address: string;
    identityCard: string;
    birthDay: any;
    isDoctor: boolean;
    isAssistant: boolean;
    structureTypeId: string;
    structureType: HrPayrollStructureTypeSimple;
    wage: number;
    hourlyWage: number;
    startWorkDate: any;
}

export class PagedResult2<T>{
    offset: number;
    limit: number;
    totalItems: number;
    items: T[];
}

export class EmployeeSimple {
    id: string;
    name: string;
}

export class EmployeeSimpleContact {
    id: string;
    name: string;
    phone: string;
}