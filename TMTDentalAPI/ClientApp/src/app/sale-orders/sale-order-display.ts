import { UserSimple } from '../users/user-simple';
import { SaleOrderLineDisplay } from './sale-order-line-display';
import { PartnerSimple } from '../partners/partner-simple';
import { ProductPriceListBasic } from '../price-list/price-list';
import { SaleOrderBasic } from './sale-order-basic';
import { EmployeeSimple } from '../employees/employee';

export class SaleOrderDisplay {
    id: string;
    partner: PartnerSimple;
    partnerId: string;
    dateOrder: string;
    user: UserSimple;
    employee:EmployeeSimple;
    employeeId:string;
    companyId: string;
    userId: string;
    state: string;
    residual: number;
    orderLines: SaleOrderLineDisplay[];
    name: string;
    pricelist: ProductPriceListBasic;
    invoiceStatus: string;
    quote: SaleOrderBasic;
    order: SaleOrderBasic;
    invoiceCount: number;
}