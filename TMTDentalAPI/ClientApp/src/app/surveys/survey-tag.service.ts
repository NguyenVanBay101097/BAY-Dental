import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export class SurveyTagBasic {
  id: string;
  name: string;
  color: string;
}

export class SurveyTagSave {
  name: string;
  color: string;
}

export class SurveyTagPagging {
  limit: number;
  offset: number;
  totalItems: number;
  items: SurveyTagBasic[];
}

export class SurveyTagPaged {
  limit: number;
  offset: number;
  search: string;
}

@Injectable({
  providedIn: 'root'
})
export class SurveyTagService {

  constructor(private http: HttpClient, @Inject("BASE_API") private base_api: string) { }
  apiUrl = "api/SurveyTags"

  getPaged(val: any): Observable<SurveyTagPagging> {
    return this.http.get<SurveyTagPagging>(this.base_api + this.apiUrl, { params: val });
  }

  get(id: string) {
    return this.http.get(this.base_api + this.apiUrl + '/' + id);
  }

  create(val: any) {
    return this.http.post(this.base_api + this.apiUrl, val);
  }

  update(id: string, val: any) {
    return this.http.put(this.base_api + this.apiUrl + '/' + id, val);
  }

  delete(id: string) {
    return this.http.delete(this.base_api + this.apiUrl + '/' + id);
  }
}
