export class InsuranceDebtFilter {
    dateFrom: string;
    dateTo: string;
    search: string;
    insuranceId: string;
}

export class InsuranceDebtDetailFilter {
    dateFrom: string;
    dateTo: string;
    paymentId: string;
}

export class InsuranceDebtDetailItem {
    name: string;
    amount: number;
}

export class InsuranceHistoryInComeFilter {
    limit: number;
    offset: number;
    dateFrom: string;
    dateTo: string;
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

export class InsuranceReportFilter {
    dateFrom: string;
    dateTo: string;
    search: string;
    companyId: string;
}

export class InsuranceReportItem {
    partnerId: string;
    partnerName: string;
    partnerRef: string;
    partnerPhone: string;
    begin: number
    debit: number;
    credit: number;
    end: number;
    dateFrom: string;
    dateTo: string;
    companyId: string;
}
export class InsuranceReportDetailFilter {
    dateFrom: string;
    dateTo: string;
    partnerId: string;
    companyId: string;
}

export class InsuranceHistoryInComeItem {
    id: string;
    partnerName: string;
    paymentDate: string;
    journalName: string;
    state: string;
    name: string;
    amount: number;
    communication: string;
    dateFrom: string;
    dateTo: string;
}

export class InsuranceHistoryInComeDetailFilter {
    paymentId: string;
}

export class InsuranceHistoryInComeDetailItem {
    id: string;
    date: string;
    partnerName: string;
    ref: string;
    refDisplay: string;
    amount: number;
}