export class ResBankBasic {
    id: string;
    name: string;
    bic: string;
}

export class ResBankPaged {
    offset: number;
    limit: number;
    search: string;
}
