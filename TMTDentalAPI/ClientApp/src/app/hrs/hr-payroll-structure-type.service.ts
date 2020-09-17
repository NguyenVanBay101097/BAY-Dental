import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ResourceCalendarBasic } from '../resource-calendars/resource-calendar.service';
import { Observable } from 'rxjs';

export class HrPayrollStructureTypeBasic {
  id: string;
  name: string;
}

export class HrPayrollStructureTypeSimple {
  id: string;
  name: string;
  wageType: string;
}

export class HrPayrollStructureTypePaged {
  limit: number;
  offset: number;
  search: string;
}

export class HrpayRollStructureTypePaging {
  limit: number;
  offset: number;
  totalItems: number;
  items: HrPayrollStructureTypeBasic[];
}

export class HrPayrollStructureTypeSave {
  defaultResourceCalendarId: string
  defaultSchedulePay: string;
  defaultStructId: string;
  defaultWorkEntryTypeId: string;
  name: string;
  wageType: string
}

export class HrPayrollStructureTypeDisplay {
  id: string;
  name: string;
  defaultSchedulePay: string;
  defaultResourceCalendarId: string;
  defaultWorkEntryTypeId: string;
  wageType: string;
}

@Injectable({
  providedIn: 'root'
})
export class HrPayrollStructureTypeService {

  constructor(
    private http: HttpClient,
    @Inject("BASE_API") private base_api: string
  ) { }

  apiUrl = "api/HrPayrollStructureTypes";

  getPaged(val): Observable<HrpayRollStructureTypePaging> {
    return this.http.get<HrpayRollStructureTypePaging>(this.base_api + this.apiUrl, { params: val });
  }

  create(val): Observable<HrPayrollStructureTypeSave> {
    return this.http.post<HrPayrollStructureTypeSave>(this.base_api + this.apiUrl, val);
  }

  update(id, val) {
    return this.http.put(this.base_api + this.apiUrl + '/' + id, val);
  }

  getId(id): Observable<HrPayrollStructureTypeDisplay> {
    return this.http.get<HrPayrollStructureTypeDisplay>(this.base_api + this.apiUrl + '/' + id);
  }

  delete(id) {
    return this.http.delete(this.base_api + this.apiUrl + '/' + id);
  }

  autocomplete(val: HrPayrollStructureTypePaged): Observable<HrPayrollStructureTypeSimple[]> {
    return this.http.post<HrPayrollStructureTypeSimple[]>(this.base_api + this.apiUrl + "/Autocomplete", val);
  }
}
