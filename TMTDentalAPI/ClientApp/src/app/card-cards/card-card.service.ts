import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { PagedResult2 } from '../core/paged-result-2';
import { PartnerSimple } from '../partners/partner-simple';
import { CardTypeBasic } from '../card-types/card-type.service';

export class CardCardPaged {
    limit: number;
    offset: number;
    search: string;
    partnerId: string;
    barcode: string;
    state: string;
    isExpired: boolean;
}

export class CardCardBasic {
    id: string;
    name: string;
}

export class CardCardDisplay {
    id: string;
    name: string;
    state: string;
    partner: PartnerSimple;
    type: CardTypeBasic;
    activatedDate: string;
    expiredDate: string;
    isExpired: boolean;
    upgradeTypeId: string;
}

@Injectable({providedIn: 'root'})
export class CardCardService {
    apiUrl = 'api/CardCards';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any): Observable<PagedResult2<CardCardBasic>> {
        return this.http.get<PagedResult2<CardCardBasic>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
    }

    get(id): Observable<CardCardDisplay> {
        return this.http.get<CardCardDisplay>(this.baseApi + this.apiUrl + "/" + id);
    }

    create(val: CardCardDisplay): Observable<CardCardDisplay> {
        return this.http.post<CardCardDisplay>(this.baseApi + this.apiUrl, val);
    }

    update(id: string, val: CardCardDisplay) {
        return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
    }

    buttonConfirm(ids) {
        return this.http.post(this.baseApi + this.apiUrl + '/ButtonConfirm', ids);
    }

    buttonActive(ids) {
        return this.http.post(this.baseApi + this.apiUrl + '/ButtonActive', ids);
    }

    buttonCancel(ids) {
        return this.http.post(this.baseApi + this.apiUrl + '/ButtonCancel', ids);
    }

    buttonReset(ids) {
        return this.http.post(this.baseApi + this.apiUrl + '/ButtonReset', ids);
    }

    buttonRenew(ids) {
        return this.http.post(this.baseApi + this.apiUrl + '/ButtonRenew', ids);
    }

    buttonLock(ids) {
        return this.http.post(this.baseApi + this.apiUrl + '/ButtonLock', ids);
    }

    buttonUnlock(ids) {
        return this.http.post(this.baseApi + this.apiUrl + '/ButtonUnlock', ids);
    }

    buttonUpgrade(ids) {
        return this.http.post(this.baseApi + this.apiUrl + '/ButtonUpgradeCard', ids);
    }

    excelServerExport(paged) {
        return this.http.get(this.baseApi + this.apiUrl + '/ExportExcelFile', { responseType: 'blob', params: paged });
    }

}