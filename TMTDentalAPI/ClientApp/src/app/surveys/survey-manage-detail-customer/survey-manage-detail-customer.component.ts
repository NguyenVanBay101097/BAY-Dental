import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { SurveyUserinputDialogComponent } from '../survey-userinput-dialog/survey-userinput-dialog.component';
import { AssignmentActionDone, SurveyAssignmentDisplay, SurveyAssignmentService } from '../survey.service';

@Component({
  selector: 'app-survey-manage-detail-customer',
  templateUrl: './survey-manage-detail-customer.component.html',
  styleUrls: ['./survey-manage-detail-customer.component.css']
})
export class SurveyManageDetailCustomerComponent implements OnInit {

  formGroup: FormGroup;
  id: string;
  searchUpdate = new Subject<string>();
  surveyAssignment: SurveyAssignmentDisplay = new SurveyAssignmentDisplay();
  loading = false;

  constructor(
    private modalService: NgbModal,
    private surveyService: SurveyAssignmentService,
    private activateRoute: ActivatedRoute,
    private notificationService: NotificationService,
    private fb: FormBuilder
  ) { }

  ngOnInit() {
    this.id = this.activateRoute.parent.snapshot.paramMap.get('id');
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
        this.formGroup.patchValue(this.surveyAssignment);
        // let dateOrder = new Date(result.dateOrder);
        // this.formGroup.get('dateOrderObj').patchValue(dateOrder);

        let control = this.formGroup.get('callContents') as FormArray;
        control.clear();
        // result.callContents.forEach(line => {
        //   var g = this.fb.group(line);
        //   control.push(g);
        // });
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
        this.notificationService.show({
          content: 'Hủy thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.loadDataFromApi();
      });
    });
  }

  getViewSurvey() {
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
        this.notificationService.show({
          content: 'Hoàn thành khảo sát đánh giá',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
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
