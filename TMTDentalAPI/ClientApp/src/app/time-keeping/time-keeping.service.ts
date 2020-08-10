import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EmployeeSimple, EmployeeBasic } from '../employees/employee';

export class ChamCongSave {
  employeeId: string;
  timeIn: string;
  timeOut: string;
  hourWorked: number;
  status: string;
}

export class EmployeeChamCongPaged {
  limit: number;
  offset: number;
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
  employee: EmployeeBasic;
  employeeId: string;
}

export class TimeSheetEmployee {
  date: Date;
  chamCong: ChamCongBasic;
  empId: string;
}

export class TimeKeepingSave {

}

@Injectable({
  providedIn: 'root'
})
export class TimeKeepingService {

  constructor(private http: HttpClient, @Inject("BASE_API") private base_api: string) { }
  apiUrl = "api/ChamCongs";

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

  CreateSetupChamcong(val)
  {
    return this.http.post(this.base_api + 'api/SetupChamcongs', val);
  }

  UpdateSetupChamcong(id, val)
  {
    return this.http.put(this.base_api + 'api/SetupChamcongs/' + id, val);
  }
}
