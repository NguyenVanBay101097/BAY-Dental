import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { ResInsurancePaymentSave } from './res-insurance.model';

@Injectable({
  providedIn: 'root'
})
export class ResInsurancePaymentService {
  readonly apiUrl = 'api/ResInsurancePayments';

  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  create(val){
    return this.http.post(this.baseApi + this.apiUrl, val);
  }

  actionPayment(ids){
    return this.http.post(this.baseApi + this.apiUrl + '/ActionPayment', ids);
  }
}
