import { UserSimple } from '../users/user-simple';
import { SaleOrderLineDisplay } from './sale-order-line-display';

export class SaleOrderDisplay {
    id: string;
    partner: object;
    partnerId: string;
    dateOrder: string;
    user: UserSimple;
    companyId: string;
    userId: string;
    state: string;
    residualSum: number;
    orderLines: SaleOrderLineDisplay[];
}