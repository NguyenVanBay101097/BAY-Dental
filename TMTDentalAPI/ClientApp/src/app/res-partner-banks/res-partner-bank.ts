export class ResPartnerBankBasic {
    id: string;
    accountNumber: string;
    bankId: string;
}

export class ResPartnerBankPaged {
    offset: number;
    limit: number;
    search: string;
    partnerId: string;
    bankId: string;
}
