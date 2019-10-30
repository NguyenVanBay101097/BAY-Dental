import { PartnerSimple } from '../partners/partner-simple';

import { UserSimple } from '../users/user-simple';

export class DotKhamDefaultGet {
    invoiceId: string;
    saleOrderId: string;
}

export class DotKhamBasic {
    id: string;
    name: string;
    date: string;
    partnerId: string;
    partner: object;
    doctorId: string;
    doctor: object;
    userId: string;
    user: object;
    note: string;
    appointmentId: string;
}

export class DotKhamPaged {
    offset: number;
    limit: number;
    search: string;
}

export class DotKhamDisplay {
    id: string;
    name: string;
    invoiceId: string;
    invoice: object;
    partnerId: string;
    partner: object;
    date: string;
    doctorId: string;
    doctor: PartnerSimple;
    assistantId: string;
    assistant: PartnerSimple;
    userId: string;
    user: UserSimple;
    note: string;
    state: string;
    companyId: string;
    lines: []
}

export class DotKhamPaging {
    offset: number;
    limit: number;
    totalItems: number;
    items: DotKhamBasic[];
}

export class DotKhamStepDisplay {
    id: string;
    name: string;
    state: string;
    order: number;
    isInclude: boolean;
    productId: string;
    dotKhamId: string;
    invoicesId: string;
}

export class DotKhamStepSave {
    dotKhamId: string;
    isDone: boolean;
    state: string;
}

export class DotKhamPatch {
    dotKhamId: string;
    appointmentId: string;
}