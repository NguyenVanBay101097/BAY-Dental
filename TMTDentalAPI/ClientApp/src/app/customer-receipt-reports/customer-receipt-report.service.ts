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

export class CustomerReceiptReportPaging {
  offset: number;
  limit: number;
  totalItems: number;
  items: CustomerReceiptReportBasic[];
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

  getCountCustomerReceipt(val: any) {
    return this.http.post(this.baseApi + this.apiUrl + '/GetCountCustomerReceipt', val);
  }

  getCountCustomerReceiptNoTreatment(val: any) {
    return this.http.post(this.baseApi + this.apiUrl + '/GetCountCustomerReceiptNoTreatment', val);
  }

}
