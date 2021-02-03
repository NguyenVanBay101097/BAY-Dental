import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { WebService } from 'src/app/core/services/web.service';
import { SurveyQuestionPaged, SurveyQuestionService } from '../survey-question.service';
import { SurveyUserInputDisplay, SurveyUserInputPaged, SurveyUserinputService } from '../survey-userinput.service';

@Component({
  selector: 'app-survey-userinput-dialog',
  templateUrl: './survey-userinput-dialog.component.html',
  styleUrls: ['./survey-userinput-dialog.component.css']
})
export class SurveyUserinputDialogComponent implements OnInit {
  formGroup: FormGroup;
  id: string;// có thể là input
  title: string;
  surveyAssignmentStatus:string;
  surveyAssignmentId : string;
  userinput: SurveyUserInputDisplay = new SurveyUserInputDisplay();
  question: any[] = [];
  limit: number = 20;
  offset: number = 0;
  search: string;


  constructor(private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private modalService: NgbModal,
    private notificationService: NotificationService,
    private userInputService: SurveyUserinputService,
    private intlService: IntlService,
    private questionService: SurveyQuestionService,
    private webService: WebService
  ) { }

  ngOnInit() {

    this.formGroup = this.fb.group({
      score: null,
      maxScore: null,
      lines: this.fb.array([])
    });
    
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


  }

  onChange(line: FormGroup, item){
    var res = this.lines.controls.find(x => x.value.questionId === line.value.question.id);
    if (res) {
      line.get('answerId').setValue(item.id);
      res.patchValue(line.value);
    }
  }

  onSave(){
    var val = this.formGroup.value;
    this.activeModal.close(val);
  }

  // handleChange(line : FormGroup){
    
  // }

  onCancel() {
    this.activeModal.dismiss();
  }

}
