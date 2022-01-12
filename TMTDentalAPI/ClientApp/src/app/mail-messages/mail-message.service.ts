import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { MailMessageFormat } from '../core/mail-message-format';

export class MailMessageFetch {
    limit: number;
}
export class LogForPartnerRequest{
    dateFrom: any;
    dateTo: any;
    threadModel: string;
    threadId: string;
    SubtypeId: string;
}

export class TimeLineLogForPartnerResponse{
    date: any;
    logs: LogForPartnerResponse[];
}

export class LogForPartnerResponse{
    id: string;
    date: any;
    body: string;
    subtypeName: string;
    userName: string;
}
@Injectable({providedIn:"root"})
export class MailMessageService {
    apiUrl = 'api/MailMessages';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    messageFetch(val: MailMessageFetch): Observable<MailMessageFormat[]> {
        return this.http.post<MailMessageFormat[]>(this.baseApi + this.apiUrl + "/MessageFetch", val);
    }

    getLogsForPartner(val: LogForPartnerRequest){
        return this.http.post<TimeLineLogForPartnerResponse[]>(this.baseApi + this.apiUrl + "/GetLogsForPartner", val);
    }

    delete(id){
        return this.http.delete(this.baseApi + this.apiUrl + `/${id}`);
    }
}