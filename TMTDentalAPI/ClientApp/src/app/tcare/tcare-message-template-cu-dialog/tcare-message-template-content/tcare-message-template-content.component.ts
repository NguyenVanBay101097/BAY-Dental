import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { validator } from 'fast-json-patch';

@Component({
  selector: 'app-tcare-message-template-content',
  templateUrl: './tcare-message-template-content.component.html',
  styleUrls: ['./tcare-message-template-content.component.css']
})
export class TcareMessageTemplateContentComponent implements OnInit {

  formGroup: FormGroup;
  @Input() template: any;
  @Input() index: any;
  @Output() valueChange = new EventEmitter();
  //cá nhân hóa
  showPluginTextarea: boolean = false;
  selectArea_start: number;
  selectArea_end: number;
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
    this.dataChange();
  }

  dataChange() {
    if (this.formGroup.invalid) { return; }
    const val = this.formGroup.value;
    this.valueChange.emit({index: this.index, template: val});
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
    var limit = 640;
    var text = this.formGroup.get('text').value;
    if (text) {
      return limit - text.length;
    } else {
      return limit;
    }
  }

  addContentPluginTextarea(value) {
    if (this.formGroup.value.text) {
      this.formGroup.patchValue({
        text: this.formGroup.value.text.slice(0, this.selectArea_start) + value + this.formGroup.value.text.slice(this.selectArea_end)
      });
      this.selectArea_start = this.selectArea_start + value.length;
      this.selectArea_end = this.selectArea_start;
    } else {
      this.formGroup.patchValue({
        text: value
      });
    }
    this.dataChange();
  }

}
