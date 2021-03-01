import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormArray } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import * as _ from 'lodash';
import { WebService } from 'src/app/core/services/web.service';
import { SurveyQuestionService } from '../survey-question.service';
import { SurveyTagBasic, SurveyTagPaged, SurveyTagService } from '../survey-tag.service';
import { SurveyUserInputDisplay, SurveyUserinputService } from '../survey-userinput.service';
import { SurveyAssignmentService } from '../survey.service';

@Component({
  selector: 'app-survey-manage-detail-survey-imformation',
  templateUrl: './survey-manage-detail-survey-imformation.component.html',
  styleUrls: ['./survey-manage-detail-survey-imformation.component.css']
})
export class SurveyManageDetailSurveyImformationComponent implements OnInit {
  formGroup: FormGroup;
  id: string;// có thể là input
  title: string;
  surveyAssignmentStatus: string;
  surveyAssignmentId: string;
  userinput: SurveyUserInputDisplay = new SurveyUserInputDisplay();
  question: any[] = [];
  view = true;
  surveyTags: SurveyTagBasic[];
  limit: number = 20;
  offset: number = 0;
  search: string;


  constructor(private fb: FormBuilder,
    private activateRoute: ActivatedRoute,
    private surveyService: SurveyAssignmentService,
    private notificationService: NotificationService,
    private surveyTagService: SurveyTagService,
    private userInputService: SurveyUserinputService,
  ) { }

  ngOnInit() {
    this.surveyAssignmentId = this.activateRoute.parent.snapshot.paramMap.get('id');
    this.formGroup = this.fb.group({
      score: null,
      maxScore: null,
      note: null,
      surveyTags: null,
      lines: this.fb.array([])
    });

    if (this.surveyAssignmentId) {
      this.loadFromApi();
    }
  }

  notify(style, content) {
    this.notificationService.show({
      content: content,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: style, icon: true }
    });
  }

  get lines() {
    return this.formGroup.get('lines') as FormArray;
  }

  loadFromApi() {
    this.surveyService.get(this.surveyAssignmentId).subscribe(
      result => {
        if (result && result.userInputId) {
          this.id = result.userInputId;
          this.surveyAssignmentStatus = result.status;
          this.loadData();
        }
      }
    )
  }

  loadData() {
    this.userInputService.get(this.id).subscribe(result => {
      this.userinput = result;
      this.patchValue(result);
    });
  }

  loadSurveyTagList() {
    this.searchSurveyTags().subscribe((result) => {
      this.surveyTags = _.unionBy(this.surveyTags, result.items, 'id');;
    });
  }

  searchSurveyTags(q?: string) {
    var val = new SurveyTagPaged();
    val.search = q || '';
    return this.surveyTagService.getPaged(val);
  }


  patchValue(res) {
    this.formGroup.patchValue(res);
    // patch attach
    if (res.lines) {
      var control = this.formGroup.get('lines') as FormArray;
      control.clear();
      var lines = this.userinput.lines;
      lines.forEach(line => {
        control.push(this.fb.group(line));
      });

    }

    if (res.surveyTags.length > 0) {
      this.formGroup.get('surveyTags').setValue(res.surveyTags);
      this.surveyTags = _.unionBy(res.surveyTags as SurveyTagBasic[], res.surveyTags, 'id');
    }


  }

  onChange(line: FormGroup, item) {
    var res = this.lines.controls.find(x => x.value.questionId === line.value.question.id);
    if (res) {
      line.get('answerId').setValue(item.id);
      res.patchValue(line.value);
    }
  }
}
