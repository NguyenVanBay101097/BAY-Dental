import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class LaboOrderLineService {
  apiUrl = 'api/LaboOrderLines';

  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  updateStatistic(id: string, val: any) {
    return this.http.patch(this.baseApi + this.apiUrl + "/" + id, val);
  }
  
}
