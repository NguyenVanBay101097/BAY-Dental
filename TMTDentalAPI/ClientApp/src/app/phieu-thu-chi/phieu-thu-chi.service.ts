import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PagedResult2 } from '../core/paged-result-2';

export class PhieuThuChiPaged {
  limit: number;
  offset: number;
  search: string;
  type: string;
  dateFrom: string;
  dateTo: string;
  companyId: string;
}

export class PhieuThuChiSearch {
  type: string;
  dateTo: string;
  dateFrom: string;
  companyId: string;
}

export class PhieuThuChiBasic {
  id: string;
  name: string;
  date: Date;
  payerReceiver: string;
  typeName: string;
  journalName: string;
  amount: number;
  state: string;
}

export class PhieuThuChiReport {
  id: string;
  name: string;
  type: string;
  amount: number;
}

export class PhieuThuChiDefault {
  type: string;
}

export class PhieuThuChiSave {
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

export class PhieuThuChi {
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

  getPaged(val: any) {
    return this.http.get(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
  }

  get(id) {
    return this.http.get(this.baseApi + this.apiUrl + "/" + id);
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

  defaultGet(val: any) {
    return this.http.post(this.baseApi + this.apiUrl + '/DefaultGet', val);
  }

  reportPhieuThuChi(val: any): Observable<PhieuThuChiReport[]> {
    return this.http.get<PhieuThuChiReport[]>(this.baseApi + this.apiUrl + '/ReportPhieuThuChi', { params: val });
  }

  getPrint(id: string) {
    return this.http.get(this.baseApi + this.apiUrl+ '/' + id + '/GetPrint');
  }
}
