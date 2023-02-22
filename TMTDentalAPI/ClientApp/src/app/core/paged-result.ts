export class PagedResult<T> {
    pageNumber: number;
    pageSize: number;
    totalPages: number;
    totalItems: number;
    items: T[];
}