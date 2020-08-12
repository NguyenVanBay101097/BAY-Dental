import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';


export class ReportFilterCommission {
  dateFrom: string;
  dateTo: string;
  companyId: string;
}

export class ReportFilterCommissionDetail {
  dateFrom: string;
  dateTo: string;
  userId: string; 
}

export class CommissionReport {
  userId : string;
  name : string;
  // amountTotal : number;
  // prepaidTotal : number; 
  commissionTotal : number;
}

export class CommissionReportDetail {
  userId : string;
  name : string;
  date : Date;
  productName : string;
  amountTotal : number;
  prepaidTotal : number;
  percentCommission : number;
  commissionTotal : number;
}

@Injectable({
  providedIn: 'root'
})
export class CommissionReportsService {
  apiUrl = "api/CommissionReports";
  constructor( private http: HttpClient, @Inject("BASE_API") private baseApi: string) { }

  getReport(val: ReportFilterCommission): Observable<CommissionReport[]> {
    return this.http.post<CommissionReport[]>(this.baseApi + this.apiUrl + "/GetReport" , val);
  }

  getReportDetail(val: ReportFilterCommissionDetail): Observable<CommissionReportDetail[]> {
    return this.http.post<CommissionReportDetail[]>(this.baseApi + this.apiUrl + "/GetReportDetail", val);
  }
}
