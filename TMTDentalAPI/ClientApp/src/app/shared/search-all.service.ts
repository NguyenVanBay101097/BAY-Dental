import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export class SearchAllViewModel {
  id: string;
  name: string;
  address: string;
  phone: string;
  type: string;
  saleOrderName: string;
  state: string;
  tags: any[];
}

@Injectable({
  providedIn: 'root'
})
export class SearchAllService {

  constructor(private http: HttpClient, @Inject("BASE_API") private base_api: string) { }

  apiUrl = "api/SearchAlls"

  getAll(val: any): Observable<SearchAllViewModel[]> {
    return this.http.post<SearchAllViewModel[]>(this.base_api + this.apiUrl + '/GetAll', val);
  }
}
