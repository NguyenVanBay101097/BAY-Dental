import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { SurveyQuestionDisplay, SurveyQuestionService } from '../survey-question.service';

@Component({
  selector: 'app-survey-userinput-create-dialog',
  templateUrl: './survey-userinput-create-dialog.component.html',
  styleUrls: ['./survey-userinput-create-dialog.component.css']
})
export class SurveyUserinputCreateDialogComponent implements OnInit {
  formGroup: FormGroup;
  questions: SurveyQuestionDisplay[] = [];
  constructor(public activeModal: NgbActiveModal,
    private questionService: SurveyQuestionService, private fb: FormBuilder) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      questions: this.fb.array([])
    });

    setTimeout(() => {
      this.loadQuestions();
    });
  }

  loadQuestions() {
    this.questionService.getListForSurvey().subscribe((result: any) => {
      this.questions = result;
      this.createFormGroup(result);
    });
  }

  createFormGroup(questions: SurveyQuestionDisplay[]) {
    var array = this.formGroup.get('questions') as FormArray;
    questions.forEach(question => {
      array.push(this.fb.group({
        questionId: question.id,
        answerValue: [null, Validators.required]
      }));
    });
  }

  onSave() {
    console.log(this.formGroup.value);
  }

  onCancel() {
    this.activeModal.dismiss();
  }
  
}
