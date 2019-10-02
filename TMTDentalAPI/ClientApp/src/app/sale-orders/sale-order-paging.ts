import { SaleOrderBasic } from './sale-order-basic';

export class SaleOrderPaging {
    pageNumber: number;
    pageSize: number;
    totalPages: number;
    totalItems: number;
    items: SaleOrderBasic[];
}