import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

export class StockInventoryLineOnChangeCreateLine {
  productId: string;
  locationId: string;
}

@Injectable({
  providedIn: 'root'
})
export class StockInventoryLineService {

  constructor(private http: HttpClient, @Inject("BASE_API") private base_api: string) { }
  apiUrl = "api/StockInventoryLines";

  onChangeCreateLine(val) {
    return this.http.post(this.base_api + this.apiUrl + '/OnChangeCreateLine', val);
  }
}
