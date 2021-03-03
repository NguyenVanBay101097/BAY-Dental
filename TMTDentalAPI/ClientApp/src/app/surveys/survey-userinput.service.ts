import { SurveyTagBasic } from './survey-tag.service';
import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { SurveyAnswerDisplay, SurveyQuestionDisplay } from './survey-question.service';

export class SurveyUserInputPaged {
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
  id: string;
  score: number;
  maxScore: number;
}

export class SurveyUserInputCreate {
  lines: SurveyUserInputLineCreate[];
  surveyTagIds: string[];
  note: string;
  assignmentId: string;
}

export class SurveyUserInputDisplay {
  id: string;
  score: number;
  maxScore: number;
  lines: SurveyUserInputLineDisplay[];
  surveyTags: SurveyTagBasic[];
}

export class SurveyUserInputLineDisplay {
  id: string;
  score: number;
  valueText: string;
  answerId: string;
  answer: SurveyAnswerDisplay;
  questionId: string;
  Question: SurveyQuestionDisplay;
}

export class SurveyUserInputLineCreate {
  questionId: string;
  answerValue: string;
}

export class SurveyUserInputLineSave {
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

  get(id): Observable<SurveyUserInputDisplay> {
    return this.http.get<SurveyUserInputDisplay>(this.base_api + this.apiUrl + '/' + id);
  }

  create(val: any) {
    return this.http.post(this.base_api + this.apiUrl, val);
  }

  getDefault(assignmentId: string) {
    return this.http.post<SurveyUserInputDisplay>(this.base_api + this.apiUrl + '/DefaultGet', { surveyAssignmentId: assignmentId })
  }

  update(id: string, val: any) {
    return this.http.put(this.base_api + this.apiUrl + '/' + id, val);
  }

}
