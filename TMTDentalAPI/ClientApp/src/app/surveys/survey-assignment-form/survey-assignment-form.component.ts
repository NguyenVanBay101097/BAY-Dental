import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { SurveyCallcontentService } from '../survey-callcontent.service';
import { SurveyUserinputCreateDialogComponent } from '../survey-userinput-create-dialog/survey-userinput-create-dialog.component';
import { SurveyUserinputDialogComponent } from '../survey-userinput-dialog/survey-userinput-dialog.component';
import { AssignmentActionDone, SurveyAssignmentDisplay, SurveyAssignmentDisplayCallContent, SurveyAssignmentService } from '../survey.service';

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
    private surveyAssignmentService: SurveyAssignmentService,
    private route: ActivatedRoute,
    private notificationService: NotificationService,
    private callContentService: SurveyCallcontentService,
    private fb: FormBuilder
  ) { }

  ngOnInit() {
    this.route.queryParamMap.subscribe(params => {
      this.id = params.get('id');
      this.loadDataFromApi();
    });

    // this.id = this.route.snapshot.paramMap.get('id');
    this.formGroup = this.fb.group({
      saleOrderId: null,
      saleOrder: null,
      surveyUserInput: null,
      callContents: this.fb.array([]),
      status: null
    });
  }

  loadDataFromApi() {
    if (this.id) {
      this.surveyAssignmentService.get(this.id).subscribe(result => {
        this.surveyAssignment = result;
        this.formGroup.patchValue(this.surveyAssignment);
        // // let dateOrder = new Date(result.dateOrder);
        // // this.formGroup.get('dateOrderObj').patchValue(dateOrder);

        // let control = this.formGroup.get('callContents') as FormArray;
        // control.clear();
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
    this.surveyAssignmentService.actionContact([this.surveyAssignment.id]).subscribe(() => {
      this.surveyAssignment.status = "contact";
    });
  }

  actionCancel() {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Hủy khảo sát đánh giá';
    modalRef.componentInstance.body = 'Bạn có chắc chắn hủy kết quả khảo sát đánh giá ?';
    modalRef.result.then(() => {
      this.surveyAssignmentService.actionCancel([this.surveyAssignment.id]).subscribe(() => {
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

  onAddCallContent() {
    this.surveyAssignment.callContents.push(new SurveyAssignmentDisplayCallContent());
  }

  onCancelCallContent(data) {
    this.surveyAssignment.callContents.splice(data.index, 1);
  }

  onRemoveCallContent(index) {
    var item = this.surveyAssignment.callContents[index];
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa cuộc gọi';
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn xóa cuộc gọi này?';
    modalRef.result.then(() => {
      this.callContentService.remove(item.id).subscribe(() => {
        this.notificationService.show({
          content: 'Xóa thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.surveyAssignment.callContents.splice(index, 1);
      });
    }); 
  }

  onSaveCallContent({data, index}) {
    var item = this.surveyAssignment.callContents[index];
    if (item.id) {
      this.callContentService.update(item.id, data).subscribe(() => {
        item = Object.assign(item, data);
      });
    } else {
      this.callContentService.create({assignmentId: this.id, name: data.name}).subscribe((result) => {
        item = Object.assign(item, result);
      });
    }
  }

  actionDone() {
    let modalRef = this.modalService.open(SurveyUserinputCreateDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thông tin khảo sát đánh giá';
    modalRef.componentInstance.id = this.surveyAssignment.userInputId;
    modalRef.componentInstance.surveyAssignmentId = this.surveyAssignment.id;
    modalRef.componentInstance.surveyAssignmentStatus = this.surveyAssignment.status;
    modalRef.result.then(rs => {
      var val = new AssignmentActionDone();
      val.id = this.surveyAssignment.id;
      val.surveyUserInput = rs;
      this.surveyAssignmentService.actionDone(val).subscribe(() => {
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

  get getAmountTotal() {
    return this.formGroup.get('saleOrder').value.amountTotal;
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
