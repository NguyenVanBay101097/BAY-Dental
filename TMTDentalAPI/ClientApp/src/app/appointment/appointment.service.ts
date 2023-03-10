import { formatDate } from '@angular/common';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedResult2 } from '../partners/partner-simple';
import { AppointmentBasic, AppointmentDisplay, AppointmentPaged, AppointmentSearchByDate } from './appointment';

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
    if (appointmentPaged.companyId != null) {
      param = param.set('companyId', appointmentPaged.companyId);
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

  getPaged(val: any) {
    return this.http.get(this.baseApi + "api/Appointments", { params: new HttpParams({ fromObject: val }) });
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

  getCountState(val: any) {
    return this.http.post(this.baseApi + "api/Appointments/CountAppointment", val);
  }

  getCount(val: any) {
    return this.http.post(this.baseApi + "api/Appointments/Count", val);
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

  patchState(id, val): Observable<any> {
    // const body = [{
    //   op: "replace",
    //   path: "/PatchState",
    //   value: val
    // }];
    return this.http.patch(this.baseApi + "api/Appointments/" + id + '/PatchState', val);
  }

  exportExcel(val) {
    return this.http.get(this.baseApi + 'api/Appointments/ExportExcel', { params: val, responseType: 'blob' });
  }

  exportExcel2(val) {
    return this.http.get(this.baseApi + 'api/Appointments/ExportExcel2', { params: val, responseType: 'blob' });
  }

  getListDoctor(val) {
    return this.http.get<any[]>(this.baseApi + 'api/Appointments/GetListDoctor', { params: new HttpParams({ fromObject: val }) });
  }

  print(id) {
    return this.http.get(this.baseApi + 'api/Appointments/' + id + '/Print');
  }

  //c???p nh???t c??c cu???c h???n qu?? h???n
  // patchMulti(val) {
  //   return this.http.patch(this.baseApi + "api/Appointments/patchMulti", val);
  // }

}
