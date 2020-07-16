import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { Injectable, Inject } from '@angular/core';

export class PartnerCategoryBasic {
    id: string;
    name: string;
    completeName: string;
}

export class PartnerCategoryDisplay {
    id: string;
    name: string;
    parentId: string;
    parent: PartnerCategoryBasic;
}

export class PartnerCategoryPaged {
    offset: number;
    limit: number;
    search: string;
    partnerId: string;
}

export class PartnerCategoryPaging {
    offset: number;
    limit: number;
    totalItems: number;
    items: PartnerCategoryBasic[];
}

@Injectable()
export class PartnerCategoryService {
    apiUrl = 'api/partnercategories';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any): Observable<PartnerCategoryPaging> {
        return this.http.get<PartnerCategoryPaging>(this.baseApi + this.apiUrl, { params: val });
    }

    create(category: PartnerCategoryDisplay) {
        return this.http.post(this.baseApi + this.apiUrl, category);
    }

    update(id: string, category: PartnerCategoryDisplay) {
        return this.http.put(this.baseApi + this.apiUrl + '/' + id, category);
    }

    get(id: string) {
        return this.http.get<PartnerCategoryDisplay>(this.baseApi + this.apiUrl + '/' + id);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + '/' + id);
    }

    autocomplete(val: PartnerCategoryPaged): Observable<PartnerCategoryBasic[]> {
        return this.http.post<PartnerCategoryBasic[]>(this.baseApi + this.apiUrl + '/autocomplete', val);
    }

    actionImport(val) {
        return this.http.post(this.baseApi + this.apiUrl + '/ActionImport', val);
    }
}