export class IRModelBasic {
    id: string;
    name: string;
}

export class IRModelPaging {
    offset: number;
    limit: number;
    totalItems: number;
    items: IRModelBasic[];
}

export class IRModelPaged {
    limit: number;
    offset: number;
    filter: string;
}