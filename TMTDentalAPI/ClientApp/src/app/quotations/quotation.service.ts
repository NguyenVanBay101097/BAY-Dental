import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedResult2 } from '../core/paged-result-2';
import { PartnerSimple } from '../partners/partner-simple';
import { UserSimple } from '../users/user-simple';

// import { PagedResult2 } from '../paged-result-2';

export class QuotationsBasic {
  id: string;
  name: string;
  partner: PartnerSimple;
  user: UserSimple;
  dateQuotation: string;
  dateApplies: number;
  dateEndQuotation: string;
  note: string;
  totalAmount: number;
  state: string;
}

export class QuotationsDisplay {
  id: string;
  name: string;
  partner: PartnerSimple;
  user: UserSimple;
  dateQuotation: string;
  dateApplies: number;
  dateEndQuotation: string;
  note: string;
  totalAmount: number;
  state: string;
}

export class QuotationsPaged {
  dateFrom: string;
  dateTo: string;
  search: string;
  limit: number;
  partnerId: string;
  offset: number;
}
@Injectable({
  providedIn: 'root'
})
export class QuotationService {
  apiUrl = 'api/Quotations';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  getPaged(val: any): Observable<PagedResult2<QuotationsBasic>> {
    return this.http.get<PagedResult2<QuotationsBasic>>(this.baseApi + this.apiUrl, { params: val });
  }

  create(val: any) {
    return this.http.post(this.baseApi + this.apiUrl, val);
  }

  get(id: string): Observable<QuotationsDisplay> {
    return this.http.get<QuotationsDisplay>(this.baseApi + this.apiUrl + '/' + id);
  }

  defaultGet(partnerId: string): Observable<QuotationsDisplay> {
    return this.http.get<QuotationsDisplay>(this.baseApi + this.apiUrl + '/GetDefault/' + partnerId);
  }

  delete(id: string) {
    return this.http.delete(this.baseApi + this.apiUrl + '/' + id);
  }
}
