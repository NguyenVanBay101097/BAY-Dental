import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { LaboFinishLineService } from '../labo-finish-line.service';

@Component({
  selector: 'app-labo-finish-line-cu-dialog',
  templateUrl: './labo-finish-line-cu-dialog.component.html',
  styleUrls: ['./labo-finish-line-cu-dialog.component.css']
})
export class LaboFinishLineCuDialogComponent implements OnInit {
  formGroup: FormGroup;
  id: string;
  title: string;

  constructor(private laboFinishLineservice: LaboFinishLineService, public activeModal: NgbActiveModal, public notificationService: NotificationService,
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
      this.laboFinishLineservice.get(this.id).subscribe((result: any) => {
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
    if (!this.formGroup.valid) {
      return;
    }
    var val = this.formGroup.value;
    if (this.id) {
      this.laboFinishLineservice.update(this.id, val).subscribe(() => {
        this.notify('success','Lưu thành công');
        this.activeModal.close(true);
      });
    } else {
      return this.laboFinishLineservice.create(val).subscribe(result => {
        this.notify('success','Lưu thành công');
        this.activeModal.close(result);
      });;
    }
  }
}
