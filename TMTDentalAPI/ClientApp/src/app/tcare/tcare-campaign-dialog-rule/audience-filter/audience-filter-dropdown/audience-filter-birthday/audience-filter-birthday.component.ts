import { Component, OnInit, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';

@Component({
  selector: 'app-audience-filter-birthday',
  templateUrl: './audience-filter-birthday.component.html',
  styleUrls: ['./audience-filter-birthday.component.css']
})
export class AudienceFilterBirthdayComponent implements OnInit {

  formGroup: FormGroup;
  data: any;
  @Output() saveClick = new EventEmitter<any>();
  type: string;
  name: string;

  constructor(private fb: FormBuilder) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      day: [0, Validators.required]
    });

    if (this.data) {
      var day = parseInt(this.data.value) || 0;
      this.formGroup.get('day').setValue(day);
    }
  }

  onSave() {
    if (!this.formGroup.valid) {
      return false;
    }

    var day = this.formGroup.get('day').value;
    var res = {
      op: 'lte',
      opDisplay: 'trước',
      value: day + '',
      displayValue: day + ' ngày',
      type: this.type,
      name: this.name
    };

    this.saveClick.emit(res);
  }
}
