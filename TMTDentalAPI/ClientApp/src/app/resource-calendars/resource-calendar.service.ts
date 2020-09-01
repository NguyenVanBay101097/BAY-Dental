import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export class ResourceCalendarPaged {
  limit: number;
  offset: number;
  search: string;
}

export class ResourceCalendarPaging {
  items: ResourceCalendarBasic[];
  limit: number;
  offset: number;
  totalItems: number;
}

export class ResourceCalendarSave {
  name: string;
  hoursPerDay: number;
}

export class ResourceCalendarBasic {
  id: string;
  name: string;
}

export class ResourceCalendarDisplay {
  id: string;
  name: string;
  hoursPerDay: number;
  resourceCalendarAttendances: ResourceCalendarAttendanceDisplay[];
}

export class ResourceCalendarAttendancePaged {
  limit: number;
  offset: number;
  resourceCalendarId: string;
  search: string;
}

export class ResourceCalendarAttendancePaging {
  items: ResourceCalendarBasic[];
  limit: number;
  offset: number;
  totalItems: number;
}

export class ResourceCalendarAttendanceSave {
  name: string;
  dayOfWeeks: string;
  hourFrom: number;
  hourTo: number;
  dayPeriod: string;
  calendarId: string;
}

export class ResourceCalendarAttendanceDisplay {
  id: string;
  name: string;
  dayOfWeek: string;
  hourFrom: number;
  hourTo: number;
  dayPeriod: string;
  calendarId: string;
}

export class ResourceCalendarAttendanceBasic {
  id: string;
  name: string;
}

@Injectable({
  providedIn: 'root'
})
export class ResourceCalendarService {

  constructor(
    private http: HttpClient,
    @Inject("BASE_API") private base_api: string
  ) { }
  apiUrl = "api/ResourceCalendars";
  apiUrlAttendance = "api/ResourceCalendarAttendances";

  create(val): Observable<ResourceCalendarBasic> {
    return this.http.post<ResourceCalendarBasic>(this.base_api + this.apiUrl, val);
  }

  update(id, val) {
    return this.http.put(this.base_api + this.apiUrl + '/' + id, val);
  }

  delete(id) {
    return this.http.delete(this.base_api + this.apiUrl + '/' + id);
  }

  getPage(val): Observable<ResourceCalendarPaging> {
    return this.http.get<ResourceCalendarPaging>(this.base_api + this.apiUrl, { params: val });
  }

  get(id): Observable<ResourceCalendarDisplay> {
    return this.http.get<ResourceCalendarDisplay>(this.base_api + this.apiUrl + '/' + id);
  }

  createAtt(val): Observable<ResourceCalendarAttendanceBasic> {
    return this.http.post<ResourceCalendarBasic>(this.base_api + this.apiUrlAttendance, val);
  }

  updateAtt(id, val) {
    return this.http.put(this.base_api + this.apiUrlAttendance + '/' + id, val);
  }

  deleteAtt(id) {
    return this.http.delete(this.base_api + this.apiUrlAttendance + '/' + id);
  }

  getPageAtt(val): Observable<ResourceCalendarAttendancePaging> {
    return this.http.get<ResourceCalendarAttendancePaging>(this.base_api + this.apiUrlAttendance, { params: val });
  }

  getAtt(id): Observable<ResourceCalendarAttendanceDisplay> {
    return this.http.get<ResourceCalendarAttendanceDisplay>(this.base_api + this.apiUrlAttendance + '/' + id);
  }

  // GetListResourceCalendadrAtt(val): Observable<ResourceCalendarAttendanceDisplay[]> {
  //   return this.http.get<ResourceCalendarAttendanceDisplay[]>(this.base_api + this.apiUrlAttendance + '/GetListResourceCalendadrAtt', { params: val });
  // }

  setSequence(vals) {
    return this.http.post(this.base_api + this.apiUrlAttendance + '/SetSequence', vals);
  }

  DefaultGet() {
    return this.http.get(this.base_api + this.apiUrl + '/DefaultGet');
  }
}
