import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

export class SmsSmsPaged {
  limit: number;
  offset: number;
  search: string;
  state: string;
  dateFrom: string;
  dateTo: string;
}

@Injectable({
  providedIn: 'root'
})
export class SmsSmsService {
  apiUrl: string = 'api/SmsSms';
  constructor(
    @Inject("BASE_API") private base_api: string,
    private http: HttpClient
  ) { }

  getPaged(val) {
    return this.http.get(this.base_api + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
  }
}
