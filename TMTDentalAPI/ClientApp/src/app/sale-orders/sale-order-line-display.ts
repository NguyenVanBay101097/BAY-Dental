import { EmployeeSimple } from '../employees/employee';
import { ProductSimple } from '../products/product-simple';
import { ToothDisplay } from '../teeth/tooth.service';
import { ToothCategoryBasic } from '../tooth-categories/tooth-category.service';
import { SaleOrderBasic } from './sale-order-basic';

export class SaleOrderLineDisplay {
    priceUnit: number;
    product: ProductSimple;
    productId: string;
    productUOMQty: number;
    teeth: ToothDisplay[];
    name: string;
    toothCategory: ToothCategoryBasic;
    toothCategoryId: string;
    toothIds: string[];
    employee: EmployeeSimple;
    employeeId: string;
    state: string;
    diagnostic: string;
    qtyInvoiced: number;
    orderId: string;
    discount: number;
    discountType: string;
    Order: SaleOrderBasic;
    amountPaid: number;
    amountResidual: number;
    id?: string;
    priceSubTotal: number;
    assistant: EmployeeSimple;
    counselor: EmployeeSimple;
    toothType: string;
    productIsLabo: boolean;
    isActive: boolean;
    promotions: any[];
    amountInvoiced: number;
    amountDiscountTotal: number;
}
