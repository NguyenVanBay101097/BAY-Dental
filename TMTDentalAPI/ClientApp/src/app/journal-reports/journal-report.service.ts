import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PagedResult2 } from '../employee-categories/emp-category';
import { JournalReport, JournalReportDetailPaged } from './journal-report';

export class AccountMoveLineReport {
  name: string;
  debit: number;
  credit: number;
  balance: number;
  // date: string;
  // partnerId: string;
}

@Injectable({
  providedIn: 'root'
})
export class JournalReportService {

  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  readonly apiUrl = 'api/journalReports';

  getReports(paged): Observable<JournalReport[]> {
    return this.http.get<JournalReport[]>(this.baseApi + this.apiUrl, { params: paged });
  }

  getDetail(val: JournalReportDetailPaged): Observable<AccountMoveLineReport[]> {
    return this.http.post<AccountMoveLineReport[]>(this.baseApi + this.apiUrl + '/GetMoveLines', val);
  }
}
