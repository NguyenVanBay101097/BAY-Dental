import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedResult2 } from 'src/app/employee-categories/emp-category';

export class SaleOrderPaymentPaged {
  offset: number;
  limit: number;
  saleOrderId: string;
}

export class SaleOrderPaymentPagging {
  limit: number;
  offset: number;
  totalItems: number;
  items: SaleOrderPaymentBasic[];
}

export class SaleOrderPaymentBasic {
  id: string;
  amount: number;
  date: Date;
  note: string;
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

  get(id): Observable<SaleOrderPaymentDisplay> {
    return this.http.get<SaleOrderPaymentDisplay>(this.baseApi + this.apiUrl + "/" + id);
  }

  getDefault() {
    return this.http.get(this.baseApi + this.apiUrl + '/DefaultGet');
  }

  create(val): Observable<SaleOrderPaymentDisplay> {
    return this.http.post<SaleOrderPaymentDisplay>(this.baseApi + this.apiUrl, val);
  }

  actionPayment(val: string[]){
    return this.http.post(this.baseApi + this.apiUrl + "/ActionPayment", val);
  }

}
