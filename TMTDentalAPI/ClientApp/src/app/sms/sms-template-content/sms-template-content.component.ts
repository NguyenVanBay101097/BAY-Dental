import { ElementRef, Input, OnChanges, SimpleChanges, ViewChild } from '@angular/core';
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
  @ViewChild('textarea', {static: false}) textarea: ElementRef;

  tabs: Array<{name: string, value: string}> = [];

  listTabs = {
    "partner": [
      { name: 'Danh xưng', value: '{danh_xung}' },
      { name: 'Tên khách hàng', value: '{ten_khach_hang}' },
      { name: 'Ngày sinh', value: '{ngay_sinh}' }
    ], 
    "appointment": [
      { name: 'Danh xưng', value: '{danh_xung}' },
      { name: 'Tên khách hàng', value: '{ten_khach_hang}' },
      { name: 'Giờ hẹn', value: '{gio_hen}' },
      { name: 'Ngày hẹn', value: '{ngay_hen}' },
      { name: 'Bác sĩ', value: '{bac_si_lich_hen}' }
    ], 
    // "template": [
    //   { name: 'Danh xưng', value: '{danh_xung}' },
    //   { name: 'Tên khách hàng', value: '{ten_khach_hang}' },
    //   { name: 'Ngày sinh', value: '{ngay_sinh}' },
    //   { name: 'Giờ hẹn', value: '{gio_hen}' },
    //   { name: 'Ngày hẹn', value: '{ngay_hen}' },
    //   { name: 'Bác sĩ', value: '{bac_si}' }
    // ], 
    "saleOrderLine": [
      { name: 'Danh xưng', value: '{danh_xung}' },
      { name: 'Tên khách hàng', value: '{ten_khach_hang}' },
      { name: 'Số phiếu điều trị', value: '{so_phieu_dieu_tri}' },
      { name: 'Dịch vụ', value: '{dich_vu}' },
      { name: 'Bác sĩ', value: '{bac_si}' }
    ], 
    "saleOrder": [
      { name: 'Danh xưng', value: '{danh_xung}' },
      { name: 'Tên khách hàng', value: '{ten_khach_hang}' },
    ], 
    "partnerCampaign": [
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
  get textValue() { return this.formGroup.get('text').value; }

  onTextChange() {
    this.template.text = this.textValue;
  }

  getLimitText() {
    return this.textValue ? this.textareaLength - this.textValue.length : this.textareaLength;
  }

  addToContent(tabValue) {
    const selectionStart = this.textarea.nativeElement.selectionStart;
    const selectionEnd = this.textarea.nativeElement.selectionEnd;
    var tabValueNew = tabValue;
    if (this.textValue) {
      tabValueNew = ((selectionStart > 0 && this.textValue[selectionStart - 1] == ' ') ? "" : " ") 
                    + tabValue 
                    + ((selectionEnd < this.textareaLength && this.textValue[selectionEnd] == ' ') ? "" : " ");
      this.formGroup.patchValue({
        text: this.textValue.slice(0, selectionStart) 
              + tabValueNew
              + this.textValue.slice(this.textarea.nativeElement.selectionEnd)
      });
    } else {
      this.formGroup.patchValue({ text: tabValue });
    }
    this.template.text = this.textValue;

    this.textarea.nativeElement.focus();
    this.textarea.nativeElement.setSelectionRange(selectionStart + tabValueNew.length, selectionStart + tabValueNew.length);
  }

}
