import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PagedResult2 } from '../employee-categories/emp-category';
import { ResBankBasic } from './res-bank';

@Injectable({
  providedIn: 'root'
})
export class ResBankService {

  apiUrl = 'api/ResBanks';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  getPaged(val): Observable<PagedResult2<ResBankBasic>> {
    return this.http.get<PagedResult2<ResBankBasic>>(this.baseApi + this.apiUrl, { params: val });
  }

  getById(id): Observable<ResBankBasic> {
    return this.http.get<ResBankBasic>(this.baseApi + this.apiUrl + '/' + id);
  }

  deleteBank(id) {
    return this.http.delete(this.baseApi + this.apiUrl + '/' + id);
  }

  createUpdate(val, id) {
    if (id) {
      return this.http.put(this.baseApi + this.apiUrl + '/' + id, val);
    } else {
      return this.http.post(this.baseApi + this.apiUrl, val);
    }
  }
}
