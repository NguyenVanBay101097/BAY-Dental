import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

export class SaleOrderCustomerDebtPaymentSave {
  amount: number;
  date: string;
  orderId: string;
  companyId: string;
  state: string;
  journalLines: any[];
  lines: any[];
  note: string;
  isDebtPayment: boolean;
  debtJournalId: string;
  debtAmount: number;
  debNote: string;
  partnerId: string;
}


@Injectable({
  providedIn: 'root'
})
export class PaymentService {

  apiUrl = 'api/Payments';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  paymentSaleOrderAndDebt(val: SaleOrderCustomerDebtPaymentSave) {
    return this.http.post(this.baseApi + this.apiUrl, val);
  }
}
