import { ProductSimple } from '../products/product-simple';
import { ToothDisplay } from '../teeth/tooth.service';
import { ToothCategoryBasic } from '../tooth-categories/tooth-category.service';

export class SaleOrderLineDisplay {
    priceUnit: number;
    product: ProductSimple;
    productId: string;
    teeth: ToothDisplay[];
    toothCategory: ToothCategoryBasic;
}