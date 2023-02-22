export class ProductRequestLineBasic {
    id: string;
    productId: string;
    product: Object;
    productUOMId: string;
    productUOM: Object;
    saleOrderLineId: string;
    saleOrderLine: Object;
    productQty: number;
    sequence: number;
}

export class ProductRequestLineDisplay {
    id: string;
    productId: string;
    product: Object;
    productUOMId: string;
    productUOM: Object;
    saleOrderLineId: string;
    saleOrderLine: Object;
    productQty: number;
    sequence: number;
}

export class ProductRequestLineSave {
    id: string;
    productId: string;
    productUOMId: string;
    saleOrderLineId: string;
    productQty: number;
    sequence: number;
}