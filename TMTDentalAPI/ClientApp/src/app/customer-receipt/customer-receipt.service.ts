import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class CustomerReceiptService {

  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  create(val: any) {
    return this.http.post(this.baseApi + "api/CustomerReceipts", val);
  }

  updateState(id: string, val: any) {
    return this.http.patch(this.baseApi + "api/CustomerReceipts/PatchState" + id, val);
  }
}
