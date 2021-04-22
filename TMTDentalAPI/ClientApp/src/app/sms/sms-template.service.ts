import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SmsTemplateService {
  apiUrl: string = 'api/';
  constructor(
    @Inject('BASE_API') private base_api: string,
    private http: HttpClient
  ) { }

  getAutoComplete(val: any): Observable<any[]> {
    return this.http.get<any[]>(this.base_api + this.apiUrl + '/AutoComplete', { params: val })
  }
}
