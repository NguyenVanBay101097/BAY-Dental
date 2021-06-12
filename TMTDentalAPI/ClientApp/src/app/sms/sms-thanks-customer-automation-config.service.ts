import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SmsThanksCustomerAutomationConfigService {

  apiUrl: string = 'api/SmsThanksCustomerAutomationConfigs';
  constructor(@Inject('BASE_API') private base_api: string, private http: HttpClient) { }

  create(val) {
    return this.http.post(this.base_api + this.apiUrl, val);
  }

  update(id: string, val) {
    return this.http.put(this.base_api + this.apiUrl + '/' + id, val);
  }

  getByCompany(id: string) {
    return this.http.get(this.base_api + this.apiUrl + '/' + id);
  }
}
