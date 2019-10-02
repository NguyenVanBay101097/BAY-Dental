import { Product } from './product';

export class ProductPaging {
    pageNumber: number;
    pageSize: number;
    totalPages: number;
    totalItems: number;
    items: Product[];
}

