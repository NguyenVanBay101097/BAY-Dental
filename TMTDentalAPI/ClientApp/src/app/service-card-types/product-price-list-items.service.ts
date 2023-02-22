import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ProductPriceListItemsService {
  apiUrl = 'api/ProductPricelistItems';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  delete(id: string) {
    return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
  }
}
