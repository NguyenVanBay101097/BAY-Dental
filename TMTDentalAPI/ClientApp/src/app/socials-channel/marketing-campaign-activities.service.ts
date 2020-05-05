import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';

@Injectable({
  providedIn: 'root'
})

export class MarketingCampaignActivitiesService {
  apiUrl = 'api/MarketingCampaignActivities';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  get(val: any) {
    return this.http.get(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
  }

  post(data: any) {
    return this.http.post(this.baseApi + this.apiUrl, data);
  }

  getWithID(id) {
    return this.http.get(this.baseApi + this.apiUrl + "/" + id);
  }

  put(id, val) {
    return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
  }

  delete(id) {
    return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
  }
}
