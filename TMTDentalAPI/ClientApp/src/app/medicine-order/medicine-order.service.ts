import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

export class Paged {
  limit: number;
  offset: number;
  search: string;
  dateFrom: string;
  dateTo: string;
  state: string;
}

export class PrecscriptionPaymentVM {
  id:string;
  name:string;
  state:string;
  
}

export class PrescriptionPaymentPagging {
  limit: number;
  offset: number;
  totalItems: number;
  items: [];
}

@Injectable({
  providedIn: 'root'
})
export class MedicineOrderService {

  constructor(private http: HttpClient, @Inject("BASE_API") private base_api: string) { }
  apiUrl = "api/MedicineOrders";

  getPaged(val) {
    return this.http.get(this.base_api + this.apiUrl, { params: val });
  }

  getPrint(id: string) {
    return this.http.get(this.base_api + this.apiUrl + "/" + id + '/GetPrint');
}
}
