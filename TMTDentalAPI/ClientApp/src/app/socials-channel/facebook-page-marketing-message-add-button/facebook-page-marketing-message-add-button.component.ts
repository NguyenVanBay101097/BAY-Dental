import { Component, OnInit, Output, EventEmitter, Input, HostListener, ElementRef, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-facebook-page-marketing-message-add-button',
  templateUrl: './facebook-page-marketing-message-add-button.component.html',
  styleUrls: ['./facebook-page-marketing-message-add-button.component.css']
})
export class FacebookPageMarketingMessageAddButtonComponent implements OnInit {

  formGroup: FormGroup;
  @Output() saveClick = new EventEmitter<any>();
  @Output() clickOutside = new EventEmitter<any>();
  @Input() data: any;
  @ViewChild('textInput', { static: true }) textInput: ElementRef;

  constructor(private fb: FormBuilder, private _elementRef: ElementRef) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      type: 'web_url',
      url: ['', Validators.required],
      title: ['', Validators.required],
      payload: null
    });

    this.setValidators();

    if (this.data) {
      this.formGroup.patchValue(this.data);
    }
  }

  @HostListener('document:click', ['$event.target'])
  onMouseEnter(targetElement) {
    const clickedInside = this._elementRef.nativeElement.contains(targetElement);
    if (!clickedInside) {
      this.clickOutside.emit(null);
    }
  }

  get typeValue() {
    return this.formGroup.get('type').value;
  }

  setType(value) {
    this.formGroup.get('type').setValue(value);
  }

  focusTextInput() {
    this.textInput.nativeElement.focus();
  }

  onSave() {
    if (!this.formGroup.valid) {
      return false;
    }

    var value = this.formGroup.value;
    this.saveClick.emit(value);
  }

  setValidators() {
    const typeControl = this.formGroup.get('type');
    const urlControl = this.formGroup.get('url');
    const titleControl = this.formGroup.get('title');
    const payloadControl = this.formGroup.get('payload');

    typeControl.valueChanges
      .subscribe(type => {
        if (type === 'web_url') {
          urlControl.setValidators([Validators.required]);
          titleControl.setValidators([Validators.required]);
          payloadControl.setValidators(null);
        }

        if (type === 'phone_number') {
          urlControl.setValidators(null);
          titleControl.setValidators([Validators.required]);
          payloadControl.setValidators([Validators.required]);
        }

        urlControl.updateValueAndValidity();
        titleControl.updateValueAndValidity();
        payloadControl.updateValueAndValidity();
      });
  }
}
