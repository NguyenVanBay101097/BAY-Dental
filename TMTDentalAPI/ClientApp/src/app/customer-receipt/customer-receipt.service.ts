import { EmployeeSimple } from './../employees/employee';
import { PartnerSimpleContact } from './../partners/partner-simple';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedResult2 } from '../employee-categories/emp-category';
import { ProductSimple } from '../products/product-simple';

export class CustomerReceiptPaged {
  limit: number;
  offset: number;
  search: string;
  dateFrom: string;
  dateTo: string;
  doctorId: string;
  state: string;
}

export class CustomerReceiptPaging {
  offset: number;
  limit: number;
  totalItems: number;
  items: CustomerReceiptBasic[];
}

export class CustomerReceiptBasic {
  id: string;
  dateWaiting: string;
  dateExamination: string;
  dateDone: string;
  doctorId: string;
  doctorName: string;
  partnerId: string;
  partnerName: string;
  state: string;
}

export class CustomerReceiptDisplay
{
    id: string;
    dateWaiting: string;
    dateExamination: string;
    dateDone: string;
    timeExpected: string;
    products: ProductSimple[];
    note: string;
    partnerId: string;
    partner: PartnerSimpleContact;
    companyId: string;
    doctorId: string;
    doctor: EmployeeSimple;
    state: string;
    reason: string;
    isRepeatCustomer: boolean;
    isNoTreatment: boolean;
}

export class CustomerReceiptSave
{
    dateWaiting: string;
    dateExamination: string;
    dateDone: string;
    timeExpected: string;
    products: ProductSimple[];
    note: string;
    partnerId: string;
    companyId: string;
    doctorId: string;
    state: string;
    reason: string;
    isRepeatCustomer: boolean;
    isNoTreatment: boolean;
}

export class CustomerReceiptGetCountVM {
  search: string;
  doctorId: string;
  state: string;
  dateFrom: string;
  dateTo: string;
}

export class CustomerReceiptStatePatch {
  state: string;
  isNoTreatment: string;
  dateExamination: string;
  dateDone: string;
  reason: string;
}

@Injectable({
  providedIn: 'root'
})
export class CustomerReceiptService {
  apiUrl = 'api/CustomerReceipts';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  getPaged(val: any): Observable<PagedResult2<CustomerReceiptBasic>> {
    return this.http.get<PagedResult2<CustomerReceiptBasic>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
  }

  get(id) {
    return this.http.get(this.baseApi + this.apiUrl + "/" + id);
  }

  create(val) {
    return this.http.post(this.baseApi + this.apiUrl, val);
  }

  update(id, val) {
    return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
  }

  getCount(val: any) {
    return this.http.post(this.baseApi + this.apiUrl + "/GetCount", val);
  }

  patchState(id, val): Observable<any> {
    return this.http.patch(this.baseApi + this.apiUrl + id + '/PatchState', val);
  }
}
