import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PagedResult2 } from '../core/paged-result-2';

export class phieuThuChiPaged { 
  limit: number;
  offset: number;
  search: string;
  type: string;
}

export class phieuThuChiBasic { 
  id: string;
  name: string;
  date: Date; 
  payerReceiver: string; 
  typeName: string;
  journalName: string;
  amount: number;
  state: string;
}

export class phieuThuChiDefault { 
  type: string;
}

export class phieuThuChiSave { 
  companyId: string;
  company: any;
  date: Date; 
  journalId: string;
  journal: any; 
  state: string; 
  name: string;
  type: string;
  amount: number;
  communication: string;
  reason: string;
  payerReceiver: string;
  address: string;
  loaiThuChiId: string;
  loaiThuChi: any;
}

export class phieuThuChi { 
  id: string;
  companyId: string;
  company: any;
  date: Date; 
  journalId: string;
  journal: any; 
  state: string; 
  name: string;
  type: string;
  amount: number;
  communication: string;
  reason: string;
  payerReceiver: string;
  address: string;
  loaiThuChiId: string;
  loaiThuChi: any;
}

@Injectable({
  providedIn: 'root'
})
export class PhieuThuChiService {
  apiUrl = 'api/PhieuThuChis';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  getPaged(val: any): Observable<PagedResult2<phieuThuChiBasic>> {
    return this.http.get<PagedResult2<phieuThuChiBasic>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
  }

  get(id): Observable<phieuThuChi> {
    return this.http.get<phieuThuChi>(this.baseApi + this.apiUrl + "/" + id);
  }

  create(val) {
    return this.http.post(this.baseApi + this.apiUrl, val);
  }

  update(id: string, val) {
    return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
  }

  actionConfirm(ids: string[]) {
    return this.http.post(this.baseApi + this.apiUrl + '/ActionConfirm', ids);
}

  actionCancel(ids: string[]) {
    return this.http.post(this.baseApi + this.apiUrl + '/ActionCancel', ids);
  }

  delete(id: string) {
    return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
  }
}
