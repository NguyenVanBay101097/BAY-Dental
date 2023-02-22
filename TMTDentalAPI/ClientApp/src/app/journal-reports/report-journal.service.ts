import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

export class ReportCashBankJournalSearch {
  resultSelection: string;
  dateFrom: string;
  dateTo: string;
  companyId: string;
}

@Injectable({
  providedIn: 'root'
})

export class ReportJournalService {

  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  readonly apiUrl = 'api/ReportJournals';

  getCashBankReport(val: ReportCashBankJournalSearch) {
    return this.http.post(this.baseApi + this.apiUrl + '/GetCashBankReport', val);
  }
}
