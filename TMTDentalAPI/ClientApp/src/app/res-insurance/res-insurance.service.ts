import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedResult2 } from '../core/paged-result-2';
import { InsuranceActionDeactiveRequest, ResInsuranceBasic, ResInsuranceDisplay, ResInsurancePaged, ResInsuranceSave, ResInsuranceSimple } from './res-insurance.model';

@Injectable({
  providedIn: 'root'
})
export class ResInsuranceService {
  readonly apiUrl = 'api/ResInsurances';

  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  getPaged(paged): Observable<PagedResult2<ResInsuranceBasic>> {
    return this.http.get<PagedResult2<ResInsuranceBasic>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: paged }) });
  }

  getById(id: string): Observable<ResInsuranceDisplay> {
    return this.http.get<ResInsuranceDisplay>(this.baseApi + this.apiUrl + '/' + id);
  }

  autoComplete(val: ResInsurancePaged): Observable<ResInsuranceSimple> {
    return this.http.post<ResInsuranceSimple>(this.baseApi + this.apiUrl + '/AutoComplete', val);
  }

  create(val: ResInsuranceSave) {
    return this.http.post(this.baseApi + this.apiUrl, val);
  }

  update(id: string, val: ResInsuranceSave) {
    return this.http.put(this.baseApi + this.apiUrl + '/' + id, val);
  }

  actionActive(val: InsuranceActionDeactiveRequest) {
    return this.http.post(this.baseApi + this.apiUrl + '/ActionActive', val);
  }

  actionDeactive(val: InsuranceActionDeactiveRequest) {
    return this.http.post(this.baseApi + this.apiUrl + '/ActionDeactive', val);
  }

  remove(id: string) {
    return this.http.delete(this.baseApi + this.apiUrl + '/' + id);
  }
}
