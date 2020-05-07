import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export class UoMCategoryBasic {
  id: string;
  name: string;
}

export class UoMCategoryPaging {
  offset: number;
  limit: number;
  totalItems: number;
  items: UoMCategoryBasic[];
}

export class UoMCategoryPaged {
  search: string;
  limit: number;
  offset: number;
}

@Injectable({
  providedIn: 'root'
})
export class UomCategoryService {

  constructor(@Inject('BASE_API') private base_api: string, private http: HttpClient) { }
  private readonly apiUrl = "api/UoMCategories"

  create(value): Observable<UoMCategoryBasic> {
    return this.http.post<UoMCategoryBasic>(this.base_api + this.apiUrl, value);
  }

  update(id, value) {
    return this.http.put(this.base_api + this.apiUrl + '/' + id, value);
  }

  getPaged(value): Observable<UoMCategoryPaging> {
    return this.http.get<UoMCategoryPaging>(this.base_api + this.apiUrl);
  }

  get(id): Observable<UoMCategoryBasic> {
    return this.http.get<UoMCategoryBasic>(this.base_api + this.apiUrl + '/' + id);
  }

  delete(id) {
    return this.http.delete(this.base_api + this.apiUrl + '/' + id);
  }

}
