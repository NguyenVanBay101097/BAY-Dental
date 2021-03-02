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
    state: string;
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