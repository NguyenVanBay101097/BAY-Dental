import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export class ChamCongSave {

}

@Injectable({
  providedIn: 'root'
})
export class TimeKeepingService {

  constructor(private http: HttpClient, @Inject("BASE_API") private base_api: string) { }
  apiUrl = "api/TimeKeeping";

  create(val): Observable<ChamCongSave> {
    return this.http.post<ChamCongSave>(this.base_api + this.apiUrl, val);
  }

  update(id, val) {
    return this.http.put(this.base_api + this.apiUrl + '/' + id, val);
  }

  // get()
}
