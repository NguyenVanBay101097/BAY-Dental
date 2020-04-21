import { Component, OnInit, Input } from '@angular/core';
import { AudienceFilterItem } from '../../facebook-mass-messaging.service';

@Component({
  selector: 'app-audience-filter-input',
  templateUrl: './audience-filter-input.component.html',
  styleUrls: ['./audience-filter-input.component.css']
})
export class AudienceFilterInputComponent implements OnInit {

  @Input() dataReceive: any;

  AudienceFilter_Picker = {
    formula_types: ['eq', 'neq', 'contains', 'doesnotcontain', 'startswith'],
    formula_values: [],
    formula_displays: null
  }

  selected_AudienceFilter_Picker: AudienceFilterItem;
  inputFormulaValue: string;

  constructor() { }

  ngOnInit() {
    this.selected_AudienceFilter_Picker = this.dataReceive;
    if (!this.dataReceive.formula_type) {
      this.selected_AudienceFilter_Picker.formula_type = this.AudienceFilter_Picker.formula_types[0]
    }
    if (this.dataReceive.formula_value) {
      this.inputFormulaValue = this.selected_AudienceFilter_Picker.formula_value;
    }
  }

  convertFormulaType(item) {
    switch (item) {
      case 'eq':
        return 'bằng';
      case 'neq':
        return 'không bằng';
      case 'contains':
        return 'có chứa';
      case 'doesnotcontain':
        return 'không chứa';
      case 'startswith':
        return 'bắt đầu với';
      case 'bằng':
        return 'eq';
      case 'không bằng':
        return 'neq';
      case 'có chứa':
        return 'contains';
      case 'không chứa':
        return 'doesnotcontain';
      case 'bắt đầu với':
        return 'startswith';
    }
  }

  selectFormulaType(item) {
    this.selected_AudienceFilter_Picker.formula_type = item;
  }

  saveInputFormulaValue() {
    this.selected_AudienceFilter_Picker.formula_value = this.inputFormulaValue;
  }
}
