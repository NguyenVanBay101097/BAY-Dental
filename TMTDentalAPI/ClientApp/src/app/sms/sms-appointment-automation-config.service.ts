import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SmsAppointmentAutomationConfigService {

  apiUrl: string = 'api/SmsAppointmentAutomationConfigs';
  constructor(@Inject('BASE_API') private base_api: string, private http: HttpClient) { }

  saveConfig(val) {
    return this.http.post(this.base_api + this.apiUrl, val);
  }
 
  getByCompany() {
    return this.http.get(this.base_api + this.apiUrl);
  }
}
