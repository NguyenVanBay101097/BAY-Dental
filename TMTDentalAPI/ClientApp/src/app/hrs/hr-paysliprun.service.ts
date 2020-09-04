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

export class HrPayslipRunDisplay{
  id: string;
  name: string;
  dateStart: string;
  dateEnd: string;
  state: string;
  payslipCount: number;
  slips: HrPayslipDisplay[];
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

export class PaySlipRunConfirmViewModel{
  payslipRunId: string;
  structureId: string;
  empIds: string[];
}

export class HrPayslipRunDefaultGet{
  state: string;
}

@Injectable({
  providedIn: 'root'
})
export class HrPaysliprunService {

  constructor(private http: HttpClient, @Inject("BASE_API") private base_api: string) { }
  apiUrl = "api/HrPayslipRuns";

  getPaged(val): Observable<HrPayslipRunPaging> {
    return this.http.get<HrPayslipRunPaging>(this.base_api + this.apiUrl, { params: val });
  }

  get(id): Observable<HrPayslipRunDisplay> {
    return this.http.get<HrPayslipRunDisplay>(this.base_api + this.apiUrl + '/' + id);
  }

  default(val: HrPayslipRunDefaultGet){
    return this.http.post(this.base_api + this.apiUrl + '/DefaultGet' , val);
  }

  create(val: HrPayslipRunSave ) {
    return this.http.post(this.base_api + this.apiUrl, val);
  }

  actionConfirm(val: PaySlipRunConfirmViewModel ) {
    return this.http.post(this.base_api + this.apiUrl + '/ActionConfirm', val);
  }

  actionDone(ids : string[]) {
    return this.http.post(this.base_api + this.apiUrl + '/ActionDone', ids);
  }

  update(id, val: HrPayslipRunSave) {
    return this.http.put(this.base_api + this.apiUrl + '/' + id, val);
  }

  delete(id) {
    return this.http.delete(this.base_api + this.apiUrl + '/' + id);
  }

}
