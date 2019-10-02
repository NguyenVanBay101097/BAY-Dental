import { ToothDisplay } from '../teeth/tooth.service';
import { ToothCategoryBasic } from '../tooth-categories/tooth-category.service';
import { ProductSimple } from '../products/product-simple';
import { PartnerSimple } from '../partners/partner-simple';

export class AccountInvoiceLineDisplay {
    id: string;
    name: string;
    uomId: string;
    product: ProductSimple;
    productId: string;
    employee: PartnerSimple;
    employeeId: string;
    accountId: string;
    priceUnit: number;
    priceSubTotal: number;
    discount: number;
    quantity: number;
    teeth: ToothDisplay[];
    toothCategory: ToothCategoryBasic;
}

