import { Injectable, Inject } from '@angular/core';
import { HistoryPaged, PagedResult2, HistorySimple } from './history';
import { Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class HistoryService {

  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  readonly apiUrl = "api/histories"
  getList(paged: HistoryPaged): Observable<PagedResult2<HistorySimple>> {
    var params = new HttpParams()
      .set('offset', paged.offset.toString())
      .set('limit', paged.limit.toString())
    if (paged.search) {
      params = params.set('search', paged.search);
    };
    return this.http.get<PagedResult2<HistorySimple>>(this.baseApi + this.apiUrl + '?' + params);
  }

  getById(id: string): Observable<HistorySimple> {
    return this.http.get<HistorySimple>(this.baseApi + this.apiUrl + '/' + id);
  }

  createUpdate(id: string, val: HistorySimple) {
    if (id) {
      console.log(1);
      return this.http.put(this.baseApi + this.apiUrl + '/' + id, val);
    }
    else {
      console.log(2);
      return this.http.post<HistorySimple>(this.baseApi + this.apiUrl, val);
    }
  }

  delete(id: string) {
    return this.http.delete(this.baseApi + this.apiUrl + '/' + id);
  }
}
