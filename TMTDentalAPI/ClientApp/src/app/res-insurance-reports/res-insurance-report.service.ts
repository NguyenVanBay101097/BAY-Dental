import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ResInsuranceReportService {

  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  readonly apiUrl = 'api/ResInsuranceReports';

  getInsuranceDebtReport(val) {
    return this.http.get(this.baseApi + this.apiUrl + '/GetInsuranceDebtReport', { params: new HttpParams({fromObject: val})});
  }

}
