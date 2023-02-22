import { ResBankSimple } from "../res-banks/res-bank.service";

export class ResPartnerBankBasic {
    id: string;
    accountNumber: string;
    bankId: string;
    accountHolderName: string;
    branch: string;
    bank: ResBankSimple;
}

export class ResPartnerBankPaged {
    offset: number;
    limit: number;
    search: string;
    partnerId: string;
    bankId: string;
}
