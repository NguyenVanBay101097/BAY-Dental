import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/internal/Observable';

export class CommissionSettlementReport {
  dateFrom: string;
  dateTo: string;
  employeeId: string;
  companyId: string;
  limit: number;
  offset: number;
}

export class CommissionSettlementReportOutput {
  employeeId: string;
  employeeName: string;
  baseAmount: number;
  percentage: number;
  amount: number;
  dateFrom: string;
  dateTo: string;
  companyId: string;
}

export class CommissionSettlementReportDetailOutput {
  date : Date;
  productName: string;
  baseAmount: number;
  percentage: number;
  amount: number;
}

@Injectable({
  providedIn: 'root'
})
export class CommissionSettlementsService {
  apiUrl = "api/CommissionSettlements";
  constructor(private http: HttpClient, @Inject("BASE_API") private baseApi: string) { }

  getReport(val: CommissionSettlementReport): Observable<CommissionSettlementReportOutput[]> {
    return this.http.post<CommissionSettlementReportOutput[]>(this.baseApi + this.apiUrl + "/GetReport" , val);
  }

  getReportDetail(val: CommissionSettlementReport) {
    return this.http.post(this.baseApi + this.apiUrl + "/GetReportDetail", val);
  }
}
