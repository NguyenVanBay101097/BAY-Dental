import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PagedResult2 } from '../core/paged-result-2';

export class PartnerTitle {
  id: string;
  name: string;
}
export class PartnerTitleSave {
  name: string;
}

export class PartnerTitlePaged {
  offset: number;
  limit: number;
  search: string;
}

@Injectable({
  providedIn: 'root'
})
export class PartnerTitleService {
  apiUrl = "api/PartnerTitles";
  constructor(
    private http: HttpClient,
    @Inject("BASE_API") private baseApi: string
  ) { }

  getPaged(val: any): Observable<PagedResult2<PartnerTitle>> {
    return this.http.get<PagedResult2<PartnerTitle>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
  }

  get(id: string) {
    return this.http.get<PartnerTitleSave>(this.baseApi + this.apiUrl + "/" + id);
  }

  create(val: PartnerTitleSave) {
    return this.http.post(this.baseApi + this.apiUrl, val);
  }

  update(id: string, val: PartnerTitleSave) {
    return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
  }

  autocomplete(val: PartnerTitlePaged): Observable<PartnerTitle[]> {
    return this.http.post<PartnerTitle[]>(this.baseApi + this.apiUrl + "/Autocomplete", val);
  }

  delete(id: string) {
    return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
  }
}
