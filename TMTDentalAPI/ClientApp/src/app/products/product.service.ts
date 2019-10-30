import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { Product } from './product';
import { ProductPaging } from './product-paging';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { map } from 'rxjs/operators';
import { ProductSimple } from './product-simple';
import { ProductStepDisplay, ProductDisplayAndStep } from './product-step';
import { PagedResult2 } from '../core/paged-result-2';

export class ProductFilter {
    search: string;
    keToaOK: boolean;
    purchaseOK: boolean;
    saleOK: boolean;
    limit: number;
    offset: number;
    type: string;
}

export class ProductPaged {
    search: string;
    limit: number;
    offset: number;
    categId: string;
    saleOK: boolean;
    purchaseOK: boolean;
    keToaOK: boolean;
    isLabo: boolean;
    type: string;
    type2: string;
}

export class ProductBasic2 {
    id: string;
    name: string;
    categName: string;
    listPrice: string;
    type: string;
    defaultCode: string;
    qtyAvailable: number;
}

export class ProductPaging2 {
    pageNumber: number;
    pageSize: number;
    totalPages: number;
    totalItems: number;
    items: ProductBasic2[];
}

export class ProductImportExcelViewModel {
    fileBase64: string;
}

export class ProductLaboBasic {
    id: string;
    name: string;
    purchasePrice: number;
}

export class ProductLaboSave {
    name: string;
    purchasePrice: number;
}

export class ProductLaboDisplay {
    id: string;
    name: string;
    purchasePrice: number;
}

@Injectable()
export class ProductService {
    apiUrl = 'api/products';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getGridData(pageIndex: number, pageSize: number): Observable<GridDataResult> {
        let params = new HttpParams()
            .set('pageNumber', (pageIndex + 1).toString())
            .set('pageSize', pageSize.toString());
        return this.http
            .get<ProductPaging>(this.apiUrl, { params })
            .pipe(
                map(response => (<GridDataResult>{
                    data: response.items,
                    total: response.totalItems
                }))
            );
    }

    getPaged(val: any): Observable<ProductPaging2> {
        return this.http.get<ProductPaging2>(this.baseApi + this.apiUrl, { params: val });
    }

    get(id: string): Observable<Product> {
        return this.http.get<Product>(this.baseApi + this.apiUrl + "/" + id);
    }

    defaultGet(): Observable<Product> {
        return this.http.post<Product>(this.baseApi + this.apiUrl + "/defaultget", {});
    }

    defaultProductStepGet(): Observable<Product> {
        return this.http.post<Product>(this.baseApi + this.apiUrl + "/DefaultProductStepGet", {});
    }

    create(product: Product) {
        return this.http.post(this.baseApi + this.apiUrl, product);
    }


    update(id: string, product: Product) {
        return this.http.put(this.baseApi + this.apiUrl + "/" + id, product);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
    }

    autocomplete(filter: string)
        : Observable<ProductSimple[]> {
        let params = new HttpParams()
            .set('filter', filter);
        return this.http.get<ProductSimple[]>(this.baseApi + this.apiUrl + "/autocomplete", { params });
    }

    autocomplete2(val: ProductFilter): Observable<Product[]> {
        return this.http.post<Product[]>(this.baseApi + this.apiUrl + "/autocomplete2", val);
    }


    importExcel(val: ProductImportExcelViewModel) {
        return this.http.post(this.baseApi + this.apiUrl + "/ImportExcel", val);
    }

    getLaboPaged(val: any): Observable<PagedResult2<ProductLaboBasic>> {
        return this.http.get<PagedResult2<ProductLaboBasic>>(this.baseApi + this.apiUrl + '/GetLaboPaged', { params: new HttpParams({ fromObject: val }) });
    }

    getLabo(id: string): Observable<ProductLaboDisplay> {
        return this.http.get<ProductLaboDisplay>(this.baseApi + this.apiUrl + "/" + id + '/GetLabo');
    }

    createLabo(product: ProductLaboSave): Observable<ProductLaboBasic> {
        return this.http.post<ProductLaboBasic>(this.baseApi + this.apiUrl + '/CreateLabo', product);
    }

    updateLabo(id: string, product: ProductLaboSave) {
        return this.http.put(this.baseApi + this.apiUrl + "/" + id + "/UpdateLabo", product);
    }

    //==============================PHẦN CÔNG ĐOẠN - STEP=====================================//
    createWithSteps(product: ProductDisplayAndStep) {
        return this.http.post(this.baseApi + this.apiUrl + "/CreateWithSteps", product);
    }

    getStepByProductId(id): Observable<ProductStepDisplay[]> {
        return this.http.get<ProductStepDisplay[]>(this.baseApi + "api/ProductSteps/" + id);
    }

    updateWithSteps(id: string, product: ProductDisplayAndStep) {
        return this.http.post(this.baseApi + this.apiUrl + "/UpdateWithSteps/" + id, product);
    }
}