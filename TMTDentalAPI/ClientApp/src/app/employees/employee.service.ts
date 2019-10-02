import { Injectable, Inject } from '@angular/core';
import { HttpParams, HttpClient } from '@angular/common/http';
import { EmployeePaged, PagedResult2, EmployeeBasic, EmployeeDisplay, EmployeeSimple } from './employee';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {

  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  getEmployeePaged(empPaged: EmployeePaged): Observable<PagedResult2<EmployeeBasic>> {
    var params = new HttpParams()
      .set('offset', empPaged.offset.toString())
      .set('limit', empPaged.limit.toString())
    if (empPaged.search) {
      params = params.set('search', empPaged.search);
    };
    if (empPaged.position) {
      params = params.set('position', empPaged.position);
    };

    return this.http.get<PagedResult2<EmployeeBasic>>(this.baseApi + "api/Employees?" + params);
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


}
