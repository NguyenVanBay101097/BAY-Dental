import { HttpClient } from '@angular/common/http';
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

export class SumaryRevenueReportFilter {
  dateFrom: string;
  dateTo: string;
  companyId: string;
  resultSelection: string;
  accountCode: string;
}

export class SumaryRevenueReport {
  type: string;
  credit: number;
  debit: number;
  balance: number;
}

export class ReportRevenueChartFilter {
  dateFrom: string;
  dateTo: string;
  companyId: string; 
  groupBy: string;
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
    return this.http.post(this.baseApi + this.apiUrl + '/GetCountMedicalXamination', val);
  }

  getSumary(val) {
    return this.http.post<RevenueTodayReponse>(this.baseApi + this.apiUrl + '/GetSumary', val);
  }

  createCustomerReceiptToAppointment(val) {
    return this.http.post(this.baseApi + this.apiUrl + '/CreateCustomerReceiptToAppointment', val);
  }

  getSumaryRevenueReport(val: any) {
    return this.http.post<SumaryRevenueReport>(this.baseApi + this.apiUrl + "/GetSumaryRevenueReport", val);
  }

  getRevenueChartReport(val: any) {
    return this.http.post(this.baseApi + this.apiUrl + "/GetRevenueChartReport", val);
  }

  getRevenueActualReport(val: any) {
    return this.http.post(this.baseApi + this.apiUrl + "/GetRevenueActualReport", val);
  }

  getThuChiReport(val: any) {
    return this.http.post(this.baseApi + this.apiUrl + "/GetThuChiReport", val);
  }

  getSummaryReport(val: any) {
    return this.http.post(this.baseApi + this.apiUrl + "/GetSummaryReport", val);
  }

  getRevenueReportChart(val: any) {
    return this.http.post(this.baseApi + this.apiUrl + "/GetRevenueReportChart", val);
  }
}
