import { Injectable, Inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from 'rxjs';

export class ReportTCareSearch {
  dateFrom: string;
  dateTo: string;
  tCareScenarioId: string;
}

export class ReportTCare {
  id: string;
  name: string;
  items: number;
  messageTotal: number;
  deliveryTotal: number;
  readTotal: number;
}

export class ReportTCareItem {
  id: string;
  name: string;
  messageTotal: number;
  deliveryTotal: number;
  readTotal: number;
}

@Injectable({
  providedIn: "root",
})
export class TcareReportService {
  apiUrl = "api/ReportTCares";
  constructor(private http: HttpClient,@Inject("BASE_API") private baseApi: string) {}

  getReport(val: ReportTCareSearch): Observable<ReportTCare[]> {
    return this.http.post<ReportTCare[]>(this.baseApi + this.apiUrl + "/GetReport", val);
  }

  getDetail(val: ReportTCare): Observable<ReportTCareItem[]> {
    return this.http.post<ReportTCareItem[]>(this.baseApi + this.apiUrl + "/GetReportDetail", val);
  }
}
