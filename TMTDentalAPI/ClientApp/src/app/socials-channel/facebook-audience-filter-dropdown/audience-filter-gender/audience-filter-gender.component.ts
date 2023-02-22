import { Component, OnInit, Input } from '@angular/core';
import { AudienceFilterItem } from '../../facebook-mass-messaging.service';

@Component({
  selector: 'app-audience-filter-gender',
  templateUrl: './audience-filter-gender.component.html',
  styleUrls: ['./audience-filter-gender.component.css']
})
export class AudienceFilterGenderComponent implements OnInit {

  @Input() dataReceive: any;

  AudienceFilter_Picker = {
    formula_types: ['eq', 'neq'],
    formula_values: ['male', 'female'],
    formula_displays: ['Nam', 'Nữ'],
  }

  selected_AudienceFilter_Picker: AudienceFilterItem;

  constructor() { }

  ngOnInit() {
    this.selected_AudienceFilter_Picker = this.dataReceive;
    if (!this.dataReceive.formula_type) {
      this.selected_AudienceFilter_Picker.formula_type = this.AudienceFilter_Picker.formula_types[0]
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
  
  selectFormulaValue(item, i) {
    this.selected_AudienceFilter_Picker.formula_value = item;
    this.selected_AudienceFilter_Picker.formula_display = this.AudienceFilter_Picker.formula_displays[i];
  }


}
