import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

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

  constructor(private fb: FormBuilder, public activeModal: NgbActiveModal) { }

  ngOnInit() {
    this.title = this.service.Name;
    this.formGroup = this.fb.group({
      steps: this.fb.array([])
    });

    const control = this.steps;
    control.clear();
    this.service.Steps.forEach(step => {
      var g = this.fb.group(step);
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
    if (val == null)
      return;

    console.log(val);
  }

  cancel() {
    this.activeModal.dismiss();
  }

  orderUp() {

  }

  orderDown() {

  }

  delete(i) {
    this.steps.removeAt(i);
  }

  add() {
    this.steps.push(this.fb.group({
      Id: null,
      Name: null, 
      Order: this.steps.length + 1, 
      IsDone: false
    }));
  }

  getValueFormSave() {
    var val = this.formGroup.value;
    return val;
  }
}
