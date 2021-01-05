import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';



@Injectable({
  providedIn: 'root'
})
export class MedicineOrderService {

  constructor(private http: HttpClient, @Inject("BASE_API") private base_api: string) { }
  apiUrl = "api/MedicineOrders";

}
