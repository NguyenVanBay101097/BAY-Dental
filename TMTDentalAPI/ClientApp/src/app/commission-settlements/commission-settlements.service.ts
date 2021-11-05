import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs/internal/Observable';

export class CommissionSettlementFilterReport {
  dateFrom: string;
  dateTo: string;
  search: string;
  employeeId: string;
  companyId: string;
  limit: number;
  offset: number;
  commissionType: string;
  groupBy: string;
  agentId: string;
  classify: string;
  commissionDisplay: string;
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

export class CommissionSettlementOverviewFilter {
  dateFrom: string;
  dateTo: string;
  groupBy: string;
  classify: string;
}
@Injectable({
  providedIn: 'root'
})
export class CommissionSettlementsService {
  apiUrl = "api/CommissionSettlements";
  constructor(private http: HttpClient, @Inject("BASE_API") private baseApi: string) { }

  getReport(val: CommissionSettlementFilterReport): Observable<CommissionSettlementReportOutput[]> {
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

  getSumAmountTotalReport(val: any) {
    return this.http.post(this.baseApi + this.apiUrl + '/GetSumAmountTotalReport', val);
  }

  getAgentOverview(val: CommissionSettlementOverviewFilter){
    return this.http.post(this.baseApi + this.apiUrl + '/GetCommissionSettlementReport',val);
  }

  excelCommissionExport(val) {
    return this.http.get(this.baseApi + this.apiUrl + '/ExportExcel', { params: val, responseType: 'blob' });
  }
  excelCommissionDetailExport(val) {
    return this.http.get(this.baseApi + this.apiUrl + '/DetailExportExcel', { params: new HttpParams({fromObject: val}), responseType: 'blob' });
  }

  exportExcelCommissionItemDetail(val) {
    return this.http.get(this.baseApi + this.apiUrl + '/ItemDetailExportExcel', { params: new HttpParams({fromObject: val}), responseType: 'blob' });
  }
}
