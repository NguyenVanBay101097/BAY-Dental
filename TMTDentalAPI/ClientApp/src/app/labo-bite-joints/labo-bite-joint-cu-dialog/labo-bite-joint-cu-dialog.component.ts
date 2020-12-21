import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
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

  constructor(private laboBiteJointService: LaboBiteJointService, public activeModal: NgbActiveModal,
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

  onSave() {
    if (!this.formGroup.valid) {
      return;
    }
    var val = this.formGroup.value;
    if (this.id) {
      this.laboBiteJointService.update(this.id, val).subscribe(() => {
        this.activeModal.close(true);
      });
    } else {
      return this.laboBiteJointService.create(val).subscribe(result => {
        this.activeModal.close(result);
      });;
    }
  }
}
