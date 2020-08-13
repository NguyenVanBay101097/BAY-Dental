import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EmployeeSimple, EmployeeBasic } from '../employees/employee';

export class ChamCongSave {
  employeeId: string;
  timeIn: string;
  timeOut: string;
  date:string;
  hourWorked: number;
  workEntryTypeId: string;
  status: string;
}

export class EmployeeChamCongPaged {
  limit: number;
  offset: number;
  filter: string;
  from: string;
  to: string;
}

export class EmployeeChamCongPaging {
  offset: number;
  limit: number;
  totalItems: number;
  items: EmployeeBasic[];
}

export class ChamCongBasic {
  id: string;
  timeIn: string;
  timeOut: string;
  status: string;
  hourWorked: number;
  date:string;
  employee: EmployeeBasic;
  employeeId: string;
  workEntryTypeId: string;
  workEntryType: WorkEntryType;
  dateCreated: string;
}

export class TimeSheetEmployee {
  date: Date;
  chamCongs: ChamCongBasic[];
  empId: string;
}

export class TimeKeepingSave {

}

export class WorkEntryType {
  id: string;
  name: string;
  isHasTimeKeeping: boolean;
  color: string;
}

export class WorkEntryTypePage {
  limit: number;
  offset: number;
  filter: string;
}

export class WorkEntryTypePaging {
  offset: number;
  limit: number;
  totalItems: number;
  items: WorkEntryType[];
}

export class WorkEntryTypeSave {
  name: string;
  color: string;
}

@Injectable({
  providedIn: 'root'
})
export class TimeKeepingService {

  constructor(private http: HttpClient, @Inject("BASE_API") private base_api: string) { }
  apiUrl = "api/ChamCongs";
  apiUrlWorkingEntryType = "api/WorkEntryTypes";
  create(val): Observable<ChamCongSave> {
    return this.http.post<ChamCongSave>(this.base_api + this.apiUrl, val);
  }

  update(id, val) {
    return this.http.put(this.base_api + this.apiUrl + '/' + id, val);
  }

  getEmpChamCong(val): Observable<EmployeeChamCongPaging> {
    return this.http.get<EmployeeChamCongPaging>(this.base_api + this.apiUrl, { params: val });
  }

  GetsetupTimeKeeping() {
    return this.http.get(this.base_api + 'api/SetupChamcongs');
  }

  get(id): Observable<ChamCongBasic> {
    return this.http.get<ChamCongBasic>(this.base_api + this.apiUrl + '/' + id);
  }

  exportTimeKeeping(val) {
    return this.http.post(this.base_api + this.apiUrl + "/ExportExcelFile", val,
      { responseType: "blob" });
  }

  CreateSetupChamcong(val) {
    return this.http.post(this.base_api + 'api/SetupChamcongs', val);
  }

  UpdateSetupChamcong(id, val) {
    return this.http.put(this.base_api + 'api/SetupChamcongs/' + id, val);
  }

  getPagedWorkEntryType(val): Observable<WorkEntryTypePaging> {
    return this.http.get<WorkEntryTypePaging>(this.base_api + this.apiUrlWorkingEntryType, { params: val });
  }

  deleteWorkEntryType(id) {
    return this.http.delete(this.base_api + this.apiUrlWorkingEntryType + '/' + id);
  }

  getWorkEntryType(id): Observable<WorkEntryType> {
    return this.http.get<WorkEntryType>(this.base_api + this.apiUrlWorkingEntryType + '/' + id);
  }

  createWorkEntryType(val) {
    return this.http.post(this.base_api + this.apiUrlWorkingEntryType, val);
  }

  updateWorkEntryType(id, val) {
    return this.http.put(this.base_api + this.apiUrlWorkingEntryType + '/'+id, val);
  }
}
