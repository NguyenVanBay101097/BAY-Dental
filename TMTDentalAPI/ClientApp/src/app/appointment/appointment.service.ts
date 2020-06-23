import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import { AppointmentDisplay, AppointmentPaged, AppointmentBasic, AppointmentPaging, AppointmentPatch, AppointmentDefaultGet, AppointmentSearchByDate } from './appointment';
import { Observable } from 'rxjs';
import { PagedResult2 } from '../partners/partner-simple';
import { formatDate } from '@angular/common';

@Injectable({
  providedIn: 'root'
})
export class AppointmentService {

  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  getAppointmentList(appointmentPaged: AppointmentPaged): Observable<PagedResult2<AppointmentBasic>> {
    var param = new HttpParams()
      .set('offset', appointmentPaged.offset.toString());
    return this.http.get<PagedResult2<AppointmentBasic>>(this.baseApi + "api/Appointments?" + param);
  }

  loadAppointmentList(appointmentPaged: AppointmentPaged): Observable<PagedResult2<AppointmentBasic>> {
    var param = new HttpParams()
      .set('offset', appointmentPaged.offset.toString())
      .set('limit', appointmentPaged.limit.toString());
    if (appointmentPaged.search) {
      param = param.set('search', appointmentPaged.search.toString());
    };
    // if (appointmentPaged.searchCustomer) {
    //   param = param.set('searchByCustomer', appointmentPaged.searchCustomer.toString());
    // };
    // if (appointmentPaged.searchDoctor) {
    //   param = param.set('searchByDoctor', appointmentPaged.searchDoctor.toString());
    // };
    if (appointmentPaged.dateTimeFrom) {
      param = param.set('dateTimeFrom', formatDate(appointmentPaged.dateTimeFrom.toString(), 'yyyy-MM-dd HH:mm:ss', 'vi-VN'));
    };
    if (appointmentPaged.dateTimeTo) {
      param = param.set('dateTimeTo', formatDate(appointmentPaged.dateTimeTo.toString(), 'yyyy-MM-dd HH:mm:ss', 'vi-VN'));
    };
    if (appointmentPaged.state != null) {
      param = param.set('state', appointmentPaged.state);
    };
    return this.http.get<PagedResult2<AppointmentBasic>>(this.baseApi + "api/Appointments?" + param);
  }

  getUserList() {
    return this.http.get(this.baseApi + "api//");
  }


  createUpdateAppointment(appoint: AppointmentDisplay, id: string) {
    if (id == null) {
      return this.http.post<AppointmentDisplay>(this.baseApi + "api/Appointments/", appoint);
    } else {
      return this.http.put<AppointmentDisplay>(this.baseApi + "api/Appointments/" + id, appoint);
    }
  }

  getPaged(val: any): Observable<AppointmentPaging> {
    return this.http.get<AppointmentPaging>(this.baseApi + "api/Appointments", { params: val });
  }

  create(val: any) {
    return this.http.post(this.baseApi + "api/Appointments", val);
  }

  update(id: string, val: any) {
    return this.http.put(this.baseApi + "api/Appointments/" + id, val);
  }

  get(id: string) {
    return this.http.get(this.baseApi + "api/Appointments/" + id);
  }

  defaultGet(val: any) {
    return this.http.post(this.baseApi + "api/Appointments/DefaultGet", val);
  }

  searchRead(val: any): Observable<AppointmentBasic[]> {
    return this.http.post<AppointmentBasic[]>(this.baseApi + "api/Appointments/SearchRead", val);
  }

  searchReadByDate(val: AppointmentSearchByDate): Observable<PagedResult2<AppointmentBasic>> {
    return this.http.post<PagedResult2<AppointmentBasic>>(this.baseApi + "api/Appointments/SearchReadByDate", val);
  }

  checkForthcoming() {
    return this.http.get(this.baseApi + "api/Appointments/CheckForthcoming");
  }

  getBasic(id): Observable<AppointmentBasic> {
    return this.http.get<AppointmentBasic>(this.baseApi + "api/Appointments/" + id + '/GetBasic');
  }

  removeAppointment(id) {
    return this.http.delete(this.baseApi + "api/Appointments/" + id);
  }

  patch(id, val) {
    return this.http.patch(this.baseApi + "api/Appointments/" + id, val);
  }

  //cập nhật các cuộc hẹn quá hạn
  // patchMulti(val) {
  //   return this.http.patch(this.baseApi + "api/Appointments/patchMulti", val);
  // }

}
