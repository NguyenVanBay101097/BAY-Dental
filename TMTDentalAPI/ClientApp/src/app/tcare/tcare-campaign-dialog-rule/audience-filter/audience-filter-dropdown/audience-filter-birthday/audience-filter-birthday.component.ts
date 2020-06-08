import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { TCareRuleCondition } from 'src/app/tcare/tcare.service';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-audience-filter-birthday',
  templateUrl: './audience-filter-birthday.component.html',
  styleUrls: ['./audience-filter-birthday.component.css']
})
export class AudienceFilterBirthdayComponent implements OnInit {

  @Input() dataReceive: any;
  @Output() dataSend = new EventEmitter<any>();
  formGroup: FormGroup;

  AudienceFilter_Picker = {
    formula_types: ['lte'],
    formula_values: [],
    formula_displays: []
  }

  selected_AudienceFilter_Picker: TCareRuleCondition;

  constructor(private fb: FormBuilder) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      day: 0
    });

    this.selected_AudienceFilter_Picker = this.dataReceive;
    if (!this.dataReceive.op) {
      this.selected_AudienceFilter_Picker.op = this.AudienceFilter_Picker.formula_types[0]
    }
    if (this.dataReceive.value) {
      this.formGroup.patchValue({ day: this.selected_AudienceFilter_Picker.value });
    }
  }

  convertFormulaType(item) {
    switch (item) {
      case 'lte':
        return 'trước';
      case 'trước':
        return 'lte';
    }
  }

  selectFormulaType(item) {
    this.selected_AudienceFilter_Picker.op = item;
  }

  saveFormulaValue() {
    this.selected_AudienceFilter_Picker.value = this.formGroup.get('day').value;
    this.selected_AudienceFilter_Picker.displayValue = this.formGroup.get('day').value + ' ngày';
    this.dataSend.emit(false);
  }
}
