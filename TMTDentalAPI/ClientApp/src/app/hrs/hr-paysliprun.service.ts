import { EmployeeBasic } from './../employees/employee';
import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HrPayslipDisplay } from './hr-payslip.service';


export class HrPayslipRunBasic {
  id: string;
  name: string;
  dateStart: string;
  dateEnd: string;
  totalAmount: number;
  state: string;
}

export class HrPayslipRunDisplay {
  id: string;
  name: string;
  dateStart: string;
  dateEnd: string;
  state: string;
  companyId: string;
  payslipCount: number;
}

export class HrPayslipRunPaged {
  limit: number;
  offset: number;
  search: string;
}

export class HrPayslipRunPaging {
  offset: number;
  limit: number;
  totalItems: number;
  items: HrPayslipRunBasic[];
}

export class HrPayslipRunSave {
  name: string;
  dateStart: string;
  dateEnd: string;
  companyId: string;
  state: string;
}

export class PaySlipRunConfirmViewModel {
  payslipRunId: string;
  structureId: string;
  empIds: string[];
}

export class HrPayslipRunDefaultGet {
  state: string;
}

@Injectable({
  providedIn: 'root'
})
export class HrPaysliprunService {

  constructor(private http: HttpClient, @Inject("BASE_API") private base_api: string) { }
  apiUrl = "api/HrPayslipRuns";
  apiPrintUrl = "SalaryEmployee";

  getPaged(val): Observable<HrPayslipRunPaging> {
    return this.http.get<HrPayslipRunPaging>(this.base_api + this.apiUrl, { params: val });
  }

  get(id): Observable<HrPayslipRunDisplay> {
    return this.http.get<HrPayslipRunDisplay>(this.base_api + this.apiUrl + '/' + id);
  }

  default(val: HrPayslipRunDefaultGet) {
    return this.http.post(this.base_api + this.apiUrl + '/DefaultGet', val);
  }

  create(val: HrPayslipRunSave) {
    return this.http.post(this.base_api + this.apiUrl, val);
  }

  actionConfirm(id) {
    return this.http.post(this.base_api + this.apiUrl + '/ActionConfirm/' + id, null);
  }

  actionDone(ids: string[]) {
    return this.http.post(this.base_api + this.apiUrl + '/ActionDone', ids);
  }

  actionCancel(ids: string[]) {
    return this.http.post(this.base_api + this.apiUrl + '/ActionCancel', ids);
  }

  update(id, val: HrPayslipRunSave) {
    return this.http.put(this.base_api + this.apiUrl + '/' + id, val);
  }

  delete(id) {
    return this.http.delete(this.base_api + this.apiUrl + '/' + id);
  }

  CreatePayslipByRunId(id) {
    return this.http.post(this.base_api + this.apiUrl + '/CreatePayslipByRunId/' + id, null);
  }

  ReComputeSalary(id) {
    return this.http.post(this.base_api + this.apiUrl + '/ComputeSalaryByRunId/' + id, null);
  }

  CheckExist(d: Date) {
    return this.http.post(this.base_api + this.apiUrl + '/CheckExist?date=' + d.toISOString(), null);
  }

  // printAllEmpSalary(id: string, val: any) {
  //   return this.http.put(this.base_api + this.apiUrl + `/${id}/Print`, val);
  // }

  printAllEmpSalary(id: string, val : any ) {
    debugger
    return this.http.put(this.base_api + this.apiPrintUrl + '/Print' + '/' + id, val);
  }

  ExportExcelFile(payslipIds: string[]) {
    return this.http.post(this.base_api + this.apiUrl + '/ExportExcelFile', payslipIds, { responseType: 'blob' }
    );
  }

}
