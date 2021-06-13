import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SmsCareAfterOrderAutomationConfigService {

  apiUrl: string = 'api/SmsCareAfterOrderAutomationConfigs';
  constructor(@Inject('BASE_API') private base_api: string, private http: HttpClient) { }

  create(val) {
    return this.http.post(this.base_api + this.apiUrl, val);
  }

  update(id: string, val) {
    return this.http.put(this.base_api + this.apiUrl + '/' + id, val);
  }

  delete(id: string) {
    return this.http.delete(this.base_api + this.apiUrl + '/' + id);
  }

  getPaged(val: any) {
    return this.http.post(this.base_api + this.apiUrl + '/GetPaged', val);
  }

  getDisplay(id: string) {
    return this.http.get(this.base_api + this.apiUrl + '/' + id);
  }
}
