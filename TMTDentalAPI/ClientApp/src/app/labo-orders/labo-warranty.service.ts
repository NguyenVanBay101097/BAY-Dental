import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedResult2 } from '../core/paged-result-2';

export class LaboWarrantyPaged {
  limit: number;
  offset: number;
  search: string;
  states: string;
  dateReceiptFrom: string;
  dateReceiptTo: string;
  supplierId: string;
  laboOrderId: string;
  notDraft: boolean;
}

export class LaboWarrantySave {
  name: string;
  laboOrderId: string;
  employeeId: string;
  dateReceiptWarranty: string;
  teeth: any[];
  reason: string;
  content: string;
  note: string;
  state: string;
}

export class LaboWarrantyBasic {
  id: string;
  name: string;
  laboOrderId: string;
  laboOrderName: string;
  dateReceiptWarranty: string;
  dateSendWarranty: string;
  dateReceiptInspection: string;
  dateAssemblyWarranty: string;
  reason: string;
  content: string;
  state: string;
}

@Injectable({
  providedIn: 'root'
})
export class LaboWarrantyService {
  apiUrl = 'api/LaboWarranties';

  constructor(
    private http: HttpClient,
    @Inject('BASE_API') private baseApi: string
  ) { }

  getPaged(val: any): Observable<PagedResult2<LaboWarrantyBasic>> {
    return this.http.get<PagedResult2<LaboWarrantyBasic>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) })
  }

  get(id: string) {
    return this.http.get(this.baseApi + this.apiUrl + '/' + id)
  }

  getDefault(val) {
    return this.http.post(this.baseApi + this.apiUrl + "/GetDefault", val);
  }

  create(val: LaboWarrantySave): Observable<LaboWarrantySave> {
    return this.http.post<LaboWarrantySave>(this.baseApi + this.apiUrl, val);
  }

  update(id, val: LaboWarrantySave): Observable<LaboWarrantySave> {
    return this.http.put<LaboWarrantySave>(this.baseApi + this.apiUrl + '/' + id, val);
  }

  buttonCancel(ids: string[]) {
    return this.http.post(this.baseApi + this.apiUrl + '/ButtonCancel', ids);
  }

  buttonConfirm(ids: string[]) {
    return this.http.post(this.baseApi + this.apiUrl + '/ButtonConfirm', ids);
  }

  delete(id: string) {
    return this.http.delete(this.baseApi + this.apiUrl + '/' + id);
  }

  confirmSendWarranty(val) {
    return this.http.post(this.baseApi + this.apiUrl + '/ConfirmSendWarranty', val);
  }

  cancelSendWarranty(ids: string[]) {
    return this.http.post(this.baseApi + this.apiUrl + '/CancelSendWarranty', ids);
  }

  confirmReceiptInspection(val) {
    return this.http.post(this.baseApi + this.apiUrl + '/ConfirmReceiptInspection', val);
  }

  cancelReceiptInspection(ids: string[]) {
    return this.http.post(this.baseApi + this.apiUrl + '/CancelReceiptInspection', ids);
  }

  confirmAssemblyWarranty(val) {
    return this.http.post(this.baseApi + this.apiUrl + '/ConfirmAssemblyWarranty', val);
  }

  cancelAssemblyWarranty(ids: string[]) {
    return this.http.post(this.baseApi + this.apiUrl + '/CancelAssemblyWarranty', ids);
  }

  exportExcelFile(val: any) {
    return this.http.post(
        this.baseApi + this.apiUrl + "/GetExcelFile", val,
        { responseType: "blob" }
    );
}
}
