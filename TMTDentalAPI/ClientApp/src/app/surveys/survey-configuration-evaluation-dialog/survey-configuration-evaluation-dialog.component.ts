import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { SurveyQuestionDisplay, SurveyQuestionService } from '../survey-question.service';

@Component({
  selector: 'app-survey-configuration-evaluation-dialog',
  templateUrl: './survey-configuration-evaluation-dialog.component.html',
  styleUrls: ['./survey-configuration-evaluation-dialog.component.css']
})
export class SurveyConfigurationEvaluationDialogComponent implements OnInit {
  question: SurveyQuestionDisplay;
  formGroup: FormGroup;
  activeNote: string;
  id: string;
  title: string;
  submitted = false;
  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private surveyQuestionService: SurveyQuestionService,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      type: ['radio', Validators.required],
      sequence: 0,
      answers: this.fb.array([])
    });

    if (this.id) {
      this.loadDataFromApi();
      this.radioDisable();
    } else {
      this.loadDefaultAnswer();
    }
  }

  get answers() {
    return this.formGroup.get('answers') as FormArray;
  }

  loadDataFromApi() {
    this.surveyQuestionService.get(this.id).subscribe(
      result => {
        this.question = result;
        this.formGroup.patchValue(this.question);
        if (this.question && this.question.answers) {
          var answers = this.formGroup.get('answers') as FormArray;
          this.question.answers.sort((a, b) => a.sequence - b.sequence).forEach(item => {
            let formGroupChild = this.fb.group(item);
            formGroupChild.controls.name.setValidators(Validators.required);
            formGroupChild.controls.name.updateValueAndValidity();
            answers.push(formGroupChild);
          })
        }

      }
    )
  }

  loadDefaultAnswer() {
    var answers = this.formGroup.get('answers') as FormArray;
    var score = 5;
    for (let index = 1; index <= 5; index++) {
      answers.push(this.fb.group({
        name: ['', Validators.required],
        score: score,
        sequence: index
      }));
      score--;
    }
  }

  getValueFromFormGroup() {
    if (this.formGroup.get('name').invalid) { return false; }
    if (this.formGroup.get('type').value == "radio" && this.formGroup.get('answers').invalid) { return false }
    var value = this.formGroup.value;
    return value;
  }

  onSave() {
    this.submitted = true;
    if (!this.formGroup.valid) {
      return false;
    }

    var val = this.getValueFromFormGroup();
    if (this.id) {
      this.surveyQuestionService.update(this.id, val).subscribe(
        res => {
          this.notificationService.show({
            content: 'L??u th??nh c??ng',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
          this.activeModal.close();
        }
      )
    } else {
      this.surveyQuestionService.create(val).subscribe(
        res => {
          this.notificationService.show({
            content: 'L??u th??nh c??ng',
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

  onDrop(event: CdkDragDrop<any[]>) {
    var answers = this.answers.value;
    moveItemInArray(this.answers.controls, event.previousIndex, event.currentIndex);
    moveItemInArray(this.answers.value, event.previousIndex, event.currentIndex);
    this.answers.clear();
    answers.forEach((item, idx) => {
      item.sequence = idx + 1;
      item.score = idx + 1;
      this.answers.push(this.fb.group(item));
    });
  }

  get f() {
    return this.formGroup.controls;
  }

  radioDisable() {
    document.getElementById('radioType').setAttribute('disabled', '')
    document.getElementById('radioType1').setAttribute('disabled', '')
  }

  onChangeQuestionType() {
    var questionType = this.formGroup.get('type').value;
    if (questionType != 'radio') {
      this.answers.clear();
    } else {
      this.loadDefaultAnswer();
    }
  }
}
