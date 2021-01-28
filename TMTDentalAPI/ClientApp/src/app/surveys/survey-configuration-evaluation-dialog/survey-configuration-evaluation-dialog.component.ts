import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { SurveyAnswerDisplay, SurveyQuestionDisplay, SurveyQuestionService } from '../survey-question.service';

@Component({
  selector: 'app-survey-configuration-evaluation-dialog',
  templateUrl: './survey-configuration-evaluation-dialog.component.html',
  styleUrls: ['./survey-configuration-evaluation-dialog.component.css']
})
export class SurveyConfigurationEvaluationDialogComponent implements OnInit {
  question: SurveyQuestionDisplay
  formGroup: FormGroup;
  id: string;
  constructor(
    private fb: FormBuilder,
    private activeModal: NgbActiveModal,
    private surveyQuestionService: SurveyQuestionService,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      type: ['radio', Validators.required],
      answers: this.fb.array([
      ])
    });
    if (this.id) {
      this.loadDataFromApi();
    } else {
      this.loadDefaultAnswer();
    }
  }

  get answers() {
    return this.formGroup.get('answers') as FormArray;
  }

  loadDataFromApi() {

  }

  loadDefaultAnswer() {
    var answers = this.formGroup.get('answers') as FormArray;
    for (let index = 1; index <= 5; index++) {
      var ans = new SurveyAnswerDisplay();
      ans.name = '';
      ans.score = index;
      ans.sequence = index;
      answers.push(this.fb.group(ans));
    }
  }

  getValueFromFormGroup() {
    if (this.formGroup.invalid) { return false; }
    var value = this.formGroup.value;
    console.log(value);
    return value;
  }

  onSave() {
    var val = this.getValueFromFormGroup();
    this.surveyQuestionService.create(val).subscribe(
      res => {
        this.notificationService.show({
          content: 'Lưu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.activeModal.close();
      }
    )
  }


}
