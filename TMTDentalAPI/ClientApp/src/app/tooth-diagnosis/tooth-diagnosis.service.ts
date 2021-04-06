import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedResult2 } from '../core/paged-result-2';


export class ToothDiagnosis {
  id: string;
  name: string;
}

export class ToothDiagnosisSave {
  id: string;
  name: string;
  product: any[];
}

export class ToothDiagnosisPaged {
  offset: number;
  limit: number;
  search: string;
}
@Injectable({
  providedIn: 'root'
})
export class ToothDiagnosisService {
  apiUrl = "api/ToothDiagnosis";
  constructor(
    private http: HttpClient,
    @Inject("BASE_API") private baseApi: string
  ) { }

  getPaged(val: any): Observable<PagedResult2<ToothDiagnosis>> {
    return this.http.get<PagedResult2<ToothDiagnosis>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
  }

  get(id: string) {
    return this.http.get<ToothDiagnosisSave>(this.baseApi + this.apiUrl + "/" + id);
  }

  create(val: ToothDiagnosisSave) {
    return this.http.post(this.baseApi + this.apiUrl, val);
  }

  update(id: string, val: ToothDiagnosisSave) {
    return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
  }

  autocomplete(val: ToothDiagnosisPaged): Observable<ToothDiagnosis[]> {
    return this.http.post<ToothDiagnosis[]>(this.baseApi + this.apiUrl + "/Autocomplete", val);
  }

  delete(id: string) {
    return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
  }

}
