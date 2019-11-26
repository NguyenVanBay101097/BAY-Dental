import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { MailMessageFormat } from '../core/mail-message-format';

export class MailMessageFetch {
    limit: number;
}

@Injectable()
export class MailMessageService {
    apiUrl = 'api/MailMessages';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    messageFetch(val: MailMessageFetch): Observable<MailMessageFormat[]> {
        return this.http.post<MailMessageFormat[]>(this.baseApi + this.apiUrl + "/MessageFetch", val);
    }
}