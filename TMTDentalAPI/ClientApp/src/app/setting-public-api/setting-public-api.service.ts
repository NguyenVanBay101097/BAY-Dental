import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SettingPublicApiService {
  apiUrl = 'api/AccessToken';
  constructor(private http: HttpClient, @Inject('BASE_API') private base_api: string) { }

  getToken() {
    return this.http.get(this.base_api + this.apiUrl,  { responseType: 'text' });
  }

  generateToken(){
    return this.http.get(this.base_api + this.apiUrl + '/GenerateToken');
  }

}
