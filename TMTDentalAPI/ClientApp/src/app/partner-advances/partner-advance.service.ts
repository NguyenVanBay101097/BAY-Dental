import { AccountJournalSimple } from './../account-journals/account-journal.service';
import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export class PartnerAdvancePaged {
  limit: number;
  offset: number;
  search: string;
  dateFrom: string;
  dateTo: string;

}


export class PartnerAdvanceBasic {
  id: string;
  name: string;
  journalName: string;
  amount: number;
  date: Date;
  state: string;
  type: string;
}

export class PartnerAdvancePagging {
  limit: number;
  offset: number;
  totalItems: number;
  items: PartnerAdvanceBasic[];
}

export class PartnerAdvanceSave {
  date: Date;
  note: string;
  state: string;
  amount: number;
  journalId: string;
  partnerId: string;
  companyId: string;
  type: string;
}

export class PartnerAdvanceDisplay {
  id: string;
  date: Date;
  note: string;
  state: string;
  type: string;
  journalId: string;
  journal: AccountJournalSimple;
  partnerId: string;
  partnerName: string;
  companyId: string;
  amount: number;
}

export class PartnerAdvanceDefaultFilter {
  type: string;
  partnerId: string;
}

export class PartnerAdvanceSummaryFilter {
  type: string;
  dateFrom: string;
  dateTo: string;
}

export class PartnerAdvanceGetSummary {
  type: string;
  amountTotal: number;
}

@Injectable({
  providedIn: 'root'
})
export class PartnerAdvanceService {

  constructor(private http: HttpClient, @Inject("BASE_API") private base_api: string) { }
  apiUrl = "api/PartnerAdvances";

  getPaged(val): Observable<PartnerAdvancePagging> {
    return this.http.get<PartnerAdvancePagging>(this.base_api + this.apiUrl, { params: val });
  }

  get(id): Observable<PartnerAdvanceDisplay> {
    return this.http.get<PartnerAdvanceDisplay>(this.base_api + this.apiUrl + "/" + id);
  }

  getDefault(val) {
    return this.http.post(this.base_api + this.apiUrl + '/DefaultGet', val);
  }

  getSumary(val) {
    return this.http.post(this.base_api + this.apiUrl + '/GetSummary', val);
  }

  create(val) {
    return this.http.post(this.base_api + this.apiUrl, val);
  }

  update(id, val) {
    return this.http.put(this.base_api + this.apiUrl + '/' + id, val);
  }

  actionConfirm(ids) {
    return this.http.post(this.base_api + this.apiUrl + '/ActionConfirm', ids)
  }

  delete(id: string) {
    return this.http.delete(this.base_api + this.apiUrl + "/" + id);
  }

  getPrint(id: string) {
    return this.http.get(this.base_api + this.apiUrl + "/" + id + '/GetPrint');
  }
}
