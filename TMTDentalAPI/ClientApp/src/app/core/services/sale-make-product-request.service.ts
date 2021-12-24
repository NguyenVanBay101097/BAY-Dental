import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SaleMakeProductRequestService {

  apiUrl = 'api/SaleMakeProductRequests';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  create(data: any) {
    return this.http.post(this.baseApi + this.apiUrl, data);
  }
}
