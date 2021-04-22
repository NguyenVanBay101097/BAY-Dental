import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

export class SmsAccountBasic {
  constructor() {
    this.apiKey = '';
    this.clientId = '';
    this.clientSecret = '';
    this.password = '';
    this.secretkey = '';
    this.userName = '';
  }
  provider: string;
  brandName: string;
  clientId: string;
  clientSecret: string;
  userName: string;
  password: string;
  apiKey: string;
  secretkey: string;
}

@Injectable({
  providedIn: 'root'
})
export default class SmsAccountService {

  apiUrl: string = 'api/SmsAccounts';
  constructor(@Inject('BASE_API') private base_api: string, private http: HttpClient) { }

  create(val) {
    return this.http.post(this.base_api + this.apiUrl, val);
  }

  update(id: string, val) {
    return this.http.put(this.base_api + this.apiUrl + '/' + id, val);
  }

  get(provider: string) {
    return this.http.get(this.base_api + this.apiUrl, { params: new HttpParams().set('provider', provider) })
  }
}
