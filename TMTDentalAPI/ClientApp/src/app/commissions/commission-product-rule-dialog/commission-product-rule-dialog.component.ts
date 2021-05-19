import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-commission-product-rule-dialog',
  templateUrl: './commission-product-rule-dialog.component.html',
  styleUrls: ['./commission-product-rule-dialog.component.css']
})
export class CommissionProductRuleDialogComponent implements OnInit {

  myform: FormGroup;
  title: string;
  submitted = false;

  get f() { return this.myform.controls; }

  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
  ) { }

  ngOnInit() {
    this.myform = this.fb.group({
      percent: [0, Validators.required]
    });
  }

  onSet() {
    this.submitted = true;

    if (!this.myform.valid) {
      return;
    }

    this.activeModal.close(this.myform.value);
  }

}
