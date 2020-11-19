import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

export class SalaryPaymentPaged { 
  limit: number;
  offset: number;
  search: string;
  type: string;
}

@Injectable({
  providedIn: 'root'
})
export class SalaryPaymentService {
  apiUrl = 'api/SalaryPayment';
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
}
