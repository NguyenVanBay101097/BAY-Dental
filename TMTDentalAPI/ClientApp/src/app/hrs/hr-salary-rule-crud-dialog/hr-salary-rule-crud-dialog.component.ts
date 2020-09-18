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
  title: string;
  listAmountSelect = [
    // { text: 'Phần trăm (%)', value: 'percentage' },
    { text: 'Tiền cố định', value: 'fix' },
    { text: 'Công thức', value: 'code' },
  ];
  listamountCodeCompute: Array<any> = [
    { text: 'Lương chính', value: 'luong_chinh', description: '= số ngày công thực tế * (lương nhân viên / tổng số ngày công của tháng)' },
    { text: 'Hoa hồng', value: 'hoa_hong', description: '= (số phần trăm hoa hồng * tổng tiền dịch vụ được tính hoa hồng) / 100' },
    { text: 'Lương tháng 13', value: 'luong_thang13', description: '= (số tháng đi làm của năm * tiền lương)/12' }
  ];
  listamountPercentageBase: Array<{ text: string, value: string }> = [
    { text: 'Lương chính', value: 'luong_chinh' },
    { text: 'Hoa hồng', value: 'hoa_hong' }
  ];

  constructor(public activeModal: NgbActiveModal, private fb: FormBuilder, private activeroute: ActivatedRoute) { }

  ngOnInit() {
    this.RuleForm = this.fb.group({
      name: [null, [Validators.required]],
      code: null,
      sequence: 0,
      active: true,
      amountSelect: 'fix',
      amountFix: 0,
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

    if (value === 'fix') {
      this.amountfixControl.setValidators([Validators.required]);
    } else if (value === 'percentage') {
      this.amountPercentageControl.setValidators([Validators.required]);
      this.amountPercentageBaseControl.setValidators([Validators.required]);
    } else if (value === 'code') {
      this.amountCodeComputeControl.setValidators([Validators.required]);
    }

    this.amountfixControl.updateValueAndValidity();
    this.amountPercentageControl.updateValueAndValidity();
    this.amountPercentageBaseControl.updateValueAndValidity();
    this.amountCodeComputeControl.updateValueAndValidity();
  }

  onSave() {
    if (this.form.valid) {
      const entity = this.RuleForm.value;
      if (!entity.id) { delete entity.id; }
      this.activeModal.close(entity);
    }
  }

  getCodeDescription() {
    if (!this.amountCodeComputeControl.value) { return ''; }
    const item = this.listamountCodeCompute.find(x => x.value === this.amountCodeComputeControl.value);
    return item.description;
  }
}
