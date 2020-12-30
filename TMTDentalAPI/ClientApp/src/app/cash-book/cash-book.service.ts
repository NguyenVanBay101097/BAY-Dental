import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedResult2 } from '../core/paged-result-2';

export class CashBookPaged {
  limit: number;
  offset: number;
  type: string;
  search: string;
  resultSelection: string;
  dateFrom: string;
  dateTo: string;
}

export class CashBookDisplay {
  resId: string;
  resModel: string;
  date: string;
  name: string;
  type: string;
  type2: string;
  amount: number;
  state: string;
  recipientPayer: string;
  journalId: string;
  journal: Object;
  companyId: string;
}

@Injectable({
  providedIn: 'root'
})

export class CashBookService {
  apiUrl = 'api/FundBooks';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  getPaged(val: any): Observable<PagedResult2<CashBookDisplay>> {
    return this.http.get<PagedResult2<CashBookDisplay>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
  }
}
