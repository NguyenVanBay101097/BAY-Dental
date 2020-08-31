import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EmployeeSimple, EmployeeBasic, EmployeeDisplay } from '../employees/employee';
import { WorkEntryType } from '../work-entry-types/work-entry-type.service';

export class ChamCongSave {
  employeeId: string;
  timeIn: string;
  timeOut: string;
  date: string;
  hourWorked: number;
  workEntryTypeId: string;
  status: string;
}

export class EmployeeChamCongPaged {
  status: string;
  employeeId: string
  from: string;
  to: string;
}

export class EmployeeChamCongPaging {
  offset: number;
  limit: number;
  totalItems: number;
  items: EmployeeDisplay[];
}

export class ChamCongBasic {
  id: string;
  timeIn: string;
  timeOut: string;
  status: string;
  hourWorked: number;
  date: string;
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

  getAllByEmpId(val): Observable<ChamCongBasic[]> {
    return this.http.get<ChamCongBasic[]>(this.base_api + this.apiUrl, { params: val });
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

  actionImport(val) {
    return this.http.post(this.base_api + this.apiUrl + '/ExcelImportCreate', val);
  }

  CreateSetupChamcong(val) {
    return this.http.post(this.base_api + 'api/SetupChamcongs', val);
  }

  UpdateSetupChamcong(id, val) {
    return this.http.put(this.base_api + 'api/SetupChamcongs/' + id, val);
  }

  getLastChamCong(val):Observable<ChamCongBasic> {
    return this.http.get<ChamCongBasic>(this.base_api + this.apiUrl + '/GetLastChamCong', { params: val });
  }


}
