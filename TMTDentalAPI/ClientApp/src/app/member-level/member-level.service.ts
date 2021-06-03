import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

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
}
