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

export class ReportDataResult {
  begin: number;
  totalAmount: number;
  totalChi: number;
  totalThu: number;
}

@Injectable({
  providedIn: 'root'
})

export class CashBookService {
  
  apiUrl = 'api/FundBooks';

  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  getMoney(val: CashBookPaged): Observable<PagedResult2<CashBookDisplay>> {
    return this.http.post<PagedResult2<CashBookDisplay>>(this.baseApi + this.apiUrl + '/GetMoney', val);
  }

  getSumary(val: any): Observable<ReportDataResult> {
    return this.http.post<ReportDataResult>(this.baseApi + this.apiUrl + '/GetSumary', val);
  }
}
