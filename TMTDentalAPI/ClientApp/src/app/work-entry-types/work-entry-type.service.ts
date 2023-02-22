import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';


export class WorkEntryType {
  id: string;
  name: string;
  isHasTimeKeeping: boolean;
  color: string;
}

export class WorkEntryTypeBasic {
  id: string;
  name: string;
}

export class WorkEntryTypePage {
  limit: number;
  offset: number;
  filter: string;
}

export class WorkEntryTypePaging {
  offset: number;
  limit: number;
  totalItems: number;
  items: WorkEntryType[];
}

export class WorkEntryTypeSave {
  name: string;
  color: string;
  code: string;
  sequence: number;
}

@Injectable({
  providedIn: 'root'
})
export class WorkEntryTypeService {

  constructor(private http: HttpClient, @Inject("BASE_API") private base_api: string) { }

  apiUrl = "api/WorkEntryTypes";

  getPaged(val): Observable<WorkEntryTypePaging> {
    return this.http.get<WorkEntryTypePaging>(this.base_api + this.apiUrl, { params: val });
  }

  delete(id) {
    return this.http.delete(this.base_api + this.apiUrl + '/' + id);
  }

  get(id): Observable<WorkEntryType> {
    return this.http.get<WorkEntryType>(this.base_api + this.apiUrl + '/' + id);
  }

  create(val: WorkEntryTypeSave ) {
    return this.http.post(this.base_api + this.apiUrl, val);
  }

  update(id, val: WorkEntryTypeSave) {
    return this.http.put(this.base_api + this.apiUrl + '/' + id, val);
  }

}
