import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export class SurveyQuestionBasic {
  id: string;
  name: string;
  sequence: number;
  active;
}

export class SurveyQuestionDisplay {
  id: string;
  name: string;
  type: string;
  sequence: number;
  answers: SurveyAnswerDisplay[];
}

export class SurveyAnswerDisplay {
  id: string;
  name: string;
  score: number;
  sequence: number;
}

export class SurveyQuestionPagging {
  limit: number;
  offset: number;
  totalItems: number;
  items: SurveyQuestionBasic[];
}

export class SurveyQuestionPaged {
  limit: number;
  offset: number;
  search: string;
  active: any;
}

export interface ActionActivePar{
  id: string;
  active: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class SurveyQuestionService {

  constructor(private http: HttpClient, @Inject("BASE_API") private base_api: string) { }
  apiUrl = "api/SurveyQuestions"

  getPaged(val: any): Observable<SurveyQuestionPagging> {
    return this.http.get<SurveyQuestionPagging>(this.base_api + this.apiUrl, { params: val });
  }

  get(id: string): Observable<SurveyQuestionDisplay> {
    return this.http.get<SurveyQuestionDisplay>(this.base_api + this.apiUrl + '/' + id);
  }

  create(val: any) {
    return this.http.post(this.base_api + this.apiUrl, val);
  }

  update(id: string, val: any) {
    return this.http.put(this.base_api + this.apiUrl + '/' + id, val);
  }

  duplicate(id: string) {
    return this.http.post(this.base_api + this.apiUrl + '/' + id + '/Duplicate', null);
  }

  resequence(vals: any) {
    return this.http.post(this.base_api + this.apiUrl + '/Resequence', vals)
  }

  delete(id: string) {
    return this.http.delete(this.base_api + this.apiUrl + '/' + id);
  }

  getListForSurvey(userInputId?: string) {
    var p = new HttpParams();
    if(userInputId) p = p.append("userInputId",userInputId);
    return this.http.get(this.base_api + this.apiUrl + '/GetListForSurvey', {params: p});
  }

  actionActive(val: any) {
    return this.http.post(this.base_api + this.apiUrl + '/ActionActive', val);
  }
}
