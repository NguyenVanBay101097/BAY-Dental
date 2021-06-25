import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbDropdown, NgbPopover } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-toa-thuoc-line-useat-popover',
  templateUrl: './toa-thuoc-line-useat-popover.component.html',
  styleUrls: ['./toa-thuoc-line-useat-popover.component.css']
})
export class ToaThuocLineUseatPopoverComponent implements OnInit {
  @Output() useAtFormGroup = new EventEmitter<any>();
  @Input() item: FormGroup;
  @Input() index: number;
  submitted = false;
  @ViewChild('popOver', { static: true }) public popover: NgbPopover;
  @ViewChild('myDrop', { static: true }) public myDrop: NgbDropdown;
  formGroup: FormGroup;

  useAtFilterOptions: any[] = [
    { text: 'Sau khi ăn', value: 'after_meal' },
    { text: 'Trước khi ăn', value: 'before_meal' },
    { text: 'Trong khi ăn', value: 'in_meal' },
    { text: 'Sau khi thức dậy', value: 'after_wakeup' },
    { text: 'Trước khi đi ngủ', value: 'before_sleep' },
    { text: 'Khác', value: 'other' },
  ];
  
  constructor(private fb: FormBuilder) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      useAt: null,
      note: null
    });
    this.reLoad();
  }

  reLoad() {
    if (this.item) {
      var line = this.item.value;
      this.formGroup.get('useAt').setValue(line.useAt);
      if (line.useAt == 'other') {
        this.formGroup.get('note').setValue(line.note);
        this.formGroup.get('note').setValidators(Validators.required);
        this.formGroup.get('note').updateValueAndValidity();
      }
    }
  }

  get useAtControl() {
    return this.formGroup.get('useAt').value;
  }

  get noteControl() {
    return this.formGroup.get('note').value;
  }

  get f() { return this.formGroup.controls; }

  onChangeUseAt() {
    if (this.useAtControl != 'other') {
      this.formGroup.get('note').clearValidators();
      this.formGroup.get('note').updateValueAndValidity();
      this.onSave();
    } else {
      this.formGroup.get('note').setValidators(Validators.required);
      this.formGroup.get('note').updateValueAndValidity();
    }
  }

  togglePopover(popOver) {
    if (popOver.isOpen()) {
      popOver.close();
    } else {
      this.formGroup.reset();
      this.reLoad();
      popOver.open();
    }
  }

  toggleDropdown(myDrop) {
    if (!myDrop) {
      myDrop.close();
    } else {
      this.formGroup.reset();
      this.reLoad();
      myDrop.open();
    }
  }

  onSave() {
    this.submitted = true;
    if (!this.formGroup.valid) {
      return;
    }

    const val = this.formGroup.value;
    this.useAtFormGroup.emit(val);
    // this.popover.close();
    this.myDrop.close();
  }

}
