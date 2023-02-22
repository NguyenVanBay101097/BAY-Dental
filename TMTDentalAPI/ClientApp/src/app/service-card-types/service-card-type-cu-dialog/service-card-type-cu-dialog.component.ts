import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ServiceCardTypeService } from '../service-card-type.service';

@Component({
  selector: 'app-service-card-type-cu-dialog',
  templateUrl: './service-card-type-cu-dialog.component.html',
  styleUrls: ['./service-card-type-cu-dialog.component.css']
})
export class ServiceCardTypeCuDialogComponent implements OnInit {

  formGroup: FormGroup;
  title: string;
  id: string;

  submitted = false;

  get f() { return this.formGroup.controls; }

  constructor(private fb: FormBuilder, public activeModal: NgbActiveModal,
    private cardTypeService: ServiceCardTypeService) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      price: 0,
      amount: 0,
      period: ['month', Validators.required],
      nbrPeriod: 1
    });

    if (this.id) {
      this.cardTypeService.get(this.id).subscribe((result: any) => {
        this.formGroup.patchValue(result);
      });
    }
  }

  onSave() {
    this.submitted = true;

    if (!this.formGroup.valid) {
      return false;
    }

    var value = this.formGroup.value;

    if (this.id) {
      this.cardTypeService.update(this.id, value).subscribe(() => {
        this.activeModal.close(true);
      });
    } else {
      this.cardTypeService.create(value).subscribe((result: any) => {
        this.activeModal.close(true);
      });
    }
  }
}
