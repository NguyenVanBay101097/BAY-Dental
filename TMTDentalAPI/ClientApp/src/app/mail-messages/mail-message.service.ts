import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
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