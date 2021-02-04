import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { SurveyCallcontentService } from '../survey-callcontent.service';
import { SurveyAssignmentDisplay, SurveyService } from '../survey.service';

@Component({
  selector: 'app-survey-manage-detail',
  templateUrl: './survey-manage-detail.component.html',
  styleUrls: ['./survey-manage-detail.component.css']
})
export class SurveyManageDetailComponent implements OnInit {
  id: string;
  surveyAssignment: any;
  constructor(
    private surveyService: SurveyService,
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
