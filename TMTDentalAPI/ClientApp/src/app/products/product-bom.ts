import { Product } from './product';

export class ProductBomDisplay {
    id: string;
    name: string;
    materialProductId: string;
    materialProduct: Product;
    productUOMId: string;
    productId: string;
    product: Product;
    quantity: number;
    sequence: number = 0;
}