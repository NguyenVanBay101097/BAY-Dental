import { Injectable, Inject } from '@angular/core';
import { HttpParams, HttpClient } from '@angular/common/http';
import { EmployeePaged, PagedResult2, EmployeeBasic, EmployeeDisplay, EmployeeSimple } from './employee';
import { Observable } from 'rxjs';

export class EmployeeSurveyDisplay{
  id : string;
  ref : string;
  name : string;
  phone : string;
  companyName : string;
  email : string;
  totalAssignment : number;
  doneAssignment: number;
  followAssignment : number;
}

@Injectable({
  providedIn: 'root'
})

export class EmployeeService {

  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  getEmployeePaged(empPaged: any): Observable<PagedResult2<EmployeeBasic>> {
    return this.http.get<PagedResult2<EmployeeBasic>>(this.baseApi + "api/Employees", { params: new HttpParams({ fromObject: empPaged }) });
  }

  getEmployee(id): Observable<EmployeeDisplay> {
    return this.http.get<EmployeeDisplay>(this.baseApi + "api/Employees/" + id);
  }

  deleteEmployee(id) {
    return this.http.delete(this.baseApi + "api/Employees/" + id);
  }

  createUpdateEmployee(emp: EmployeeDisplay, id: string) {
    if (id == null) {
      return this.http.post(this.baseApi + "api/Employees/", emp);
    } else {
      return this.http.put(this.baseApi + "api/Employees/" + id, emp);
    }
  }

  createEmployeeCategory(categ) {
    return this.http.post(this.baseApi + "api/EmployeeCategories", categ);
  }

  getEmployeeSimpleList(val): Observable<EmployeeSimple[]> {
    return this.http.post<EmployeeSimple[]>(this.baseApi + "api/Employees/Autocomplete", val);
  }

  getSearchRead(empPaged: any): Observable<PagedResult2<EmployeeBasic>> {
    return this.http.post<PagedResult2<EmployeeBasic>>(this.baseApi + "api/Employees" + "/SearchRead", empPaged);
  }

  actionActive(id: string, active: boolean) {
    return this.http.post<any>(this.baseApi + "api/Employees" + "/ActionActive/"+ id, {active:active});
  }

  getAllowSurveyList() {
    return this.http.get(this.baseApi + "api/Employees/AllowSurveyList");
  }
    
  GetEmployeeSurveyCount(val): Observable<PagedResult2<EmployeeSurveyDisplay>>{
    return this.http.get<PagedResult2<EmployeeSurveyDisplay>>(this.baseApi + 'api/Employees' + '/GetEmployeeSurveyCount' , {params: new HttpParams({fromObject: val})});
  }

}
