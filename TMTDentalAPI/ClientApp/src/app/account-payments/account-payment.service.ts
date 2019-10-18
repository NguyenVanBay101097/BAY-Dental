import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { PagedResult2 } from '../core/paged-result-2';

export class AccountPaymentBasic {
    id: string;
    partnerName: string;
    paymentDate: string;
    journalName: string;
    state: string;
    name: string;
    amount: number;
}

export class AccountPaymentPaged {
    offset: number;
    limit: number;
    search: string;
    partnerType: string;
    state: string;
    paymentDateFrom: string;
    paymentDateTo: string;
}

export class AccountPaymentDisplay {
    id: string;
    partnerId: string;
    partner: Object;
    partnerType: string;
    paymentDate: string;
    journalId: string;
    journal: Object;
    state: string;
    name: string;
    paymentType: string;
    amount: number;
    communication: string;
}

@Injectable()
export class AccountPaymentService {
    apiUrl = 'api/AccountPayments';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any): Observable<PagedResult2<AccountPaymentBasic>> {
        return this.http.get<PagedResult2<AccountPaymentBasic>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
    }

    get(id: string): Observable<AccountPaymentDisplay> {
        return this.http.get<AccountPaymentDisplay>(this.baseApi + this.apiUrl + '/' + id);
    }

    actionCancel(ids: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/ActionCancel', ids);
    }

    unlink(ids: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/Unlink', ids);
    }
}