import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export class AccountJournalSimple {
    id: string;
    name: string;
    type: string;
}

export class AccountJournalFilter {
    offset: number;
    limit: number;
    search: string;
    type: string;
    companyId: string;
}

@Injectable({ providedIn: 'root' })
export class AccountJournalService {
    apiUrl = 'api/accountjournals';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    autocomplete(val: AccountJournalFilter): Observable<AccountJournalSimple[]> {
        return this.http.post<AccountJournalSimple[]>(this.baseApi + this.apiUrl + "/autocomplete", val);
    }
}