import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedResult2 } from '../core/paged-result-2';
import { UserSimple } from '../users/user-simple';
import { PartnerSimple } from './partner-simple';

// import { PagedResult2 } from '../paged-result-2';

export class QuotationsBasic {
  id: string;
  name: string;
  partner: PartnerSimple;
  user: UserSimple;
  dateQuotation: string;
  dateApplies: number;
  dateEndQuotation: string;
  note: string;
  totalAmount: number;
  state: string;
}
export class QuotationsPaged{
  dateFrom: string;
  dateTo: string;
  search: string;
  limt: number;
  offset: number;
}
@Injectable({
  providedIn: 'root'
})
export class QuotationService {
  apiUrl = 'api/Quotations';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }
  getPaged(val: any): Observable<PagedResult2<QuotationsBasic>> {
    return this.http.get<PagedResult2<QuotationsBasic>>(this.baseApi + this.apiUrl, { params: new HttpParams(val) });
  }
}
