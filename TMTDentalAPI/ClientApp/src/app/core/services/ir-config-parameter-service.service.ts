import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class IrConfigParameterServiceService {

  apiUrl = 'api/IrConfigParameters';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  getIrConfigParameter(params) {
    return this.http.get(this.baseApi + this.apiUrl + '/GetParam', { params: params });
  }
}
