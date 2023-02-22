export class IRRuleBasic {
    id: string;
    name: string;
}

export class IRRulePaging {
    offset: number;
    limit: number;
    totalItems: number;
    items: IRRuleBasic[];
}

export class IRRulePaged {
    limit: number;
    offset: number;
    filter: string;
}