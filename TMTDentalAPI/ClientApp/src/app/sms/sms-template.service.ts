import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SmsTemplateService {
  apiUrl: string = 'api/SmsTemplates';
  constructor(
    @Inject('BASE_API') private base_api: string,
    private http: HttpClient
  ) { }

  getPaged(val) {
    return this.http.get(this.base_api + this.apiUrl, val);
  }

  get(id: string) {
    return this.http.get(this.base_api + this.apiUrl + '/' + id);
  }

  create(val) {
    return this.http.post(this.base_api + this.apiUrl, val);
  }

  update(id: string, val) {
    return this.http.put(this.base_api + this.apiUrl + '/' + id, val);
  }
}
