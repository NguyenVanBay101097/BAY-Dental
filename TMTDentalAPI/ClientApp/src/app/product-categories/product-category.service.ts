import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { map } from 'rxjs/operators';

export class ProductCategoryFilter {
    sort: string;
    order: string;
    pageIndex: number;
    pageSize: number;
    filter: string;
}
export class ProductCategoryImportExcelBaseViewModel {
    fileBase64: string;
    type: string;
}

export class ProductCategoryBasic {
    id: string;
    name: string;
    completeName: string;
}

export class ProductCategoryDisplay {
    id: string;
    name: string;
    parentId: string;
    parent: ProductCategoryBasic;
    sequence: number;
    serviceCateg: boolean;
    laboCateg: boolean;
    productCateg: boolean;
    medicineCateg: boolean;
}

export class ProductCategoryPaged {
    offset: number;
    limit: number;
    search: string;
    serviceCateg: boolean;
    laboCateg: boolean;
    productCateg: boolean;
    medicineCateg: boolean;
    type: string;
}

export class ProductCategoryPaging {
    offset: number;
    limit: number;
    totalItems: number;
    items: ProductCategoryBasic[];
}

@Injectable({ providedIn: 'root' })
export class ProductCategoryService {
    apiUrl = 'api/productcategories';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any): Observable<ProductCategoryPaging> {
        return this.http.get<ProductCategoryPaging>(this.baseApi + this.apiUrl, { params: val });
    }

    search(filter: ProductCategoryFilter)
        : Observable<ProductCategoryPaging> {
        let params = new HttpParams()
            .set('pageNumber', (filter.pageIndex + 1).toString())
            .set('pageSize', filter.pageSize.toString());
        if (filter.sort) {
            params = params.append('orderBy', filter.sort);
            params = params.append('orderDirection', filter.order);
        }
        if (filter.filter) {
            params = params.append('filter', filter.filter);
        }
        return this.http.get<ProductCategoryPaging>(this.baseApi + this.apiUrl, { params });
    }

    create(category: ProductCategoryDisplay) {
        return this.http.post(this.baseApi + this.apiUrl, category);
    }

    update(id: string, category: ProductCategoryDisplay) {
        return this.http.put(this.baseApi + this.apiUrl + '/' + id, category);
    }

    get(id: string) {
        return this.http.get<ProductCategoryDisplay>(this.baseApi + this.apiUrl + '/' + id);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + '/' + id);
    }

    autocomplete(val: ProductCategoryPaged): Observable<ProductCategoryBasic[]> {
        return this.http.post<ProductCategoryBasic[]>(this.baseApi + this.apiUrl + '/autocomplete', val);
    }

    importExcel(val: ProductCategoryImportExcelBaseViewModel) {
        return this.http.post(this.baseApi + this.apiUrl + "/ImportExcel", val);
    }
}