export class SaleOrderBasic {
    id: string;
    name: string;
    type: string;
    state: string;
    dateInvoice: Date;
    partner: object;
    amountTotal: number;
    residual: number;
    number: string;
    user: object;
    dateOrder: Date;
    dateDone: Date;
    totalPaid: number;
}