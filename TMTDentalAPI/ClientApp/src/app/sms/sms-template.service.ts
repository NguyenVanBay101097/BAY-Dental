import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export class SmsTemplateBasic {
  id: string;
  name: string;
  body: string;
}
export class SmsTemplatePaging {
  offset: number;
  limit: number;
  totalItems: number;
  items: SmsTemplateBasic[];
}

export class SmsTemplatePaged{
  offset: number;
  limit: number;
  search: number;
}

@Injectable({
  providedIn: 'root'
})
export class SmsTemplateService {
  apiUrl: string = 'api/SmsTemplates';
  constructor(@Inject('BASE_API') private base_api: string, private http: HttpClient) { }

  get(id: string) {
    return this.http.get(this.base_api + this.apiUrl + '/' + id);
  }

  create(val) {
    return this.http.post(this.base_api + this.apiUrl, val);
  }

  update(id: string, val) {
    return this.http.put(this.base_api + this.apiUrl + '/' + id, val);
  }

  getPaged(val:any) {
    return this.http.get(this.base_api + this.apiUrl, { params: val });
  }

  delete(id: string) {
    return this.http.delete(this.base_api + this.apiUrl + '/' + id);
  }

  getAutoComplete(val) {
    return this.http.get(this.base_api + this.apiUrl + '/Autocomplete', val);
  }
}