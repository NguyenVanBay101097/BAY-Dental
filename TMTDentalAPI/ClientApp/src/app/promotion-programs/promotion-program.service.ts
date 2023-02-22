import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { PagedResult2 } from '../core/paged-result-2';
import { CompanySimple } from '../companies/company.service';
import { ProductCategoryBasic } from '../product-categories/product-category.service';
import { Product } from '../products/product';

export class PromotionProgramPaged {
    limit: number;
    offset: number;
    search: string;
}

export class PromotionProgramBasic {
    id: string;
    name: string;
}

export class PromotionProgramDisplay {
    id: string;
    name: string;
    dateFrom: string;
    dateTo: string;
    maximumUseNumber: number;
    companies: CompanySimple[];
    rules: PromotionProgramRuleDisplay[];
}


export class PromotionProgramRuleDisplay {
    id: string;
    minQuantity: number;
    discountType: string;
    discountPercentage: number;
    discountFixedAmount: number;
    discountApplyOn: string;
    categories: ProductCategoryBasic[];
    products: Product[];
}

export class PromotionProgramSave {
    id: string;
    name: string;
}

@Injectable()
export class PromotionProgramService {
    apiUrl = 'api/PromotionPrograms';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any): Observable<PagedResult2<PromotionProgramBasic>> {
        return this.http.get<PagedResult2<PromotionProgramBasic>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
    }

    get(id): Observable<PromotionProgramDisplay> {
        return this.http.get<PromotionProgramDisplay>(this.baseApi + this.apiUrl + "/" + id);
    }

    create(val: PromotionProgramSave): Observable<PromotionProgramBasic> {
        return this.http.post<PromotionProgramBasic>(this.baseApi + this.apiUrl, val);
    }

    update(id: string, val: PromotionProgramSave) {
        return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
    }

    toggleActive(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + "/ToggleActive", ids);
    }

}