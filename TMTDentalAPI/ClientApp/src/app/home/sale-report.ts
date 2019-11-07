export class SaleReportTopServicesCs {
    productId: string;
    productName: string;
    priceTotalSum: number;
    productUOMQtySum: number;
}

export class SaleReportTopServicesFilter {
    number: number;
    byInvoice: boolean;
    byQty: boolean;
    companyId: string;
    state: string;
    categId: string;
    partnerId: string;
    dateFrom: string;
    dateTo: string;
}
