import { EmployeeSimple } from '../employees/employee';
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
    employee: EmployeeSimple;
    employeeId: string;
    state: string;
    qtyInvoiced: number;
    orderId: string;
    Order: SaleOrderBasic;
    amountPaid:number;
    amountResidual:number;
    id?: string;
}
