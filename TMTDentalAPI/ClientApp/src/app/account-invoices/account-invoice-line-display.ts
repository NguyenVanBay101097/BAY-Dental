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


export class AccountInvoiceLinePaged {
    offset: number;
    limit: number;
    invoiceId: string;
    productId: string;
    accountId: string;
    partnerId: string;
    toothCategoryId: string;
    diagnostic: string;
}


