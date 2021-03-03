import { ProductRequestLineDisplay } from "./product-request-line";

export class ProductRequestBasic {
    id: string;
    name: string;
    userId: string;
    user: Object;
    date: string;
    employeeId: string;
    employee: Object;
    pickingId: string;
    picking: Object;
    state: string;
}

export class ProductRequestPaged {
    offset: number;
    limit: number;
    search: string;
    saleOrderId: string;
    dateFrom: string;
    dateTo: string;
    states: string[];
}

export class ProductRequestDisplay {
    id: string;
    name: string;
    userId: string;
    user: Object;
    date: string;
    employeeId: string;
    employee: Object;
    pickingId: string;
    picking: Object;
    state: string;
    lines: ProductRequestLineDisplay;
}

export class ProductRequestSave {
    userId: string;
    date: string;
    employeeId: string;
    saleOrderId: string;
    pickingId: string;
    state: string;
    lines: ProductRequestLineDisplay;
}

export class GetLinePar {
    saleOrderLineId: string;
    productBomId: string;
}

export class ProductRequestDefaultGet {
    saleOrderId: string;
}