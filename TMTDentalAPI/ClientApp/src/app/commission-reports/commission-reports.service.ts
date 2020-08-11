import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';


export class ReportFilterCommission {
  dateFrom: string;
  dateTo: string;
 
}

export class CommissionReport {
  userId : string;
  name : string;
  productName : string;
  amountTotal : number;
  prepaidTotal : number;
  commissionTotal : number;
}

@Injectable({
  providedIn: 'root'
})
export class CommissionReportsService {
  apiUrl = "api/CommissionReports";
  constructor( private http: HttpClient, @Inject("BASE_API") private baseApi: string) { }

  getReport(val: ReportFilterCommission): Observable<CommissionReport[]> {
    return this.http.post<CommissionReport[]>(this.baseApi + this.apiUrl, val);
  }

}
