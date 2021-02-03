import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { SurveyCallcontentService } from '../survey-callcontent.service';
import { SurveyUserinputDialogComponent } from '../survey-userinput-dialog/survey-userinput-dialog.component';
import { AssignmentActionDone, SurveyAssignmentDisplay, SurveyService } from '../survey.service';

@Component({
  selector: 'app-survey-assignment-form',
  templateUrl: './survey-assignment-form.component.html',
  styleUrls: ['./survey-assignment-form.component.css']
})
export class SurveyAssignmentFormComponent implements OnInit {
  formGroup: FormGroup;
  id: string;
  searchUpdate = new Subject<string>();
  surveyAssignment: SurveyAssignmentDisplay = new SurveyAssignmentDisplay();
  loading = false;

  constructor(
    private intlService: IntlService,
    private modalService: NgbModal,
    private surveyService: SurveyService,
    private route: ActivatedRoute,
    private callContentService: SurveyCallcontentService,
    private fb: FormBuilder
  ) { }

  ngOnInit() {

    this.route.queryParamMap.subscribe(params => {
      this.id = params.get('id');
    });

    // this.id = this.route.snapshot.paramMap.get('id');
    this.formGroup = this.fb.group({
      saleOrderId: null,
      saleOrder: null,
      surveyUserInput: null,
      callContents: this.fb.array([]),
      status: null
    });
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    if (this.id) {
      this.surveyService.get(this.id).subscribe(result => {
        this.surveyAssignment = result;
        console.log(this.surveyAssignment);
        this.formGroup.patchValue(this.surveyAssignment);
        // let dateOrder = new Date(result.dateOrder);
        // this.formGroup.get('dateOrderObj').patchValue(dateOrder);

        let control = this.formGroup.get('callContents') as FormArray;
        control.clear();
        result.callContents.forEach(line => {
          var g = this.fb.group(line);
          control.push(g);
        });
      });
    }
  }

  showTeethDiagnostic(line) {
    var list = [];
    if (line.teeth.length) {
      list.push(line.teeth.map(x => x.name).join(','));
    }

    if (line.diagnostic) {
      list.push(line.diagnostic);
    }

    return list.join('; ');
  }

  showTeethDkLine(line) {
    var list = [];
    if (line.teeth.length) {
      list.push(line.teeth.map(x => x.name).join(','));
    }
    return list.join('; ');
  }

  actionContact() {
    if (this.surveyAssignment.id) {
      this.surveyService.actionContact([this.surveyAssignment.id]).subscribe(() => {
        this.loadDataFromApi();
      });
    }
  }

  actionCancel() {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Hủy khảo sát đánh giá';
    modalRef.componentInstance.body = 'Bạn có chắc chắn hủy kết quả khảo sát đánh giá ?';
    modalRef.result.then(() => {
      this.surveyService.actionCancel([this.surveyAssignment.id]).subscribe(() => {
        this.loadDataFromApi();
      });
    }); 
  }

  getViewSurvey(){
    let modalRef = this.modalService.open(SurveyUserinputDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thông tin khảo sát đánh giá';
    modalRef.componentInstance.id = this.surveyAssignment.userInputId;
    modalRef.componentInstance.surveyAssignmentId = this.surveyAssignment.id;
    modalRef.componentInstance.surveyAssignmentStatus = this.surveyAssignment.status;
    modalRef.result.then(() => {
        this.loadDataFromApi();
    }, () => {
    });
  }


  actionDone() {
    let modalRef = this.modalService.open(SurveyUserinputDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thông tin khảo sát đánh giá';
    modalRef.componentInstance.id = this.surveyAssignment.userInputId;
    modalRef.componentInstance.surveyAssignmentId = this.surveyAssignment.id;
    modalRef.componentInstance.surveyAssignmentStatus = this.surveyAssignment.status;
    modalRef.result.then(rs => {
      var val = new AssignmentActionDone();
      val.id = this.surveyAssignment.id;
      val.surveyUserInput = rs;
      this.surveyService.actionDone(val).subscribe(() => {
        this.loadDataFromApi();
      });
    }, () => {
    });
  }




  GetStateSaleOrder(state) {
    switch (state) {
      case 'draft':
        return 'Nháp';
      case 'confirm':
        return 'Xác nhận';
      case 'done':
        return 'Hoàn thành '
    }
  }

}
