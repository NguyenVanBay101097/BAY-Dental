import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { validator } from 'fast-json-patch';

@Component({
  selector: 'app-salary-rule-crud-dialog',
  templateUrl: './hr-salary-rule-crud-dialog.component.html',
  styleUrls: ['./hr-salary-rule-crud-dialog.component.css']
})
export class HrSalaryRuleCrudDialogComponent implements OnInit {

  RuleForm: FormGroup;
  rule: any;
  listAmountSelect = [
    { text: 'Phần trăm (%)', value: 'percent' },
    { text: 'Tiền cố định', value: 'fixamount' },
    { text: 'Công thức', value: 'code' },
  ];
  listamountCodeCompute: Array<{ text: string, value: string }> = [
    { text: 'Lương chính', value: 'LC' },
    { text: 'Hoa hồng', value: 'HH' }
  ];
  listamountPercentageBase: Array<{ text: string, value: string }> = [
    { text: 'Lương chính', value: 'LC' },
    { text: 'Hoa hồng', value: 'HH' }
  ];

  constructor(public activeModal: NgbActiveModal, private fb: FormBuilder, private activeroute: ActivatedRoute) { }

  ngOnInit() {
    this.RuleForm = this.fb.group({
      name: [null, [Validators.required]],
      code: null,
      sequence: null,
      active: true,
      amountSelect: 'fixamount',
      amountFix: [null],
      amountPercentage: null,
      appearsOnPayslip: true,
      note: null,
      amountPercentageBase: null,
      amountCodeCompute: null,
      structId: null,
      id: null
    });
    setTimeout(() => {
      if (this.rule) {
        this.RuleForm.patchValue(this.rule);
        this.SetValidate(this.rule.amountSelect);
      } else {
        this.amountfixControl.setValidators([Validators.required]);
        this.amountfixControl.updateValueAndValidity();
      }
    });
  }

  get form() { return this.RuleForm; }
  get amountfixControl() { return this.form.get('amountFix'); }
  get amountPercentageControl() { return this.form.get('amountPercentage'); }
  get amountPercentageBaseControl() { return this.form.get('amountPercentageBase'); }
  get amountCodeComputeControl() { return this.form.get('amountCodeCompute'); }

  amountSelectChange(e) {
    const value = e.target.value;
    this.SetValidate(value);
  }

  SetValidate(value) {
    this.amountfixControl.setValidators(null);
    this.amountPercentageControl.setValidators(null);
    this.amountPercentageBaseControl.setValidators(null);
    this.amountCodeComputeControl.setValidators(null);

    if (value === 'fixamount') {
      {
        this.amountfixControl.setValidators([Validators.required]);

        this.amountPercentageControl.setValue(null);
        this.amountPercentageBaseControl.setValue(null);
        this.amountCodeComputeControl.setValue(null);
      }
    } else if (value === 'percent') {
      {
        this.amountPercentageControl.setValidators([Validators.required]);
        this.amountPercentageBaseControl.setValidators([Validators.required]);

        this.amountfixControl.setValue(null);
        this.amountCodeComputeControl.setValue(null);
      }
    } else if (value === 'code') {
      this.amountCodeComputeControl.setValidators([Validators.required]);

      this.amountPercentageControl.setValue(null);
      this.amountPercentageBaseControl.setValue(null);
      this.amountfixControl.setValue(null);
    }

    this.amountfixControl.updateValueAndValidity();
    this.amountPercentageControl.updateValueAndValidity();
    this.amountPercentageBaseControl.updateValueAndValidity();
    this.amountCodeComputeControl.updateValueAndValidity();
  }

  onSave() {
    if (this.form.valid) {
      this.activeModal.close(this.RuleForm.value);
    }
  }
}
