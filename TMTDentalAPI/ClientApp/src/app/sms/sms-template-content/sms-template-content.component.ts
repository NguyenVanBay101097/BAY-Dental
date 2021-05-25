import { Input, OnChanges, SimpleChanges } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-sms-template-content',
  templateUrl: './sms-template-content.component.html',
  styleUrls: ['./sms-template-content.component.css']
})
export class SmsTemplateContentComponent implements OnInit, OnChanges {

  formGroup: FormGroup;
  @Input() type: any;
  @Input() template: any;
  @Input() textareaLength: any;

  tabs: Array<{name: string, value: string}> = [];

  selectArea_start: number = 0;
  selectArea_end: number = 0;

  listTabs = {
    "birthday": [
      { name: 'Danh xưng', value: '{danh_xung}' },
      { name: 'Tên khách hàng', value: '{ten_khach_hang}' },
      { name: 'Ngày sinh', value: '{ngay_sinh}' }
    ], 
    "appointment": [
      { name: 'Danh xưng', value: '{danh_xung}' },
      { name: 'Tên khách hàng', value: '{ten_khach_hang}' },
      { name: 'Giờ hẹn', value: '{gio_hen}' },
      { name: 'Ngày hẹn', value: '{ngay_hen}' },
      { name: 'Bác sĩ', value: '{bac_si}' }
    ], 
    "template": [
      { name: 'Danh xưng', value: '{danh_xung}' },
      { name: 'Tên khách hàng', value: '{ten_khach_hang}' },
      { name: 'Ngày sinh', value: '{ngay_sinh}' },
      { name: 'Giờ hẹn', value: '{gio_hen}' },
      { name: 'Ngày hẹn', value: '{ngay_hen}' },
      { name: 'Bác sĩ', value: '{bac_si}' }
    ], 
    "care_after_order": [
      { name: 'Danh xưng', value: '{danh_xung}' },
      { name: 'Tên khách hàng', value: '{ten_khach_hang}' },
      { name: 'Số phiếu điều trị', value: '{so_phieu_dieu_tri}' },
      { name: 'Dịch vụ', value: '{dich_vu}' },
      { name: 'Bác sĩ', value: '{bac_si}' }
    ], 
    "thanks": [
      { name: 'Danh xưng', value: '{danh_xung}' },
      { name: 'Tên khách hàng', value: '{ten_khach_hang}' },
    ], 
    "campaign": [
      { name: 'Danh xưng', value: '{danh_xung}' },
      { name: 'Tên khách hàng', value: '{ten_khach_hang}' },
    ]
  }

  constructor(
    private fb: FormBuilder
  ) { }
  
  ngOnChanges(changes: SimpleChanges): void {
    this.ngOnInit();
  }

  ngOnInit() {
    this.formGroup = this.fb.group({
      templateType: this.template.templateType,
      text: [this.template.text, Validators.required]
    });
    this.textControl.markAsTouched();
    this.textControl.markAsDirty();

    if (this.listTabs[this.type]) {
      this.tabs = this.listTabs[this.type];
    }
  }

  get textControl() { return this.formGroup.get('text'); }

  onTextChange(e) {
    this.textControl.setValue(e.target.value);
    this.template.text = this.textControl.value;
  }

  selectArea(event) {
    this.selectArea_start = event.target.selectionStart;
    this.selectArea_end = event.target.selectionEnd;
  }

  getLimitText() {
    var text = this.formGroup.get('text').value;
    if (text) {
      return this.textareaLength - text.length;
    } else {
      return this.textareaLength;
    }
  }

  addToContent(value) {
    if (this.formGroup.value.text) {
      this.formGroup.patchValue({
        text: this.formGroup.value.text.slice(0, this.selectArea_start) + value + this.formGroup.value.text.slice(this.selectArea_end)
      });
      this.template.text = this.textControl.value;
    } else {
      this.formGroup.patchValue({
        text: value
      });
      this.template.text = this.textControl.value;
    }
    this.selectArea_start = this.selectArea_start + value.length;
    this.selectArea_end = this.selectArea_start;
    // this.content.nativeElement.focus();
    // this.content.nativeElement.selectionEnd = this.selectArea_end;
    // this.content.nativeElement.selectionStart = this.selectArea_start;
  }

}
