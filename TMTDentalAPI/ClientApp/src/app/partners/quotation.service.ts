import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedResult2 } from '../core/paged-result-2';

// import { PagedResult2 } from '../paged-result-2';

export class QuotationBasic {
  id: string;
  name: string;
}
@Injectable({
  providedIn: 'root'
})
export class QuotationService {
  apiUrl = 'api/';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }
  getPaged(val: any): Observable<PagedResult2<QuotationBasic>> {
    return this.http.get<PagedResult2<QuotationBasic>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
  }
}
