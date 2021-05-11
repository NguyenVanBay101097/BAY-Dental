import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { PagedResult2 } from '../core/paged-result-2';
import { PartnerSimple } from '../partners/partner-simple';
import { CardTypeBasic } from '../card-types/card-type.service';
import { ProductSimple } from '../products/product-simple';

export class SaleCouponProgramPaged {
    limit: number;
    offset: number;
    search: string;
    programType: string;
    active: boolean;
    status: string;
    ids: string[];
}

export class SaleCouponProgramBasic {
    id: string;
    name: string;
}

export class SaleCouponProgramDisplay {
    id: string;
    name: string;
    active: boolean;
    couponCount: number;
    orderCount: number;
    rewardProduct: ProductSimple;
    ruleDateFrom: string;
    ruleDateTo: string;
    days: string;
    notIncremental: boolean;
    saleOrderMinimumAmount: number;
    discountSpecificProducts: any;
    discountSpecificProductCategories: any;
}

export class SaleCouponProgramSave {
    id: string;
    name: string;
}

@Injectable()
export class SaleCouponProgramService {
    apiUrl = 'api/SaleCouponPrograms';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any): Observable<PagedResult2<SaleCouponProgramBasic>> {
        return this.http.get<PagedResult2<SaleCouponProgramBasic>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
    }

    get(id): Observable<SaleCouponProgramDisplay> {
        return this.http.get<SaleCouponProgramDisplay>(this.baseApi + this.apiUrl + "/" + id);
    }

    create(val: SaleCouponProgramSave): Observable<SaleCouponProgramBasic> {
        return this.http.post<SaleCouponProgramBasic>(this.baseApi + this.apiUrl, val);
    }

    update(id: string, val: SaleCouponProgramSave) {
        return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
    }

    unlink(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + "/Unlink", ids);
    }

    toggleActive(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + "/ToggleActive", ids);
    }

    actionArchive(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + "/ActionArchive", ids);
    }

    actionUnArchive(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + "/ActionUnArchive", ids);
    }

    generateCoupons(data: any) {
        return this.http.post(this.baseApi + this.apiUrl + "/GenerateCoupons", data);
    }
}