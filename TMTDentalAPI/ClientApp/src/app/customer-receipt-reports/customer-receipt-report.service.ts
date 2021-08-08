import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedResult2, PartnerSimple } from '../partners/partner-simple';

export class CustomerReceiptReportFilter {
  limit: number;
  offset: number;
  search: string;
  dateFrom: string;
  dateTo: string;
  timeFrom: string;
  timeTo: string;
  doctorId: string;
  companyId: string;
  isRepeatCustomer: string;
  isNoTreatment: string;
  state: string;
}

export class CustomerReceiptTimeDetailFilter {
  limit: number;
  offset: number;
  dateFrom: string;
  dateTo: string;
  time: number;
  companyId: string;
}

export class CustomerReceiptReportPaging {
  offset: number;
  limit: number;
  totalItems: number;
  items: CustomerReceiptReportBasic[];
}

export class CustomerReceiptTimePaging {
  offset: number;
  limit: number;
  totalItems: number;
  items: CustomerReceiptTime[];
}

export class CustomerReceiptReportBasic {
  id: string;
  dateWaiting: string;
  dateExamination: string;
  dateDone: string;
  doctorName: string;
  partner: PartnerSimple;
  products: string;
  isRepeatCustomer: boolean;
  isNoTreatment: boolean;
  minuteWaiting: number;
  minuteExamination: number;
  minuteTotal: number;
  state: string;
  reason: string;
}

export class CustomerReceiptTime {
  timeRange: string;
  time: number;
  timeRangeCount: number;
}

export class CustomerReceiptGetCountExamination {
  isExamination: boolean;
  countCustomerReceipt: number;
  totalCustomerReceipt: number;
}

export class CustomerReceiptGetCountNotreatment {
  isNotreatment: boolean;
  countCustomerReceipt: number;
  totalCustomerReceipt: number;
}

@Injectable({
  providedIn: 'root'
})
export class CustomerReceiptReportService {
  apiUrl = 'api/CustomerReceiptReports';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  getPaged(val: any): Observable<PagedResult2<CustomerReceiptReportBasic>> {
    return this.http.get<PagedResult2<CustomerReceiptReportBasic>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
  }

  getCustomerReceiptTimePaged(val: any): Observable<PagedResult2<CustomerReceiptTime>> {
    return this.http.get<PagedResult2<CustomerReceiptTime>>(this.baseApi + this.apiUrl + '/GetCustomerReceiptForTime', { params: new HttpParams({ fromObject: val }) });
  }

  getCustomerReceiptForTimeDetailPaged(val: any): Observable<PagedResult2<CustomerReceiptReportBasic>> {
    return this.http.get<PagedResult2<CustomerReceiptReportBasic>>(this.baseApi + this.apiUrl + '/GetCustomerReceiptForTimeDetail', { params: new HttpParams({ fromObject: val }) });
  }

  getCountCustomerReceipt(val: any) {
    return this.http.post(this.baseApi + this.apiUrl + '/GetCountCustomerReceipt', val);
  }

  getCountCustomerReceiptNoTreatment(val: any) {
    return this.http.post(this.baseApi + this.apiUrl + '/GetCountCustomerReceiptNoTreatment', val);
  }

  getCount(val: any) {
    return this.http.post(this.baseApi + this.apiUrl + '/GetCount', val);
  }

  exportExcelReportOverview(val) {
    return this.http.post(this.baseApi + this.apiUrl + '/ExportExcelReportOverview', val, { responseType: "blob" });
  }

  exportExcelReportForTime(val) {
    return this.http.post(this.baseApi + this.apiUrl + '/ExportExcelReportForTime', val, { responseType: "blob" });
  }

  exportExcelReportTimeService(val) {
    return this.http.post(this.baseApi + this.apiUrl + '/ExportExcelReportTimeService', val, { responseType: "blob" });
  }

  exportExcelReportNoTreatment(val) {
    return this.http.post(this.baseApi + this.apiUrl + '/ExportExcelReportNoTreatment', val, { responseType: "blob" });
  }

}
