import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SurveyAssignmentService } from '../survey.service';

@Component({
  selector: 'app-survey-manage-detail',
  templateUrl: './survey-manage-detail.component.html',
  styleUrls: ['./survey-manage-detail.component.css']
})
export class SurveyManageDetailComponent implements OnInit {
  id: string;
  surveyAssignment: any;
  constructor(
    private surveyService: SurveyAssignmentService,
    private activateRoute: ActivatedRoute
  ) { }

  ngOnInit() {
    this.id = this.activateRoute.snapshot.paramMap.get('id');
    this.surveyService.get(this.id).subscribe(
      result => {
        if (result) {
          this.surveyAssignment = result
        }
      }
    )
  }

}
