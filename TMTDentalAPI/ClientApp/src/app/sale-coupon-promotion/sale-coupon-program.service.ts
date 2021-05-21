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
    discountApplyOn: string;
    promoCodeUsage: string;
    productId: string;
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
    discountType: string;
    discountPercentage: number;
    discountFixedAmount: number;
    days: string;
    notIncremental: boolean;
    saleOrderMinimumAmount: number;
    discountSpecificProducts: any;
    discountSpecificProductCategories: any;
    statusDisplay: string;
    promoCode: string;
}

export class SaleCouponProgramSave {
    id: string;
    name: string;
}

export class SaleCouponProgramGetListPagedRequest {
    limit: number;
    offset: number;
    search: string;
    programType: string;
    active: boolean;
    status: string;
}

export class SaleCouponProgramGetListPagedResponse {
    id: string;
    name: string;
    ruleDateFrom: string;
    ruleDateTo: string;
    maximumUseNumber: number;
    active: boolean;
    isPaused: boolean;
    statusDisplay: string;
}

@Injectable()
export class SaleCouponProgramService {
    apiUrl = 'api/SaleCouponPrograms';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getListPaged(val: any): Observable<PagedResult2<SaleCouponProgramGetListPagedResponse>> {
        return this.http.get<PagedResult2<SaleCouponProgramGetListPagedResponse>>(this.baseApi + this.apiUrl + "/GetListPaged", { params: new HttpParams({ fromObject: val }) });
    }

    getPaged(val: any): Observable<PagedResult2<SaleCouponProgramBasic>> {
        return this.http.get<PagedResult2<SaleCouponProgramBasic>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
    }

    get(id): Observable<SaleCouponProgramDisplay> {
        return this.http.get<SaleCouponProgramDisplay>(this.baseApi + this.apiUrl + "/" + id);
    }

    create(val: any): Observable<SaleCouponProgramBasic> {
        return this.http.post<SaleCouponProgramBasic>(this.baseApi + this.apiUrl, val);
    }

    update(id: string, val: any) {
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

    getPromotionUsageCode(code: string,productId?: string): Observable<any> {
        return this.http.get<any>(this.baseApi + this.apiUrl + '/GetPromotionUsageCode?code='+ code + (productId? '&productId='+ productId : ''));
    }

    getPromotionBySaleOrder(partnerId) {
        return this.http.get(this.baseApi + this.apiUrl + "/GetPromotionBySaleOrder?partnerId="+partnerId);
    }

    getPromotionBySaleOrderLine(id, partnerId) {
        return this.http.get(this.baseApi + this.apiUrl + "/GetPromotionBySaleOrderLine?productId=" + id + '&partnerId='+partnerId);
    }

    getAmountTotalUsagePromotion(id){
        return this.http.get(this.baseApi + this.apiUrl +'/'+id+ "/GetAmountTotalUsagePromotion");
    }

    defaultGet(programType: string){
        return this.http.get(this.baseApi + this.apiUrl + '/DefaultGet?programType=' + programType);
    }

    checkPromoCodeExist(code: any){
        return this.http.get(this.baseApi + this.apiUrl + '/CheckPromoCodeExist?code=' + code);
    }
}