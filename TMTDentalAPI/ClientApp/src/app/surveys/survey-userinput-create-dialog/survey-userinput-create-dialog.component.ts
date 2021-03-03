import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { LinearGradient } from '@progress/kendo-drawing';
import * as _ from 'lodash';
import { SurveyQuestionDisplay, SurveyQuestionService } from '../survey-question.service';
import { SurveyTagDialogComponent } from '../survey-tag-dialog/survey-tag-dialog.component';
import { SurveyTagBasic, SurveyTagPaged, SurveyTagService } from '../survey-tag.service';
import { SurveyUserinputService } from '../survey-userinput.service';

@Component({
  selector: 'app-survey-userinput-create-dialog',
  templateUrl: './survey-userinput-create-dialog.component.html',
  styleUrls: ['./survey-userinput-create-dialog.component.css']
})
export class SurveyUserinputCreateDialogComponent implements OnInit {
  formGroup: FormGroup;
  title: string;
  id: string;
  questions: SurveyQuestionDisplay[] = [];
  surveyAssignmentStatus: string;
  surveyAssignmentId: string;
  surveyTags: SurveyTagBasic[];
  selectedTagIds: any[];

  constructor(public activeModal: NgbActiveModal, private surveyUserinputService: SurveyUserinputService, private modalService: NgbModal,
    private questionService: SurveyQuestionService, private surveyTagService: SurveyTagService, private fb: FormBuilder) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      questions: this.fb.array([]),
      surveyTags: null,
      note: null,
      assignmentId: null,
    });

    this.loadSurveyTagList();
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

  get getQuestions() {
    return this.formGroup.get('questions') as FormArray;
  }

  createFormGroup(questions: SurveyQuestionDisplay[]) {
    var array = this.formGroup.get('questions') as FormArray;
    questions.forEach(question => {
      array.push(this.fb.group({
        questionId: question.id,
        answerValue: [null, Validators.required]
      }));
    });
    if (this.id) {
      this.surveyUserinputService.get(this.id).subscribe((result: any) => {
        this.formGroup.patchValue(result);
        var lines = result.lines;
        lines.forEach(item => {
          var line = this.getQuestions.controls.find(x => x.value.questionId === item.questionId);
          if (line) {
            line.value.answerValue = item.question.type == 'radio' ? item.answerId : item.valueText;
            line.patchValue(line.value);
          }
        });

        if (result.surveyTags.length > 0) {
          this.surveyTags = _.unionBy(result.surveyTags as SurveyTagBasic[], result.surveyTags, 'id');
        }
      });
    }
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

  public onSizeChange(value) {
    this.selectedTagIds = value;

  }

  quickCreateSurveyTagModal() {
    const modalRef = this.modalService.open(SurveyTagDialogComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm: Nhãn khảo sát';

    modalRef.result.then(result => {
      if (result && result.id) {
        this.surveyTags.push(result);
        this.formGroup.get('surveyTags').setValue([result as SurveyTagBasic]);
      }
    })
  }

  onSave() {
    if (this.formGroup.invalid) {
      return false;
    }

    var val = this.formGroup.value;
    val.assignmentId = this.surveyAssignmentId;
    val.surveyTagIds = this.selectedTagIds.map(x => x.id);
    this.surveyUserinputService.create(val).subscribe(() => {
      this.activeModal.close();
    });
  }

  onCancel() {
    this.activeModal.dismiss();
  }

}
