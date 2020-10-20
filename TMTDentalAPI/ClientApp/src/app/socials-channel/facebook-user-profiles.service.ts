import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PagedResult2 } from '../core/paged-result-2';

@Injectable({
  providedIn: 'root'
})
export class FacebookUserProfilesService {
  apiUrl = 'api/FacebookUserProfiles';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  getPaged(val: any): Observable<PagedResult2<any>> {
    return this.http.get<PagedResult2<any>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
  }

  update(id, val: any) {
    return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
  }

  get(id) {
    return this.http.get(this.baseApi + this.apiUrl + "/" + id);
  }

  connectPartner(val: any) {
    return this.http.post(this.baseApi + this.apiUrl + '/ConnectPartner', val);
  }

  

  removePartner(val: any) {
    return this.http.post(this.baseApi + this.apiUrl + '/RemovePartner', val);
  }
}
