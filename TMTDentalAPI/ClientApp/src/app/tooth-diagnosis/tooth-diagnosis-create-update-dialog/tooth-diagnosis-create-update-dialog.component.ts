import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { ToothDiagnosisService } from '../tooth-diagnosis.service';

@Component({
  selector: 'app-tooth-diagnosis-create-update-dialog',
  templateUrl: './tooth-diagnosis-create-update-dialog.component.html',
  styleUrls: ['./tooth-diagnosis-create-update-dialog.component.css']
})
export class ToothDiagnosisCreateUpdateDialogComponent implements OnInit {

  title: string;
  itemId: string;
  myForm: FormGroup;
  
  submitted = false;
  constructor(
    private fb: FormBuilder, 
    public activeModal: NgbActiveModal,
    private toothDiagnosisService: ToothDiagnosisService,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.myForm = this.fb.group({
      name: [null, Validators.required]
    });
    if (this.itemId) {
      setTimeout(() => {
        this.toothDiagnosisService.get(this.itemId).subscribe((result) => {
          this.myForm.patchValue(result);
        }, err => {
          console.log(err);
          this.activeModal.dismiss();
        });
      });
    }
  }

  get f() {
    return this.myForm.controls;
  }

  onSave(){
    this.submitted = true;

    if (!this.myForm.valid) {
      return false;
    }

    var value = this.myForm.value;
    if (!this.itemId) {
      this.toothDiagnosisService.create(value).subscribe(result => {
        this.submitted = false;
        this.activeModal.close(result);
        this.notificationService.show({
          content: 'Lưu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
      }, err => {
        console.log(err);
        this.submitted = false;
      });
    } else {
      this.toothDiagnosisService.update(this.itemId, value).subscribe(result => {
        this.submitted = false;
        this.activeModal.close(result);
        this.notificationService.show({
          content: 'Lưu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
      }, err => {
        console.log(err);
        this.submitted = false;
      });
    }
  }

}
