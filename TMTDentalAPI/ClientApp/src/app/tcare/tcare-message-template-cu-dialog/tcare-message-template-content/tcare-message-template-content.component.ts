import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-tcare-message-template-content',
  templateUrl: './tcare-message-template-content.component.html',
  styleUrls: ['./tcare-message-template-content.component.css']
})
export class TcareMessageTemplateContentComponent implements OnInit {

  @ViewChild('content', { static: true }) content: ElementRef;
  formGroup: FormGroup;
  @Input() template: any;
  @Input() index: any;
  @Input() textareaLength: any;

  //cá nhân hóa
  tabs = [
    { name: 'Tên khách hàng', value: '{ten_khach_hang}'},
    { name: 'Họ tên khách hàng', value: '{ho_ten_khach_hang}'},
    { name: 'Tên trang', value: '{ten_page}'},
    { name: 'Danh xưng khách hàng', value: '{danh_xung_khach_hang}'},
    { name: 'mã coupon', value: '{ma_coupon}'},
  ];
  showPluginTextarea: boolean = false;
  selectArea_start: number = 0;
  selectArea_end: number = 0;
  constructor(
    private fb: FormBuilder
  ) { }

  ngOnInit() {
    console.log(this.template);
    this.formGroup = this.fb.group({
      templateType: this.template.templateType,
      text: [this.template.text, Validators.required]
    });

  }

  get textControl() { return this.formGroup.get('text'); }

  onTextChange(e) {
    this.textControl.setValue(e.target.value);
    // this.dataChange();
  }

  onSave() {
    if (this.formGroup.invalid) { return; }
    const val = this.formGroup.value;
    return { index: this.index, template: val };
  }
  //cá nhân hóa
  hideEmoji() {
    this.showPluginTextarea = false;
  }

  selectArea(event) {
    this.selectArea_start = event.target.selectionStart;
    this.selectArea_end = event.target.selectionEnd;
  }

  showEmoji() {
    this.showPluginTextarea = true;
  }

  getLimitText() {
    // var limit = 640;
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

    } else {
      this.formGroup.patchValue({
        text: value
      });
    }
    this.selectArea_start = this.selectArea_start + value.length;
    this.selectArea_end = this.selectArea_start;
    this.content.nativeElement.focus();
    this.content.nativeElement.selectionEnd = this.selectArea_end;
    this.content.nativeElement.selectionStart = this.selectArea_start;
  }

  emotionClick(e) {
this.addToContent(e.emoji.native);
  }

}
