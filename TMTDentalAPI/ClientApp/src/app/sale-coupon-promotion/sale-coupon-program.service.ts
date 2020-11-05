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
    ids: string[];
}

export class SaleCouponProgramBasic {
    id: string;
    name: string;
}

export class SaleCouponProgramDisplay {
    id: string;
    name: string;
    couponCount: number;
    orderCount: number;
    rewardProduct: ProductSimple;
    ruleDateFrom: string;
    ruleDateTo: string;
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
        const headerSettings: { [name: string]: string | string[]; } = {};
        headerSettings['Content-Type'] = 'application/json';
        headerSettings['Authorization'] = 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIxNmU2NDE5Zi1iMWRhLTQwZDctYTYxMC0xOGM2OTQxOGYzZTMiLCJ1bmlxdWVfbmFtZSI6IjEyMzEyMyIsImNvbXBhbnlfaWQiOiIwY2VkYWFmZC0wNjFiLTRkZTUtZDg3Zi0wOGQ4N2FlZDVhNDkiLCJ1c2VyX3Jvb3QiOiJUcnVlIiwibmJmIjoxNjA0NDg3OTQ0LCJleHAiOjE2MDUwOTI3NDQsImlhdCI6MTYwNDQ4Nzk0NH0.V2nxhWIDWX5w_EyjXCOuLcK2u9fUBW_ca4KHtq7kR6Q';
        const newHeader = new HttpHeaders(headerSettings);
        return this.http.get<PagedResult2<SaleCouponProgramBasic>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }), headers:newHeader });
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