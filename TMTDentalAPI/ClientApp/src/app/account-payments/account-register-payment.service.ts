import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export class AccountRegisterPaymentDefaultGet {
    invoiceIds: string[];
}

export class AccountRegisterPaymentDisplay {
    id: string;
    paymentDate: string;
    communication: string;
    journalId: string;
    partnerType: string;
    amount: number;
    paymentType: string;
    partnerId: string;
    invoiceIds: string[];
    debitItems: []
}

export class AccountRegisterPaymentCreatePayment {
    id: string;
}

@Injectable({ providedIn: 'root' })
export class AccountRegisterPaymentService {
    apiUrl = 'api/accountregisterpayments';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    defaultGet(val: AccountRegisterPaymentDefaultGet): Observable<AccountRegisterPaymentDisplay> {
        return this.http.post<AccountRegisterPaymentDisplay>(this.baseApi + this.apiUrl + "/defaultget", val);
    }

    saleOrdersDefaultGet(ids: string[]): Observable<AccountRegisterPaymentDisplay> {
        return this.http.post<AccountRegisterPaymentDisplay>(this.baseApi + this.apiUrl + "/SaleOrdersDefaultGet", ids);
    }

    create(val: AccountRegisterPaymentDisplay): Observable<AccountRegisterPaymentDisplay> {
        return this.http.post<AccountRegisterPaymentDisplay>(this.baseApi + this.apiUrl, val);
    }

    createPayment(val: AccountRegisterPaymentCreatePayment) {
        return this.http.post(this.baseApi + this.apiUrl + "/createpayment", val);
    }
}