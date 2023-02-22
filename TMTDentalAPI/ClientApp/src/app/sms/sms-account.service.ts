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

export class SmsAccountPaged {
  limit: number;
  offset: number;
  search: string;
}

@Injectable({
  providedIn: 'root'
})
export class SmsAccountService {

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

  getDisplay(id: string) {
    return this.http.get(this.base_api + this.apiUrl + '/' + id);
  }

  getPaged(val: any) {
    return this.http.get(this.base_api + this.apiUrl + '/GetPaged', { params: val })
  }

  delete(id: string) {
    return this.http.delete(this.base_api + this.apiUrl + '/' + id);
  }

  smsSupplierAutocomplete(search: string) {
    return this.http.get(this.base_api + this.apiUrl + '/SmsSupplierAutocomplete', { params: new HttpParams().set('search', search) })
  }
}
