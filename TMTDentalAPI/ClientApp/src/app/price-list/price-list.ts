import { ProductSimple } from '../products/product-simple';
import { ProductCategoryBasic } from '../product-categories/product-category.service';

export class ProductPriceListItemSave {
    id: string;
    productId: string;
    product: ProductSimple;
    categId: string;
    categ: ProductCategoryBasic;
    appliedOn: string;
    dateStart: string;
    dateEnd: string;
    computePrice: string;
    fixedPrice: number;
    percentPrice: number;
    partnerCategId: string;
}

export class ProductPriceListSave {
    name: string;
    items: ProductPriceListItemSave[];
    dateStart: string;
    dateEnd: string;
    partnerCategId: string;
}

export class ProductPricelistPaged {
    offset: number;
    limit: number;
    search: string;
}


export class ProductPriceListBasic {
    id: string;
    name: string;
}


