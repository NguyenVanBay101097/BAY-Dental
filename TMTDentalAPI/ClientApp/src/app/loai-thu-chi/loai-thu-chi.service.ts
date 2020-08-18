import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PagedResult2 } from '../core/paged-result-2';

export class loaiThuChiPaged { 
  limit: number;
  offset: number;
  search: string;
  type: string;
}

export class loaiThuChiBasic { 
  id: string;
  name: string;
  code: string;
  note: string;
  isInclude: boolean;
}

export class loaiThuChiDefault { 
  type: string;
}

export class loaiThuChiSave { 
  name: string;
  code: string;
  note: string;
  type: string;
  isInclude: boolean;
  accountId: string;
  account: any;
  companyId: string;
  company: any;
}

export class loaiThuChi { 
  id: string;
  name: string;
  code: string;
  note: string;
  type: string;
  isInclude: boolean;
  accountId: string;
  account: any;
  companyId: string;
  company: any;
}

@Injectable({
  providedIn: 'root'
})
export class LoaiThuChiService {
  apiUrl = 'api/LoaiThuChi';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  getPaged(val: any): Observable<PagedResult2<loaiThuChiBasic>> {
    return this.http.get<PagedResult2<loaiThuChiBasic>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
  }

  get(id): Observable<loaiThuChi> {
    return this.http.get<loaiThuChi>(this.baseApi + this.apiUrl + "/" + id);
  }

  defaultGet(val: loaiThuChiDefault): Observable<loaiThuChiSave> {
    return this.http.post<loaiThuChiSave>(this.baseApi + this.apiUrl + "/defaultget", val);
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
