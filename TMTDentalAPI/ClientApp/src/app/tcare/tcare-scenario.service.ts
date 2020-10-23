import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';

export class TCareScenarioBasic
{
  id: string;
  name: string;  
}

export class TCareScenarioPaged {
  limit: number;
  offset: number;
  search: string;
}

export class TCareScenarioPaging {
  offset: number;
  limit: number;
  totalItems: number;
  items: TCareScenarioBasic[];
}

@Injectable({
  providedIn: 'root'
})
export class TCareScenarioService {
  constructor(
    @Inject('BASE_API') private base_api: string,
    private http: HttpClient
  ) {}

  private readonly apiUrl = "api/TCareScenarios"

  nameCreate(val) {
    return this.http.post(this.base_api + this.apiUrl + '/NameCreate', val);
  }

  update(id, value) {
    return this.http.put(this.base_api + this.apiUrl + '/' + id, value);
  }

  getPaged(val: any) {
    return this.http.get(this.base_api + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
  }

  get(id) {
    return this.http.get(this.base_api + this.apiUrl + '/' + id);
  }

  delete(id) {
    return this.http.delete(this.base_api + this.apiUrl + '/' + id);
  }

  autocomplete(val: TCareScenarioPaged): Observable<TCareScenarioBasic[]> {
    return this.http.post<TCareScenarioBasic[]>(this.base_api + this.apiUrl + "/Autocomplete", val);
  }

  actionStartCampaign(val) {
    return this.http.post(this.base_api + this.apiUrl + "/ActionStartCampaign", val)
  }

  actionStopCampaign(val) {
    return this.http.post(this.base_api + this.apiUrl + "/ActionStopCampaign", val)
  }

}
