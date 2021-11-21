export class InsuranceDebtFilter {
    dateFrom: string;
    dateTo: string;
    search: string;
    insuranceId: string;
}

export class InsuranceDebtReport {
    partnerName: string;
    date: string;
    amountTotal: number;
    origin: string;
    moveId: string;
    moveType: string;
}