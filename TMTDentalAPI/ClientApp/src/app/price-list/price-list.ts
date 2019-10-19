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
}

export class ProductPriceListSave {
    name: string;
    items: ProductPriceListItemSave[];
    dateStart: string;
    dateEnd: string;
}


