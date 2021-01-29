import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export class SurveyQuestionBasic {
  id: string;
  name: string;
  sequence: number;
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

  doublicateQuestion(id: string) {
    return this.http.post(this.base_api + this.apiUrl + '/' + id + '/Duplicate', null);
  }

  updateListSequence(vals: any) {
    return this.http.post(this.base_api + this.apiUrl + '/UpdateListSequence', vals)
  }

  delete(id: string) {
    return this.http.delete(this.base_api + this.apiUrl + '/' + id);
  }
}
