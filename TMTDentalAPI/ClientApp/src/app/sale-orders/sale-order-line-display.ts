import { ProductSimple } from '../products/product-simple';
import { ToothDisplay } from '../teeth/tooth.service';
import { ToothCategoryBasic } from '../tooth-categories/tooth-category.service';
import { SaleOrderBasic } from './sale-order-basic';

export class SaleOrderLineDisplay {
    priceUnit: number;
    product: ProductSimple;
    productId: string;
    teeth: ToothDisplay[];
    toothCategory: ToothCategoryBasic;
    state: string;
    qtyInvoiced: number;
    orderId: string;
    Order: SaleOrderBasic;
}