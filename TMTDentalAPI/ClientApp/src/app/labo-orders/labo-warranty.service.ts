import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedResult2 } from '../core/paged-result-2';

export class LaboWarrantyPaged {
  limit: number;
  offset: number;
  search: string;
  state: string;
  dateReceiptFrom: string;
  dateReceiptTo: string;
  supplierId: string;
  laboOrderId: string;
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

  create(val: LaboWarrantySave): Observable<LaboWarrantySave> {
    return this.http.post<LaboWarrantySave>(this.baseApi + this.apiUrl, val);
  }

}
