import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { TenantBasic } from './tenant.service';

export class TenantExtendHistorySave {
  startDate: string;
  expirationDate: string;
  activeCompaniesNbr: string;
  tenantId: string;
}

export class TenantExtendHistoryDisplay {
  id: string;
  dateCreated: string;
  startDate: string;
  expirationDate: string;
  activeCompaniesNbr: string;
  tenantId: string;
  appTenant: TenantBasic;
}

@Injectable({
  providedIn: 'root'
})
export class TenantExtendHistoryService {

  apiUrl = 'api/TenantExtendHistories';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  create(val: any): Observable<TenantExtendHistoryDisplay> {
    return this.http.post<TenantExtendHistoryDisplay>(this.baseApi + this.apiUrl, val);
  }

  delete(id) {
    return this.http.delete(this.baseApi + this.apiUrl + '/' + id);
  }

  getAllByTenantId(id): Observable<TenantExtendHistoryDisplay[]> {
    return this.http.get<TenantExtendHistoryDisplay[]>(this.baseApi + this.apiUrl + '/' + id + '/GetAllByTenantId');
  }

}
