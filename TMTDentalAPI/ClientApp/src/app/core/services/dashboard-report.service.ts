import { ProductSimple } from 'src/app/products/product-simple';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

export class GetDefaultRequest {
  appointmentId: string;
}

export class GetCountMedicalXamination {
  NewMedical: number;
  ReMedical: number;
}

export class CustomerReceiptRequest {
  dateWaitting: string;
  timeExpected: number;
  products: ProductSimple[];
  note: string;
  partnerId: string;
  companyId: string;
  doctorId: string;
  isRepeatCustomer: boolean;
  appointmentId: string;
}

export class RevenueTodayRequest {
  dateTo: string;
  dateFrom: string;
  companyId: string;
}

export class RevenueTodayReponse {
  totalBank: number;
  totalCash: number;
  totalAmountYesterday: number;
  totalAmount: number;
}

@Injectable({
  providedIn: 'root'
})

export class DashboardReportService {
  apiUrl = 'api/DashboardReports';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  getDefaultCustomerReceipt(val) {
    return this.http.get(this.baseApi + this.apiUrl + "/GetDefaultCustomerReceipt", { params: new HttpParams({ fromObject: val }) });
  }

  getCountMedicalXamination() {
    return this.http.get(this.baseApi + this.apiUrl + '/GetCountMedicalXamination');
  }

  getSumary(val) {
    return this.http.post<RevenueTodayReponse>(this.baseApi + this.apiUrl + '/GetSumary', val);
  }

  createCustomerReceiptToAppointment(val) {
    return this.http.get(this.baseApi + this.apiUrl + '/CreateCustomerReceiptToAppointment', val);
  }



}
