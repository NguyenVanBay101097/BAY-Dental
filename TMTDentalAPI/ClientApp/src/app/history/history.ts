export class HistorySimple {
    id: string;
    name: string;
}

export class HistoryPaged {
    offset: number;
    limit: number;
    search: string;
}

export class PagedResult2<T>{
    offset: number;
    limit: number;
    totalItems: number;
    items: T[];
}
