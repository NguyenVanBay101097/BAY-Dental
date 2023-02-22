export class EmployeeCategoryBasic {
    id: string;
    name: string;
}

export class EmployeeCategoryDisplay {
    id: string;
    name: string;
    type: string;
}

export class EmployeeCategoryPaged {
    limit: number;
    offset: number;
    search: string;
}

export class PagedResult2<T>{
    offset: number;
    limit: number;
    totalItems: number;
    items: T[];
}