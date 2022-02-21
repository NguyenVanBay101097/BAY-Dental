import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

export class GetReportAssignmentQueryRequest{
  dateFrom: any;
  dateTo: any;
  companyId: string;
  status: string;
} 

export class ReportRatingScroreRateOfUserInputRequest extends GetReportAssignmentQueryRequest {
 
}

export class ReportNumberOfAssigmentByEmployeeRequest extends GetReportAssignmentQueryRequest {
 
}

export class ReportSatifyScoreRatingByQuestionRequest extends GetReportAssignmentQueryRequest {
 
}

@Injectable({
  providedIn: 'root'
})
export class SurveReportService {

  constructor(private http: HttpClient, @Inject("BASE_API") private base_api: string) { }
  apiUrl = "api/SurveyReport"

  reportRatingScroreRate(val: ReportRatingScroreRateOfUserInputRequest){
    return this.http.post(this.base_api + this.apiUrl + '/ReportRatingScroreRate', val);
  }

  reportNumberOfAssigmentByEmployee(val: ReportNumberOfAssigmentByEmployeeRequest){
    return this.http.post(this.base_api + this.apiUrl + '/ReportNumberOfAssigmentByEmployee', val);
  }

  reportSatifyScoreRatingByQuestion(val: ReportSatifyScoreRatingByQuestionRequest){
    return this.http.post(this.base_api + this.apiUrl + '/ReportSatifyScoreRatingByQuestion', val);
  }

}
