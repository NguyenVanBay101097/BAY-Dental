import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PagedResult2 } from '../core/paged-result-2';
import { ResPartnerBankBasic } from './res-partner-bank';

@Injectable({
  providedIn: 'root'
})
export class ResPartnerBankService {

  readonly apiUrl = 'api/ResPartnerBanks';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  getPaged(paged): Observable<PagedResult2<ResPartnerBankBasic>> {
    return this.http.get<PagedResult2<ResPartnerBankBasic>>(this.baseApi + this.apiUrl, { params: paged });
  }

  createUpdate(val, id): Observable<ResPartnerBankBasic> {
    if (id) {
      return this.http.put<ResPartnerBankBasic>(this.baseApi + this.apiUrl + `/${id}`, val);
    }
    else {
      return this.http.post<ResPartnerBankBasic>(this.baseApi + this.apiUrl, val);
    }
  }

  deletePartnerBank(id) {
    return this.http.delete(this.baseApi + this.apiUrl + '/' + id);
  }

  getById(id) {
    return this.http.get(this.baseApi + this.apiUrl + '/' + id);
  }
}
