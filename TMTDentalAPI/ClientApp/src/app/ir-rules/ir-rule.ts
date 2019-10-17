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