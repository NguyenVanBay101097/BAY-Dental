import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { TmtOptionSelect } from 'src/app/core/tmt-option-select';

@Component({
  selector: 'app-tmt-option-select-dropdown',
  templateUrl: './tmt-option-select-dropdown.component.html',
  styleUrls: ['./tmt-option-select-dropdown.component.css']
})
export class TmtOptionSelectDropdownComponent implements OnInit {
  @Input() options: TmtOptionSelect[];
  optionSelected: TmtOptionSelect;
  @Input() title: string;
  @Output() selectChange = new EventEmitter<any>();
  constructor() { }

  ngOnInit() {
  }

  selectOption(option) {
    this.optionSelected = option;
    this.selectChange.emit(option);
  }
}
