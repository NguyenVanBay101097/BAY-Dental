import { Component, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { MultiSelectComponent } from '@progress/kendo-angular-dropdowns';
import { LinearGradient } from '@progress/kendo-drawing';
import * as _ from 'lodash';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
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
  surveyAssignmentId: string;
  surveyTags: SurveyTagBasic[];
  disable = false;
  @ViewChild('tagMultiSelect', {static: true}) tagMultiSelect: MultiSelectComponent;

  constructor(public activeModal: NgbActiveModal, private surveyUserinputService: SurveyUserinputService, private modalService: NgbModal,
    private questionService: SurveyQuestionService, private surveyTagService: SurveyTagService, private fb: FormBuilder) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      questions: this.fb.array([]),
      surveyTags: null,
      note: null,
    });
 
    setTimeout(() => {
      this.loadSurveyTagList();
      this.loadQuestions();

      this.tagMultiSelect.filterChange.asObservable().pipe(
        debounceTime(300),
        tap(() => (this.tagMultiSelect.loading = true)),
        switchMap(value => this.searchSurveyTags(value))
      ).subscribe(result => {
        this.surveyTags = result.items;
        this.tagMultiSelect.loading = false;
      });
    });
  }

  loadQuestions() {
    this.questionService.getListForSurvey(this.id).subscribe((result: any) => {
      this.questions = result;
      this.createFormGroup(result);

      if (this.id) {
        this.surveyUserinputService.getAnswer(this.id).subscribe((result: any) => {
          this.formGroup.patchValue(result);
        });
      }
    });
  }

  get getQuestions() {
    return this.formGroup.get('questions') as FormArray;
  }

  get getSurveyTagsValue() {
    return this.formGroup.get('surveyTags').value || [];
  }

  createFormGroup(questions: SurveyQuestionDisplay[]) {
    var array = this.formGroup.get('questions') as FormArray;
    questions.forEach(question => {
      if (question.type == 'radio') {
        array.push(this.fb.group({
          questionId: question.id,
          answerValue: [null, Validators.required]
        }));
      } else {
        array.push(this.fb.group({
          questionId: question.id,
          answerValue: ['']
        }));
      }
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

  quickCreateSurveyTagModal() {
    const modalRef = this.modalService.open(SurveyTagDialogComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm: Nhãn khảo sát';

    modalRef.result.then(result => {
      if (result && result.id) {
        this.surveyTags.push(result);
      }
    })
  }

  onSave() {
    if (this.formGroup.invalid) {
      return false;
    }

    var val = this.formGroup.value;
    val.assignmentId = this.surveyAssignmentId;
    val.surveyTagIds = val.surveyTags ? val.surveyTags.map(x => x.id) : [];
    this.surveyUserinputService.create(val).subscribe(() => {
      this.activeModal.close();
    });
  }

  onCancel() {
    this.activeModal.dismiss();
  }

}
