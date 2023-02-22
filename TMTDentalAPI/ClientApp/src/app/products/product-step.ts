import { Product } from './product';

export class ProductStepDisplay {
    id: string;
    name: string;
    order: number;
    note: string;
    productId: string;
    product: Product;
    default: boolean;
}

export class ProductStepSimple {
    id: string;
    name: string;
}

export class ProductDisplayAndStep extends Product {
    stepList: ProductStepDisplay[];
}
