import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { LaboBridgeService } from '../labo-bridge.service';

@Component({
  selector: 'app-labo-bridge-cu-dialog',
  templateUrl: './labo-bridge-cu-dialog.component.html',
  styleUrls: ['./labo-bridge-cu-dialog.component.css']
})
export class LaboBridgeCuDialogComponent implements OnInit {
  formGroup: FormGroup;
  id: string;
  title: string;

  constructor(private laboBridgeService: LaboBridgeService, public activeModal: NgbActiveModal,
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
      this.laboBridgeService.get(this.id).subscribe((result: any) => {
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
      this.laboBridgeService.update(this.id, val).subscribe(() => {
        this.activeModal.close(true);
      });
    } else {
      return this.laboBridgeService.create(val).subscribe(result => {
        this.activeModal.close(result);
      });;
    }
  }
}
