import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

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
  cateIds: string[];
  memberLevelId: string;
  gender: string;
  search: string;
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

  getReport(val: any): Observable<PartnerOldNewReportRes[]> {
    return this.http.get<PartnerOldNewReportRes[]>(this.baseApi + this.apiUrl + "/GetReport", { params: new HttpParams({ fromObject: val }) });
  }

  sumReport(val: any) {
    return this.http.get(this.baseApi + this.apiUrl + "/SumReport", { params: new HttpParams({ fromObject: val }) });
  }
}
