import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class QuotationLineService {
  apiUrl = 'api/QuotationLines';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  applyDiscountOnQuotationLine(val){
    return this.http.post(this.baseApi + this.apiUrl + '/ApplyDiscountOnQuotationLine', val);
}
}
