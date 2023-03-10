import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AccountPaymentBasic } from 'src/app/account-payments/account-payment.service';
import { SaleOrderSimple } from './../../quotations/quotation.service';

export class SaleOrderPaymentPaged {
  offset: number;
  limit: number;
  saleOrderId: string;
  companyId: string;
}

export class SaleOrderPaymentPagging {
  limit: number;
  offset: number;
  totalItems: number;
  items: SaleOrderPaymentBasic[];
}

export class HistoryPartnerAdvanceFilter {
  offset: number;
  limit: number;
  dateFrom: string;
  dateTo: string;
  search: string;
  partnerId: string;
}

export class HistoryPartnerAdvancePagging {
  limit: number;
  offset: number;
  totalItems: number;
  items: HistoryPartnerAdvanceResult[];
}

export class HistoryPartnerAdvanceResult {
  paymentName: string;
  paymentDate: Date;
  paymentAmount: number;
  orders: SaleOrderSimple;
}

export class SaleOrderPaymentBasic {
  id: string;
  amount: number;
  date: Date;
  note: string;
  payments: AccountPaymentBasic[];
  state: string;
}

export class SaleOrderPaymentDisplay {
  id: string;
  amount: number;
  date: Date;
  orderId: string;
  companyId: string;
  journalLines: any[];
  lines: any[];
  note: string;
  state: string;
}

export class SaleOrderPaymentSave {
  amount: number;
  date: Date;
  orderId: string;
  companyId: string;
  journalLines: any[];
  lines: any[];
  note: string;
  state: string;
}

export class RegisterSaleOrderPayment {
  amount: number;
  date: number;
  orderId: number;
  companyId: number;
  journalLines: any[];
  Lines: any[];
  note: string;
  state: string;
}

@Injectable({
  providedIn: 'root'
})
export class SaleOrderPaymentService {
  apiUrl = 'api/SaleOrderPayments';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  getPaged(val): Observable<SaleOrderPaymentPagging> {
    return this.http.get<SaleOrderPaymentPagging>(this.baseApi + this.apiUrl, { params: val });
  }

  getHistoryPartnerAdvance(val): Observable<HistoryPartnerAdvancePagging> {
    return this.http.get<HistoryPartnerAdvancePagging>(this.baseApi + this.apiUrl + "/GetHistoryPartnerAdvance", { params: val });
  }

  get(id): Observable<SaleOrderPaymentDisplay> {
    return this.http.get<SaleOrderPaymentDisplay>(this.baseApi + this.apiUrl + "/" + id);
  }

  getDefault() {
    return this.http.get(this.baseApi + this.apiUrl + '/DefaultGet');
  }

  create(val): Observable<SaleOrderPaymentDisplay> {
    return this.http.post<SaleOrderPaymentDisplay>(this.baseApi + this.apiUrl, val);
  }

  actionCancel(val: string[]) {
    return this.http.post(this.baseApi + this.apiUrl + "/ActionCancel", val);
  }

  getPrint(id) {
    return this.http.get(this.baseApi + this.apiUrl + '/' + id + '/GetPrint');
  }

  actionPayment(val: string[]) {
    return this.http.post(this.baseApi + this.apiUrl + "/ActionPayment", val);
  }

  // getPrint(id) {
  //   return this.http.get(this.baseApi + "SaleOrderPayment/Print?id=" + id, { responseType: 'text' });
  // }



}
