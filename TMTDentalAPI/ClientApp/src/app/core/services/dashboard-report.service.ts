import { ProductSimple } from 'src/app/products/product-simple';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

export class GetDefaultRequest {
  appointmentId: string;
}

export class CountMedicalXamination {
  newMedical: number;
  reMedical: number;
}

export class CustomerReceiptRequest {
  dateWaiting: string;
  timeExpected: number;
  products: [];
  note: string;
  partnerId: string;
  companyId: string;
  doctorId: string;
  isRepeatCustomer: boolean;
  appointmentId: string;
}

export class ReportTodayRequest {
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
    return this.http.post(this.baseApi + this.apiUrl + "/GetDefaultCustomerReceipt", val);
  }

  getCountMedicalXamination(val) {
    return this.http.post(this.baseApi + this.apiUrl + '/GetCountMedicalXamination' , val);
  }

  getCountSaleOrder(val) {
    return this.http.post<number>(this.baseApi + this.apiUrl + '/GetCountSaleOrder', val);
  }

  getSumary(val) {
    return this.http.post<RevenueTodayReponse>(this.baseApi + this.apiUrl + '/GetSumary', val);
  }

  createCustomerReceiptToAppointment(val) {
    return this.http.post(this.baseApi + this.apiUrl + '/CreateCustomerReceiptToAppointment', val);
  }



}
