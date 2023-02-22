import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class QuotationPromotionService {
  apiUrl = 'api/QuotationPromotions'
  constructor(
    private http: HttpClient, @Inject('BASE_API') private baseApi: string
  ) { }

  removePromotion(ids: string[]) {
    return this.http.post(this.baseApi + this.apiUrl + '/RemovePromotion', ids);
  }
}
