import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { SaleOrderLinesOdataService } from 'src/app/shared/services/sale-order-linesOdata.service';

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
    private saleOrderLinesOdataService: SaleOrderLinesOdataService) { }

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
    this.saleOrderLinesOdataService.update(this.service.Id, val).subscribe(
      (result) => { 
        this.activeModal.close(true);
      },
      (error) => { }
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
      temp = this.steps.at(i).get('Order').value;
      this.steps.at(i).get('Order').setValue(this.steps.at(i-1).get('Order').value);
      this.steps.at(i-1).get('Order').setValue(temp);
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
    this.steps.removeAt(i);
    this.steps.markAsDirty();
  }

  add() {
    this.steps.push(this.fb.group({
      Id: null,
      Name: null, 
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
}
