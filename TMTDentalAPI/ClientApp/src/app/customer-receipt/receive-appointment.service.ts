import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ReceiveAppointmentService {
  apiUrl = 'api/ReceiveAppointments';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  defaultGet(appointmentId: string) {
    return this.http.get(this.baseApi + this.apiUrl + "/DefaultGet?appointmentId=" + appointmentId);
  }

  actionSave(val: any) {
    return this.http.post(this.baseApi + this.apiUrl + "/ActionSave", val);
  }
}
