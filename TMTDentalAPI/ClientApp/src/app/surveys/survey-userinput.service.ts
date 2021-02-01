import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { SurveyAnswerDisplay, SurveyQuestionDisplay } from './survey-question.service';

export class SurveyUserInputPaged{
  limit: number;
  offset: number;
  search: string;
}

export class SurveyUserInputPagging {
  limit: number;
  offset: number;
  totalItems: number;
  items: SurveyUserInputBasic[];
}

export class SurveyUserInputBasic {
  id : string; 
  score: number; 
  maxScore: number;
}

export class SurveyUserInputDisplay
{
    id: string;
    score: number;
    maxScore: number;
    lines: SurveyUserInputLineDisplay[];
}

export class SurveyUserInputLineDisplay 
{
  id: string;
  score: number;
  valueText: string;
  answerId: string;
  answer: SurveyAnswerDisplay;
  questionId: string;
  Question: SurveyQuestionDisplay;
}

export class SurveyUserInputLineSave 
{
  id: string;
  score: number;
  valueText: string;
  answerId: string;
  questionId: string;
}



@Injectable({
  providedIn: 'root'
})
export class SurveyUserinputService {

  constructor(private http: HttpClient, @Inject("BASE_API") private base_api: string) { }
  apiUrl = "api/SurveyUserInputs";

  get(id: string): Observable<SurveyUserInputDisplay> {
    return this.http.get<SurveyUserInputDisplay>(this.base_api + this.apiUrl + '/' + id);
  }

  create(val: any) {
    return this.http.post(this.base_api + this.apiUrl, val);
  }

  update(id: string, val: any) {
    return this.http.put(this.base_api + this.apiUrl + '/' + id, val);
  }
  
}
