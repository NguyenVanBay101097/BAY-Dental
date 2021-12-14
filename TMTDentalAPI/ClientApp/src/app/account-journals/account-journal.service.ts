import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ResPartnerBankBasic } from '../res-partner-banks/res-partner-bank';

export class AccountJournalSimple {
    id: string;
    name: string;
    type: string;
}

export class AccountJournalResBankSimple {
    id: string;
    name: string;
    type: string;
    bankAccount: ResPartnerBankBasic;
}

export class AccountJournalFilter {
    offset: number;
    limit: number;
    search: string;
    type: string;
    companyId: string;
}

export class AccountJournalSave {
    name: string;
    bankId: string;
    accountNumber: string;
    type: string;
    accountHolderName: string;
    bankBranch: string;
    active: boolean;
}

@Injectable({ providedIn: 'root' })
export class AccountJournalService {
    apiUrl = 'api/accountjournals';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    autocomplete(val: AccountJournalFilter): Observable<AccountJournalSimple[]> {
        return this.http.post<AccountJournalSimple[]>(this.baseApi + this.apiUrl + "/autocomplete", val);
    }

    getById(id: string) {
        return this.http.get(this.baseApi + this.apiUrl + "/" + id);
    }

    create(val: AccountJournalSave) {
        return this.http.post(this.baseApi + this.apiUrl + "/CreateJournalSave", val);
    }

    update(id: string, val: AccountJournalSave) {
        return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
    }

    journalResBankAutoComplete(val: AccountJournalFilter): Observable<AccountJournalResBankSimple[]> {
        return this.http.post<AccountJournalResBankSimple[]>(this.baseApi + this.apiUrl + "/JournalResBankAutoComplete", val);
    }

    createBankJournal(val: any) {
        return this.http.post(this.baseApi + this.apiUrl + "/CreateBankJournal", val);
    }

    getBankJournal(id: string) {
        return this.http.get(this.baseApi + this.apiUrl + "/" + id + '/GetBankJournal');
    }

    updateBankJournal(val: any) {
        return this.http.post(this.baseApi + this.apiUrl + "/UpdateBankJournal", val);
    }

    getBankJournals(val: any) {
        return this.http.get(this.baseApi + this.apiUrl + "/GetBankJournals", { params: new HttpParams({ fromObject: val }) });
    }
}