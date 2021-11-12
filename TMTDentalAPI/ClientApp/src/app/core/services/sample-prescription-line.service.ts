import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SamplePrescriptionLineService {

  apiUrl = 'api/SamplePrescriptionLines';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  onChangeProduct(data: any) {
    return this.http.post(this.baseApi + this.apiUrl + '/OnChangeProduct', data);
  }
}
