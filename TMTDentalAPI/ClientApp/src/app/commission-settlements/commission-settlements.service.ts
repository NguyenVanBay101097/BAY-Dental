import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs/internal/Observable';

export class CommissionSettlementReport {
  dateFrom: string;
  dateTo: string;
  employeeId: string;
  companyId: string;
  limit: number;
  offset: number;
  commissionType: string;
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
  date: Date;
  employeeName: string;
  commissionType: string;
  invoiceOrigin: string;
  partnerName: string;
  productName: string;
  baseAmount: number;
  percentage: number;
  amount: number;
}

export class CommissionSettlementDetailReportPar {
  dateFrom: string;
  dateTo: string;
  employeeId: string;
  companyId: string;
  commissionType: string;
  search: string;
  limit: number;
  offset: number;
}

export class CommissionSettlementReportRes {
  employeeName: string;
  commissionType: string;
  amount: number;
}
@Injectable({
  providedIn: 'root'
})
export class CommissionSettlementsService {
  apiUrl = "api/CommissionSettlements";
  constructor(private http: HttpClient, @Inject("BASE_API") private baseApi: string) { }

  getReport(val: CommissionSettlementReport): Observable<CommissionSettlementReportOutput[]> {
    return this.http.post<CommissionSettlementReportOutput[]>(this.baseApi + this.apiUrl + "/GetReport", val);
  }

  getReportDetail(val: CommissionSettlementDetailReportPar) {
    return this.http.post(this.baseApi + this.apiUrl + "/GetReportDetail", val);
  }

  getReportPaged(val: any) {
    return this.http.get(this.baseApi + this.apiUrl + "/GetReportPaged", { params: val });
  }

  getSumReport(val: any) {
    return this.http.post(this.baseApi + this.apiUrl + '/GetSumReport', val);
  }

  excelCommissionExport(val) {
    return this.http.post(this.baseApi + this.apiUrl + '/', val, { responseType: 'blob' });
  }
  excelCommissionDetailExport(val) {
    return this.http.post(this.baseApi + this.apiUrl + '/', val, { responseType: 'blob' });
  }
}
