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

  sumReport(val: any) {
    return this.http.get(this.baseApi + this.apiUrl + "/SumReport", { params: new HttpParams({ fromObject: val }) });
  }

}
