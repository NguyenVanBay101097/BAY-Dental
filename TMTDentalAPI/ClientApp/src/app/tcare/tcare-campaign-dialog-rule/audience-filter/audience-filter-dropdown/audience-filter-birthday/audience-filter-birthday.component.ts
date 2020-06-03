import { Component, OnInit, Input } from '@angular/core';
import { AudienceFilterItem } from 'src/app/tcare/tcare.service';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-audience-filter-birthday',
  templateUrl: './audience-filter-birthday.component.html',
  styleUrls: ['./audience-filter-birthday.component.css']
})
export class AudienceFilterBirthdayComponent implements OnInit {

  @Input() dataReceive: any;
  formGroup: FormGroup;

  AudienceFilter_Picker = {
    formula_types: ['before'],
    formula_values: [],
    formula_displays: null
  }

  selected_AudienceFilter_Picker: AudienceFilterItem;

  constructor(private fb: FormBuilder) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      day: null
    });

    this.selected_AudienceFilter_Picker = this.dataReceive;
    if (!this.dataReceive.formula_type) {
      this.selected_AudienceFilter_Picker.formula_type = this.AudienceFilter_Picker.formula_types[0]
    }
    if (this.dataReceive.formula_value) {
      this.formGroup.patchValue({ day: this.selected_AudienceFilter_Picker.formula_value });
    }
  }

  convertFormulaType(item) {
    switch (item) {
      case 'before':
        return 'trước';
      case 'trước':
        return 'before';
    }
  }

  selectFormulaType(item) {
    this.selected_AudienceFilter_Picker.formula_type = item;
  }

  saveFormulaValue() {
    this.selected_AudienceFilter_Picker.formula_value = this.formGroup.get('day').value;
  }
}
