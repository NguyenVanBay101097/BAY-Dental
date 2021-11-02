import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ProductSimple } from '../products/product-simple';
import { SaleReportTopServicesCs } from './sale-report';

export class TopServices {
  productQtyTotal: number;
  productId: string;
  productSimple: ProductSimple;
}

export class AmountInvoice {
  value: number;
  name: string;
}

export class AppointStateCount {
  count: number;
  state: string;
}

@Injectable({
  providedIn: 'root'
})
export class HomeService {

  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  getAppointCount(dateFrom, dateTo): Observable<AppointStateCount[]> {
    return this.http.post<AppointStateCount[]>(this.baseApi + "api/Appointments/CountAppointment", { dateFrom: dateFrom, dateTo: dateTo });
  }

  getInvoicedTopService(number: number): Observable<TopServices[]> {
    return this.http.get<TopServices[]>(this.baseApi + "api/AccountInvoiceReports/GetTop/" + number);
  }

  getSaleReportTopService(val): Observable<SaleReportTopServicesCs[]> {
    return this.http.get<SaleReportTopServicesCs[]>(this.baseApi + "api/SaleReports/GetTopService", { params: val });
  }

  getAmountResidualToday(): Observable<AmountInvoice[]> {
    return this.http.post<AmountInvoice[]>(this.baseApi + "api/AccountInvoiceReports/GetAmountResidualToday", null);
  }


}
