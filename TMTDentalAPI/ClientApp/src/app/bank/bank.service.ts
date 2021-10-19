import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

export class BankPaged {
  limit: number;
  offset: number;
  search: string;
}

export class BankBasic {
  id: string;
  name: string;
}
@Injectable({
  providedIn: 'root'
})
export class BankService {
  constructor(private http: HttpClient, @Inject("BASE_API") private base_api: string) { }
  apiUrl = "api/Banks";

  create(val) {
    return this.http.post(this.base_api + this.apiUrl, val);
  }

  getAutocomplete(val) {
    return this.http.post(this.base_api + this.apiUrl + '/Autocomplete', val);
  }
}
