import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SmsConfigService {

  apiUrl: string = 'api/SmsConfigs';
  constructor(@Inject('BASE_API') private base_api: string, private http: HttpClient) { }

  create(val) {
    return this.http.post(this.base_api + this.apiUrl, val);
  }

  update(id: string, val) {
    return this.http.put(this.base_api + this.apiUrl + '/' + id, val);
  }
}
