import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export class SurveyCallContentBasic {
  id: string;
  name: string;
  date: Date;
}

export class SurveyCallContentDisplay {
  id: string;
  name: string;
  date: Date;
}

export class SurveyCallContentPagging {
  limit: number;
  offset: number;
  totalItems: number;
  items: SurveyCallContentBasic[];
}

export class SurveyCallContentPaged {
  limit: number;
  offset: number;
  search: string;
  assignmentId: string;
}

@Injectable({
  providedIn: 'root'
})
export class SurveyCallcontentService {

  constructor(private http: HttpClient, @Inject("BASE_API") private base_api: string) { }
  apiUrl = "api/SurveyCallContents";

  getPaged(val: any): Observable<SurveyCallContentPagging> {
    return this.http.get<SurveyCallContentPagging>(this.base_api + this.apiUrl, { params: val });
  }

  get(id: string): Observable<SurveyCallContentDisplay> {
    return this.http.get<SurveyCallContentDisplay>(this.base_api + this.apiUrl + '/' + id);
  }

  create(val: any) {
    return this.http.post(this.base_api + this.apiUrl, val);
  }

  update(id: string, val: any) {
    return this.http.put(this.base_api + this.apiUrl + '/' + id, val);
  }

  remove(id: string) {
    return this.http.delete(this.base_api + this.apiUrl + '/' + id);
  }
}
