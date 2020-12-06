import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { SaleOrderLinesOdataService } from 'src/app/shared/services/sale-order-linesOdata.service';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';

@Component({
  selector: 'app-treatment-process-service-dialog',
  templateUrl: './treatment-process-service-dialog.component.html',
  styleUrls: ['./treatment-process-service-dialog.component.css']
})
export class TreatmentProcessServiceDialogComponent implements OnInit {
  formGroup: FormGroup;
  title: string = null;
  service: any;
  saveDefault: boolean = false;

  constructor(private fb: FormBuilder, public activeModal: NgbActiveModal, 
    private saleOrderLinesOdataService: SaleOrderLinesOdataService, 
    private errorService: AppSharedShowErrorService, 
    private notificationService: NotificationService, 
    private modalService: NgbModal) { }

  ngOnInit() {
    this.title = this.service.Name;
    this.formGroup = this.fb.group({
      steps: this.fb.array([])
    });

    const control = this.steps;
    control.clear();
    this.service.Steps.forEach(step => {
      var g = this.fb.group(step);
      g.get('Name').setValidators(Validators.required);
      control.push(g);
    });
    this.formGroup.markAsPristine();
  }

  get steps() { return this.formGroup.get('steps') as FormArray; }

  save() {
    if (!this.formGroup.valid) {
      return;
    }

    var val = this.getValueFormSave();

    this.saleOrderLinesOdataService.update(this.service.Id, val).subscribe(
      (result) => { 
        this.activeModal.close(true);
      },
      (error) => { 
        this.errorService.show(error);
      }
    );
  }

  cancel() {
    this.activeModal.dismiss();
  }

  orderUp(i) {
    if (i > 0) {
      var temp = this.steps.at(i).value;
      this.steps.at(i).patchValue(this.steps.at(i-1).value);
      this.steps.at(i-1).patchValue(temp);
    }
  }

  orderDown(i) {
    if (i < this.steps.length - 1) {
      var temp = this.steps.at(i).value;
      this.steps.at(i).patchValue(this.steps.at(i+1).value);
      this.steps.at(i+1).patchValue(temp);
    }
  }

  delete(i) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa công đoạn điều trị';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.steps.removeAt(i);
      this.steps.markAsDirty();
    });
  }

  add() {
    this.steps.push(this.fb.group({
      Id: null,
      Name: [null, Validators.required], 
      IsDone: false, 
      Order: this.steps.length + 1
    }));
    this.steps.markAsDirty();
  }

  getValueFormSave() {
    for (let i = 0; i <  this.steps.length; i++) {
      this.steps.at(i).get('Order').setValue(i+1);
    }

    var val = {
      Id: this.service.Id, 
      Steps: this.steps.value, 
      Default: this.saveDefault
    };
    return val;
  }

  notify(Style, Content) {
    this.notificationService.show({
      content: Content,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: Style, icon: true }
    });
  }
}
