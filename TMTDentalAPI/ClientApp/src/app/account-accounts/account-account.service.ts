import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PagedResult2 } from '../core/paged-result-2';

export class accountAccountPaged { 
  limit: number;
  offset: number;
  search: string;
  type: string;
}

export class accountAccountBasic { 
  id: string;
  name: string;
  companyName: string;
  code: string;
  active: boolean;
  note: string;
}

export class accountAccountDefault { 
  type: string;
}

export class accountAccountDefaultGet { 
  name: string;
  code: string;
  active: boolean;
  note: string;
  companyId: string;
  company: any;
  internalType: string;
  reconcile: boolean;
  userType: any;
  userTypeId: string;
}

@Injectable({
  providedIn: 'root'
})
export class AccountAccountService {
  apiUrl = 'api/AccountAccounts';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  getPaged(val: any): Observable<PagedResult2<accountAccountBasic>> {
    return this.http.get<PagedResult2<accountAccountBasic>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
  }

  get(id): Observable<accountAccountBasic> {
    return this.http.get<accountAccountBasic>(this.baseApi + this.apiUrl + "/" + id);
}

  defaultGet(val: accountAccountDefault): Observable<accountAccountDefaultGet> {
    return this.http.post<accountAccountDefaultGet>(this.baseApi + this.apiUrl + "/defaultget", val);
  }

  create(val) {
    return this.http.post(this.baseApi + this.apiUrl, val);
  }

  update(id: string, val) {
    return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
  }

  delete(id: string) {
    return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
  }
}
