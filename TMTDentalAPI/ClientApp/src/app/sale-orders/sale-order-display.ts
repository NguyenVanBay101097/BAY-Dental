import { PartnerDisplay } from './../partners/partner-simple';
import { AccountJournalSimple } from 'src/app/account-journals/account-journal.service';
import { UserSimple } from '../users/user-simple';
import { PartnerSimple } from '../partners/partner-simple';
import { ProductPriceListBasic } from '../price-list/price-list';
import { SaleOrderBasic } from './sale-order-basic';
import { EmployeeSimple } from '../employees/employee';
import { ProductSimple } from '../products/product-simple';
import { ToothDisplay } from '../teeth/tooth.service';
import { ToothCategoryBasic } from '../tooth-categories/tooth-category.service';
import { SaleOrderLineDisplay } from './sale-order-line-display';

export class SaleOrderDisplay {
    id: string;
    partner: PartnerSimple;
    partnerId: string;
    dateOrder: string;
    user: UserSimple;
    employee: EmployeeSimple;
    employeeId: string;
    companyId: string;
    userId: string;
    state: string;
    residual: number;
    orderLines: SaleOrderLineDisplay[];
    name: string;
    paid: number;
    note: string;
    pricelist: ProductPriceListBasic;
    journalId: string;
    journal: AccountJournalSimple;
    invoiceStatus: string;
    quote: SaleOrderBasic;
    order: SaleOrderBasic;
    invoiceCount: number;
    paidTotal: number;
    promotions: any[];
    totalPaid: number;
}

export class SaleOrderDisplayVm {
    id: string;
    partner: any;
    partnerId: string;
    dateOrder: string;
    user: UserSimple;
    employee: EmployeeSimple;
    employeeId: string;
    companyId: string;
    userId: string;
    state: string;
    residual: number;
    orderLines: any[];
    dotKhams: any[];
    name: string;
    paid: number;
    pricelist: ProductPriceListBasic;
    journalId: string;
    journal: AccountJournalSimple;
    invoiceStatus: string;
    quote: SaleOrderBasic;
    order: SaleOrderBasic;
    invoiceCount: number;
    paidTotal: number;
}