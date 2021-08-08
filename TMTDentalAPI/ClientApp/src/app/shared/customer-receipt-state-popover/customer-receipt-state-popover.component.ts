import { Component, EventEmitter, HostListener, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbDropdown, NgbDropdownConfig, NgbPopover } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-customer-receipt-state-popover',
  templateUrl: './customer-receipt-state-popover.component.html',
  styleUrls: ['./customer-receipt-state-popover.component.css']
})
export class CustomerReceiptStatePopoverComponent implements OnInit {
  @Output() stateFormGroup = new EventEmitter<any>();
  @Input() item: any;
  submitted = false;
  @ViewChild('popOver', { static: true }) public popover: NgbPopover;
  @ViewChild('myDrop', { static: true }) myDrop: NgbDropdown;
  formGroup: FormGroup;

  stateFilterOptions: any[] = [
    { text: 'Chờ khám', value: 'waiting' },
    { text: 'Đang khám', value: 'examination' },
    { text: 'Hoàn thành', value: 'done' },
  ];
  constructor(private fb: FormBuilder,config: NgbDropdownConfig) { 
    config.placement = 'bottom';
  }

  ngOnInit() {
    this.formGroup = this.fb.group({
      state: null,
      isNoTreatment: false,
      reason: null
    });

    this.reLoad();
  }

  @HostListener('mouseover')
  onMouseOver() {
    event.stopPropagation();
  }

  @HostListener('mouseout')
  onMouseOut() {
    event.stopPropagation();
  }

  reLoad() {
    if (this.item) {
      this.formGroup.get('state').setValue(this.item.state);
      if (this.item.state == 'done' && this.item.isNoTreatment) {
        this.formGroup.get('isNoTreatment').setValue(this.item.isNoTreatment);
        this.formGroup.get('reason').setValue(this.item.reason);
      }
    }
  }

  get stateControl() {
    return this.formGroup.get('state').value;
  }

  get isNoTreatmentControl() {
    return this.formGroup.get('isNoTreatment').value;
  }

  get reasonControl() {
    return this.formGroup.get('reason').value;
  }

  get f() { return this.formGroup.controls; }

  onChangeState(state){
    if(state != 'done'){
      this.formGroup.get('isNoTreatment').setValue(false);
      this.formGroup.get('reason').setValue(null);
      this.f.reason.clearValidators();
      this.f.reason.updateValueAndValidity();
    }
  }

  checkIsNoTreatment(value){
    if (value == true && !this.item.isRepeatCustomer){
      this.f.reason.setValidators(Validators.required);
      this.f.reason.updateValueAndValidity();
    }
    else{
      this.f.reason.clearValidators();
      this.f.reason.updateValueAndValidity();
    }  
  }

  onSave() {
    this.submitted = true;
    if (!this.formGroup.valid) {
      return;
    }

    const val = this.formGroup.value;
    this.stateFormGroup.emit(val);
    // this.popover.close();
    this.myDrop.close();
  }

}
