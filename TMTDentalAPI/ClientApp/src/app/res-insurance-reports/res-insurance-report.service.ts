import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { InsuranceReportDetailFilter, InsuranceReportFilter } from './res-insurance-report.model';

@Injectable({
  providedIn: 'root'
})
export class ResInsuranceReportService {

  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  readonly apiUrl = 'api/ResInsuranceReports';

  getInsuranceDebtReport(val) {
    return this.http.get(this.baseApi + this.apiUrl + '/GetInsuranceDebtReport', { params: new HttpParams({ fromObject: val }) });
  }

  getSummaryReports(val: InsuranceReportFilter) {
    return this.http.post(this.baseApi + this.apiUrl + '/GetSummaryReports', val);
  }

  getDetailReports(val: InsuranceReportDetailFilter) {
    return this.http.post(this.baseApi + this.apiUrl + '/GetDetailReports', val);
  }

  exportExcelFile(val) {
    return this.http.post(this.baseApi + this.apiUrl + "/ExportReportInsuranceDebitExcel", val, { responseType: "blob" });
  }

  printGetSummary(val) {
    return this.http.post(this.baseApi + "ResInsurance/PrintGetSummary", val, { responseType: "text" });
  }

  getSummaryPdf(val: any) {
    return this.http.post(this.baseApi + this.apiUrl + "/GetSummaryPdf", val, { responseType: "blob" });
  }
}
