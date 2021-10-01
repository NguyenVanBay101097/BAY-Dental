import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedResult2 } from '../core/paged-result-2';

export class ExportExcelDashBoardDayFilter {
  dateFrom: string;
  dateTo: string;
  companyId: string;
}

@Injectable({
  providedIn: 'root'
})
export class DayDashboardReportService {
  apiUrl = 'api/DashboardReports';

  constructor(
    private http: HttpClient, @Inject('BASE_API') private baseApi: string
  ) { }

  getPaged(val: any): Observable<PagedResult2<any>> {
    return this.http.get<PagedResult2<any>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
  }

  exportReportDayExcel(val) {
    return this.http.post(
        this.baseApi + this.apiUrl + "/ExportExcelDayReport", val,
        { responseType: "blob" }
    );
  }
}
