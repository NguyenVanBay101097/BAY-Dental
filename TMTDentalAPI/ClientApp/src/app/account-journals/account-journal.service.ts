import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';

export class AccountJournalSimple {
    id: string;
    name: string;
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