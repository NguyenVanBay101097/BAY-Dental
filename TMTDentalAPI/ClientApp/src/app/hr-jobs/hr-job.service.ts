import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedResult2 } from '../core/paged-result-2';

export class HrJobsBasic {
  id: string;
  name: string;
}

export class HrJobsPaged {
  limit: number;
  offset: number;
  search: string;
}

// export class HrJobsSave {
//   name: string;
//   companyId: string;
// }

@Injectable({
  providedIn: 'root'
})
export class HrJobService {
  apiUrl = 'api/HrJobs';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  getPaged(val: any): Observable<PagedResult2<HrJobsBasic>> {
    return this.http.get<PagedResult2<HrJobsBasic>>(this.baseApi + this.apiUrl, {params: new HttpParams({fromObject: val})});
  }

  create(val: any) {
    return this.http.post(this.baseApi + this.apiUrl, val);
  }

  get(id: string) {
    return this.http.get(this.baseApi + this.apiUrl + "/" + id);
  }

  update(id: string, val: any) {
    return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
  }

  delete(id: string) {
    return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
  }
}
