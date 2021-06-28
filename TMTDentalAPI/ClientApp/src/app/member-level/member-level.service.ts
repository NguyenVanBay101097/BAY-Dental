import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { PagedResult2 } from '../employee-categories/emp-category';

export class MemberLevelAutoCompleteReq {
  limit: number;
  offset: number;
  search: string;
}

export class MemberLevelSimple{
  id: string;
  name: string;
}

@Injectable({
  providedIn: 'root'
})
export class MemberLevelService {
  apiUrl = 'api/MemberLevels';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  get() {
    return this.http.get<any[]>(this.baseApi + this.apiUrl);
  }

  create(val) {
    return this.http.post(this.baseApi + this.apiUrl, val);
  }

  update(val) {
    return this.http.post(this.baseApi + this.apiUrl + '/Update', val);
  }

  delete(ids) {
    return this.http.post(this.baseApi + this.apiUrl + '/Remove', ids);
  }

  updateMember(vals) {
    return this.http.post(this.baseApi + this.apiUrl + '/UpdateMember', vals);
  }

  autoComplete(val){
    return this.http.get<PagedResult2<MemberLevelSimple>>(this.baseApi + this.apiUrl + '/AutoComplete', {params: new HttpParams({fromObject: val})});
  }
}
