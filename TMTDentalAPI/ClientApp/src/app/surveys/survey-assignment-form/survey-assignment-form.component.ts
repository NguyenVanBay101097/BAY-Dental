import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { SurveyCallcontentService } from '../survey-callcontent.service';
import { SurveyUserinputCreateDialogComponent } from '../survey-userinput-create-dialog/survey-userinput-create-dialog.component';
import { SurveyUserinputDialogComponent } from '../survey-userinput-dialog/survey-userinput-dialog.component';
import { SurveyAssignmentDisplay, SurveyAssignmentDisplayCallContent, SurveyAssignmentService } from '../survey.service';

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
        console.log(result);
        
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
    modalRef.componentInstance.title = 'H???y kh???o s??t ????nh gi??';
    modalRef.componentInstance.body = 'B???n c?? ch???c ch???n h???y k???t qu??? kh???o s??t ????nh gi?? ?';
    modalRef.result.then(() => {
      this.surveyAssignmentService.actionCancel([this.surveyAssignment.id]).subscribe(() => {
        this.notificationService.show({
          content: 'H???y th??nh c??ng',
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
    modalRef.componentInstance.title = 'Th??ng tin kh???o s??t ????nh gi??';
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
    modalRef.componentInstance.title = 'X??a cu???c g???i';
    modalRef.componentInstance.body = 'B???n c?? ch???c ch???n mu???n x??a cu???c g???i n??y?';
    modalRef.result.then(() => {
      this.callContentService.remove(item.id).subscribe(() => {
        this.notificationService.show({
          content: 'X??a th??nh c??ng',
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
    modalRef.componentInstance.title = 'Th??ng tin kh???o s??t ????nh gi??';
    modalRef.componentInstance.surveyAssignmentId = this.surveyAssignment.id;

    if (this.surveyAssignment.userInput) {
      modalRef.componentInstance.id = this.surveyAssignment.userInput.id;
    }

    if (this.surveyAssignment.status == 'done') {
      modalRef.componentInstance.disable = true;
    }

    modalRef.result.then(() => {
      this.notificationService.show({
        content: 'Ho??n th??nh kh???o s??t ????nh gi??',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
      this.loadDataFromApi();
           
    }, () => {
    });
  }

  actionViewUserInput() {
    let modalRef = this.modalService.open(SurveyUserinputDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Th??ng tin kh???o s??t ????nh gi??';
    modalRef.componentInstance.id = this.surveyAssignment.userInputId;
    modalRef.componentInstance.surveyAssignmentId = this.surveyAssignment.id;
    modalRef.componentInstance.surveyAssignmentStatus = this.surveyAssignment.status;
    modalRef.result.then(rs => {
      
      this.loadDataFromApi();
    }, () => {
    });
  }

  get getAmountTotal() {
    return this.formGroup.get('saleOrder').value.amountTotal;
  }




  GetStateSaleOrder(state) {
    switch (state) {
      case 'draft':
        return 'Nh??p';
      case 'confirm':
        return 'X??c nh???n';
      case 'done':
        return 'Ho??n th??nh '
    }
  }

}
