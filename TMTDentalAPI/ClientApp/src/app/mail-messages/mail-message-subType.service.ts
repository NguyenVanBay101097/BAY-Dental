import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export class MailMessageSubtypeListResponse{
    id: string;
    name: string;
}
@Injectable({providedIn: 'root'})
export class MailMessageSubTypeService {
    apiUrl = 'api/MailMessageSubtypes';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    get(): Observable<MailMessageSubtypeListResponse[]> {
        return this.http.get<MailMessageSubtypeListResponse[]>(this.baseApi + this.apiUrl);
    }

}