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
  @Input() template: any;
  @Input() textareaLength: any;

  //cá nhân hóa
  tabs = [
    { name: 'Tên khách hàng', value: '{ten_khach_hang}' },
    { name: 'Họ tên khách hàng', value: '{ho_ten_khach_hang}' },
    { name: 'Tên công ty', value: '{ten_cong_ty}' },
    { name: 'Danh xưng khách hàng', value: '{danh_xung_khach_hang}' },
  ];

  selectArea_start: number = 0;
  selectArea_end: number = 0;
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
