import { EmployeeCategoryBasic } from '../employee-categories/emp-category';

export class EmployeePaged {
    offset: number;
    limit: number;
    search: string;
    position: string;
}

export class EmployeeBasic {
    id: string;
    name: string;
    phone: string;
    ref: string;
    email: boolean;
    categoryId: string;
    category: EmployeeCategoryBasic;
}

export class EmployeeDisplay extends EmployeeBasic {
    address: string;
    identityCard: string;
    birthDay: Date;
    isDoctor: boolean;
    isAssistant: boolean;
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