import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export class FundBookSearch {
  dateFrom: string;
  dateTo: string;
  companyId: string;
  resultSelection: string;
  type: string;
  state: string;
}

export class FundBookResponse {
  begin: number;
  totalChi: number;
  totalThu: number;
  totalAmount: number;
}

@Injectable({
  providedIn: 'root'
})
export class FundBookService {

  constructor(private http: HttpClient, @Inject("BASE_API") private base_api: string) { }

  apiUrl = "api/FundBooks"

  getSumary(val): Observable<FundBookResponse> {
    return this.http.post<FundBookResponse>(this.base_api + this.apiUrl + "/GetSumary", val);
  }

}
