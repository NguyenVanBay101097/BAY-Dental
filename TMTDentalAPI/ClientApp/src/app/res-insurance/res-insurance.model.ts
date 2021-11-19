export class ResInsuranceSave {
    name: string;
    date: string;
    avatar: string;
    representative: string;
    phone: string;
    email: string;
    address: string;
    note: string;
}

export class ResInsuranceSimple {
    id: string;
    name: string;
}

export class ResInsuranceDisplay {
    id: string;
    name: string;
    date: string;
    avatar: string;
    representative: string;
    phone: string;
    email: string;
    address: string;
    note: string;
}

export class ResInsuranceBasic {
    id: string;
    name: string;
    phone: string;
    totalDebt: number;
    isActive: boolean;
}

export class ResInsurancePaged {
    limit: number;
    offset: number;
    search: string;
    isActive: string;
}

export class InsuranceIsActivePatch {
    isActive: string;
}

export class ResInsurancePaymentBasic {
    id: string;
    amount: number;
    date: string;
    note: string;
}

export class ResInsurancePaymentSave {
    amount: number;
    date: string;
    resInsuranceId: string;
    orderId: string;
    companyId: string;
    lines: any[];
    note: string;
}