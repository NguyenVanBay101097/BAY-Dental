export class PagedResult2<T> {
    limit: number;
    offset: number;
    totalItems: number;
    items: T[];
}