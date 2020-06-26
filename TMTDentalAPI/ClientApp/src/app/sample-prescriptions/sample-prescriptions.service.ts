import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export class SamplePrescriptionsPaged {
  offset: number;
  limit: number;
  search: string;
}

export class SamplePrescriptionsDisplay {
  id: string;
  name: string;
  lines: [];
}

export class SamplePrescriptionsPaging {
  offset: number;
  limit: number;
  totalItems: number;
  items: [];
}

@Injectable()
export class SamplePrescriptionsService {
  apiUrl = 'api/sampleprescriptions';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  getPaged(val: any): Observable<SamplePrescriptionsPaging> {
    return this.http.get<SamplePrescriptionsPaging>(this.baseApi + this.apiUrl, { params: val });
  }

  get(id): Observable<SamplePrescriptionsDisplay> {
    return this.http.get<SamplePrescriptionsDisplay>(this.baseApi + this.apiUrl + "/" + id);
  }

  create(val: SamplePrescriptionsDisplay): Observable<SamplePrescriptionsDisplay> {
    return this.http.post<SamplePrescriptionsDisplay>(this.baseApi + this.apiUrl, val);
  }

  update(id: string, val: SamplePrescriptionsDisplay) {
    return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
  }

  delete(id: string) {
      return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
  }
}
