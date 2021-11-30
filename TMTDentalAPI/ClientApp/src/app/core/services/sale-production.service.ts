import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

export class UpdateSaleProductionReq {
  orderId: string;
}

@Injectable({
  providedIn: 'root'
})
export class SaleProductionService {
  apiUrl = 'api/SaleProductions';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  updateSaleProduction(val) {
    return this.http.post(this.baseApi + this.apiUrl + '/UpdateSaleProduction', val);
  }
}
