import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedResult2 } from '../core/paged-result-2';
import { InsuranceDebtDetailItem, InsuranceHistoryInComeDetailItem, InsuranceHistoryInComeItem, InsuranceReportDetailFilter, InsuranceReportFilter } from './res-insurance-report.model';

@Injectable({
  providedIn: 'root'
})
export class ResInsuranceReportService {

  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  readonly apiUrl = 'api/ResInsuranceReports';

  getInsuranceDebtReport(val) {
    return this.http.get(this.baseApi + this.apiUrl + '/GetInsuranceDebtReport', { params: new HttpParams({ fromObject: val }) });
  }

  getInsuranceDebtDetailReport(val) {
    return this.http.post<InsuranceDebtDetailItem[]>(this.baseApi + this.apiUrl + '/GetInsuranceDebtDetailReport',  val);
  }

  getHistoryInComeDebtPaged(val): Observable<PagedResult2<InsuranceHistoryInComeItem>> {
    return this.http.get<PagedResult2<InsuranceHistoryInComeItem>>(this.baseApi + this.apiUrl + '/GetHistoryInComeDebtPaged',{ params: new HttpParams({ fromObject: val }) });
  }

  getHistoryInComeDebtDetails(val): Observable<PagedResult2<InsuranceHistoryInComeDetailItem>> {
    return this.http.post<PagedResult2<InsuranceHistoryInComeDetailItem>>(this.baseApi + this.apiUrl + '/GetHistoryInComeDebtDetails', val);
  }

  getSummaryReports(val: InsuranceReportFilter) {
    return this.http.post(this.baseApi + this.apiUrl + '/GetSummaryReports', val);
  }

  getDetailReports(val: InsuranceReportDetailFilter) {
    return this.http.post(this.baseApi + this.apiUrl + '/GetDetailReports', val);
  }

  exportExcelFile(val) {
    return this.http.post(this.baseApi + this.apiUrl + "/ExportExcelFile", val, { responseType: "blob" });
  }
  
  exportReportInsuranceExcelFile(val) {
    return this.http.post(this.baseApi + this.apiUrl + "/ExportReportInsuranceDebitExcel", val, { responseType: "blob" });
  }

  printGetSummary(val) {
    return this.http.post(this.baseApi + "ResInsurance/PrintGetSummary", val, { responseType: "text" });
  }

  getSummaryPdf(val: any) {
    return this.http.post(this.baseApi + this.apiUrl + "/GetSummaryPdf", val, { responseType: "blob" });
  }
}
