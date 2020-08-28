import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HrPayslipDisplay } from './hr-payslip.service';


export class HrPayslipRunBasic {
  id: string;
  name: string;
  dateStart: string;
  dateEnd: string;
  state: string;
}

export class HrPayslipRunDisplay{
  id: string;
  name: string;
  dateStart: string;
  dateEnd: string;
  state: string;
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

  delete(id) {
    return this.http.delete(this.base_api + this.apiUrl + '/' + id);
  }

  get(id): Observable<HrPayslipRunDisplay> {
    return this.http.get<HrPayslipRunDisplay>(this.base_api + this.apiUrl + '/' + id);
  }

  create(val: HrPayslipRunSave ) {
    return this.http.post(this.base_api + this.apiUrl, val);
  }

  update(id, val: HrPayslipRunSave) {
    return this.http.put(this.base_api + this.apiUrl + '/' + id, val);
  }

}
