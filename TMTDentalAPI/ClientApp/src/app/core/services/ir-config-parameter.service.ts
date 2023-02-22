import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class IrConfigParameterService {

  apiUrl = 'api/IrConfigParameters';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  getParam(key: string) {
    var params = new HttpParams().set('key', key);
    return this.http.get(this.baseApi + this.apiUrl + '/GetParam', { params: params });
  }
}
