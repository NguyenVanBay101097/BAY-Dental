import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PagedResult2 } from '../core/paged-result-2';
import { ResPartnerBankBasic } from './res-partner-bank';

export class ResBankSimple {
  id: string;
  name: string;
}
export class AccountJournalSave {
  name: string;
  accountNumber: string;
  type: string;
  bankId: string;
}

export class AccountJournalBasic {
  id: string;
  name: string;
}

@Injectable({
  providedIn: 'root'
})
export class ResPartnerBankService {

  readonly apiUrl = 'api/ResPartnerBanks';
  readonly journalUrl = 'api/AccountJournals';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  getPaged(paged): Observable<PagedResult2<AccountJournalBasic>> {
    return this.http.get<PagedResult2<AccountJournalBasic>>(this.baseApi + this.journalUrl + '/GetBankCashJournals', { params: paged });
  }

  createUpdate(val, id) {
    if (id) {
      return this.http.put(this.baseApi + this.journalUrl + `/UpdateJournalSave/${id}`, val);
    }
    else {
      return this.http.post(this.baseApi + this.journalUrl + '/CreateJournalSave', val);
    }
  }

  deactive(ids) {
    return this.http.put(this.baseApi + this.journalUrl + '/ActiveChangeJournals', ids);
  }

  getById(id): Observable<AccountJournalSave> {
    return this.http.get<AccountJournalSave>(this.baseApi + this.journalUrl + '/' + id);
  }

  autocompleteBank(val): Observable<ResBankSimple[]> {
    return this.http.get<ResBankSimple[]>(this.baseApi + 'api/ResBanks/Autocomplete', { params: val });
  }
}
