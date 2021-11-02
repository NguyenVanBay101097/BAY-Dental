import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import * as _ from 'lodash';
import { SurveyTagDialogComponent } from '../survey-tag-dialog/survey-tag-dialog.component';
import { SurveyTagBasic, SurveyTagPaged, SurveyTagService } from '../survey-tag.service';
import { SurveyUserInputDisplay, SurveyUserinputService } from '../survey-userinput.service';

@Component({
  selector: 'app-survey-userinput-dialog',
  templateUrl: './survey-userinput-dialog.component.html',
  styleUrls: ['./survey-userinput-dialog.component.css']
})
export class SurveyUserinputDialogComponent implements OnInit {
  formGroup: FormGroup;
  id: string;// có thể là input
  title: string;
  surveyAssignmentStatus: string;
  surveyAssignmentId: string;
  userinput: SurveyUserInputDisplay = new SurveyUserInputDisplay();
  question: any[] = [];
  Tags: SurveyTagBasic[];
  limit: number = 20;
  offset: number = 0;
  search: string;


  constructor(private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private modalService: NgbModal,
    private surveyTagService: SurveyTagService,
    private notificationService: NotificationService,
    private userInputService: SurveyUserinputService,
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      score: null,
      maxScore: null,
      note: null,
      surveyTags: null,
      lines: this.fb.array([])
    });
    
    this.loadSurveyTagList();
    if (this.id) {
      this.loadData();
    } else {
      this.loadDefault();
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


  loadData() {
    this.userInputService.get(this.id).subscribe(result => {
      this.userinput = result;
      this.patchValue(result);
    });
  }

  loadDefault() {
    this.userInputService.getDefault(this.surveyAssignmentId).subscribe(result => {
      this.userinput = result;
      this.patchValue(result);
    });
  }

  loadSurveyTagList() {
    this.searchSurveyTags().subscribe((result) => {
      this.Tags = _.unionBy(this.Tags, result.items, 'id');;
    });
  }

  searchSurveyTags(q?: string) {
    var val = new SurveyTagPaged();
    val.search = q || '';
    return this.surveyTagService.getPaged(val);
  }


  patchValue(res ) {
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
    
    if(res.surveyTags.length > 0){      
        this.formGroup.get('surveyTags').setValue(res.surveyTags);
        this.Tags = _.unionBy(res.surveyTags as SurveyTagBasic[], res.surveyTags, 'id');
    }

  }

  quickCreateSurveyTagModal() {
    const modalRef = this.modalService.open(SurveyTagDialogComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm: Nhãn khảo sát';

    modalRef.result.then(result => {
      if (result && result.id) {      
        this.Tags.push(result);
        this.formGroup.get('surveyTags').setValue([result as SurveyTagBasic]);
      }
    })
  }

  onChange(line: FormGroup, item) {
    var res = this.lines.controls.find(x => x.value.questionId === line.value.question.id);
    if (res) {
      line.get('answerId').setValue(item.id);
      res.patchValue(line.value);
    }
  }

  onSave() {
    var val = this.formGroup.value;
    this.activeModal.close(val);
  }


  onCancel() {
    this.activeModal.dismiss();
  }

}
