import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedResult2 } from '../employee-categories/emp-category';
import { SaleOrderBasic } from '../sale-orders/sale-order-basic';

export class PartnerOldNewReport {
  weekStart: string;
  weekEnd: string;
  weekOfYear: number;
  year: number;
  totalNewPartner: number;
  totalOldPartner: number;
  orderLines: PartnerOldNewReportDetail[];
}

export class PartnerOldNewReportDetail {
  date: string;
  partnerId: string;
  partnerName: string;
  orderName: string;
  countLine: number;
  type: string;
}

export class PartnerOldNewReportSearch {
  dateFrom: string;
  dateTo: string;
  companyId: string;
}


export class PartnerOldNewReportReq {
  dateFrom: any;
  dateTo: any;
  companyId: string;
  typeReport: string;
  cityCode: string;
  districtCode: string;
  wardCode: string;
  sourceId: string;
  categIds: string[];
  memberLevelId: string;
  gender: string;
  search: string;
  limit: number;
  offset: number;
  partnerId: string;
}

export class GetSaleOrderPagedReq {
  dateFrom: any;
  dateTo: any;
  companyId: string;
  typeReport: string;
  limit: number;
  offset: number;
  partnerId: string;
}

export class PartnerOldNewReportRes {
  id: string;
  ref: string;
  displayName: string;
  name: string;
  birthYear: number;
  orderState: string;
  revenue: number;
  memberLevel?: any;
  categories: any[];
  age: string;
  gender: string;
  address: string;
  sourceName?: any;
}
export class PartnerOldNewReportSumReq {
  dateFrom: any;
  dateTo: any;
  companyId: string;
  typeReport: string;
}

@Injectable({
  providedIn: 'root'
})
export class PartnerOldNewReportService {

  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }
  apiUrl = "api/PartnerOldNewReports";

  getPartnerOldNewReport(val: PartnerOldNewReportSearch): Observable<PartnerOldNewReport[]> {
    return this.http.post<PartnerOldNewReport[]>(this.baseApi + this.apiUrl + "/GetPartnerOldNewReport", val);
  }

  getSumaryPartnerOldNewReport(val: PartnerOldNewReportSearch): Observable<PartnerOldNewReport> {
    return this.http.post<PartnerOldNewReport>(this.baseApi + this.apiUrl + "/GetSumaryPartnerOldNewReport", val);
  }

  getReport(val: any) {
    return this.http.get<PagedResult2<PartnerOldNewReportRes>>(this.baseApi + this.apiUrl + "/GetReport", { params: new HttpParams({ fromObject: val }) });
  }

  sumReport(val: any) {
    return this.http.get(this.baseApi + this.apiUrl + "/SumReport", { params: new HttpParams({ fromObject: val }) });
  }

  sumReVenue(val: any) {
    return this.http.get(this.baseApi + this.apiUrl + "/SumReVenue", { params: new HttpParams({ fromObject: val }) });
  }

  getReportPrint(val) {
    return this.http.get(this.baseApi + 'PartnerOldNewReport/GetReportPrint', { params: new HttpParams({ fromObject: val }), responseType: 'text' });
  }

  getReportPdf(val) {
    return this.http.get(this.baseApi + this.apiUrl + '/GetReportPdf', { params: new HttpParams({ fromObject: val }), responseType: 'blob' });
  }

  getReportExcel(val) {
    return this.http.get(this.baseApi + this.apiUrl + '/GetReportExcel', { params: new HttpParams({ fromObject: val }), responseType: 'blob' });
  }

  getSaleOrderPaged(val: any) {
    return this.http.get<PagedResult2<SaleOrderBasic>>(this.baseApi + this.apiUrl + "/GetSaleOrderPaged", { params: new HttpParams({ fromObject: val }) });
  }
}
