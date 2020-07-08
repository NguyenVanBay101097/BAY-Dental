import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';

export class ReportPartnerSourceSearch {
  dateFrom: string;
  dateTo: string;
 
}

export class ReportSource {
  id : string;
  name : string;
  totalPartner : number;
  countPartner : number;
}


@Injectable({
  providedIn: 'root'
})
export class ReportPartnerSourcesService {
  apiUrl = "api/ReportPartnerSources";
  constructor( private http: HttpClient, @Inject("BASE_API") private baseApi: string) { }

  getReport(val: ReportPartnerSourceSearch): Observable<ReportSource[]> {
    return this.http.post<ReportSource[]>(this.baseApi + this.apiUrl, val);
  }
}
