import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { LaboBiteJointService } from '../labo-bite-joint.service';

@Component({
  selector: 'app-labo-bite-joint-cu-dialog',
  templateUrl: './labo-bite-joint-cu-dialog.component.html',
  styleUrls: ['./labo-bite-joint-cu-dialog.component.css']
})
export class LaboBiteJointCuDialogComponent implements OnInit {
  formGroup: FormGroup;
  id: string;
  title: string;

  submitted = false;

  get f() { return this.formGroup.controls; }

  constructor(private laboBiteJointService: LaboBiteJointService, public activeModal: NgbActiveModal, private notificationService: NotificationService,
    private fb: FormBuilder) {
  }
  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
    });

    this.default();
  }

  get form() {return this.formGroup;}
  get nameC() {return this.formGroup.get('name');}

  default() {
    if (this.id) {
      this.laboBiteJointService.get(this.id).subscribe((result: any) => {
        this.formGroup.patchValue(result);
      });
    } else {
    } }

    
  notify(Style, Content) {
    this.notificationService.show({
      content: Content,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: Style, icon: true }
    });
  }

  onSave() {
    this.submitted = true;

    if (!this.formGroup.valid) {
      return;
    }
    
    var val = this.formGroup.value;
    if (this.id) {
      this.laboBiteJointService.update(this.id, val).subscribe(() => {
        this.notify('success','Lưu thành công');
        this.activeModal.close(true);
      });
    } else {
      return this.laboBiteJointService.create(val).subscribe(result => {
        this.notify('success','Lưu thành công');
        this.activeModal.close(result);
      });;
    }
  }
}
