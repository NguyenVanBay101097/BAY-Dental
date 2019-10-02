import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EmployeeCategoryDisplay, EmployeeCategoryBasic, EmployeeCategoryPaged, PagedResult2 } from './emp-category';

@Injectable({
  providedIn: 'root'
})
export class EmpCategoryService {

  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  createUpdateEmployeeCategory(empCat: EmployeeCategoryDisplay, id: string): Observable<EmployeeCategoryDisplay> {
    if (id == null) {
      return this.http.post<EmployeeCategoryDisplay>(this.baseApi + "api/EmployeeCategories/", empCat);
    } else {
      return this.http.put<EmployeeCategoryDisplay>(this.baseApi + "api/EmployeeCategories/" + id, empCat);
    }
  }

  autocompleteCategoryTypes(val: EmployeeCategoryPaged): Observable<EmployeeCategoryBasic[]> {
    return this.http.post<EmployeeCategoryBasic[]>(this.baseApi + "api/EmployeeCategories/autocomplete", val);
  }

  autocompleteCategoryTypes2(val: EmployeeCategoryPaged): Observable<EmployeeCategoryDisplay[]> {
    return this.http.post<EmployeeCategoryDisplay[]>(this.baseApi + "api/EmployeeCategories/autocomplete2", val);
  }

  getCategEmployeePaged(empCatPaged: EmployeeCategoryPaged): Observable<PagedResult2<EmployeeCategoryBasic>> {
    var params = new HttpParams()
      .set('offset', empCatPaged.offset.toString())
      .set('limit', empCatPaged.limit.toString())
    if (empCatPaged.search) {
      params = params.set('search', empCatPaged.search);
    };
    return this.http.get<PagedResult2<EmployeeCategoryBasic>>(this.baseApi + "api/EmployeeCategories?" + params);
  }

  getCategEmployee(id): Observable<EmployeeCategoryDisplay> {
    return this.http.get<EmployeeCategoryDisplay>(this.baseApi + "api/EmployeeCategories/" + id);
  }

  deleteCategEmployee(id) {
    return this.http.delete(this.baseApi + "api/EmployeeCategories/" + id);
  }
}
