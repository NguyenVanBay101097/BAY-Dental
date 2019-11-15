import { Component, OnInit, Output, EventEmitter, ViewChild, Input } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';

@Component({
  selector: 'app-appointment-state-filter',
  templateUrl: './appointment-state-filter.component.html',
  styleUrls: ['./appointment-state-filter.component.css'],
  host: {
    class: "o_advance_search_dropdown"
  }
})
export class AppointmentStateFilterComponent implements OnInit {
  stateSelected: string;
  states: { text: string, value: string, class: string }[] = [
    { text: 'Tất cả trạng thái', value: '', class: '' },
    { text: 'Đang hẹn', value: 'confirmed', class: 'text-primary' },
    { text: 'Đã tới', value: 'done', class: 'text-success' },
    { text: 'Đã hủy', value: 'cancel', class: 'text-dark' },
  ];

  @Output() searchChange = new EventEmitter<any>();

  constructor() { }

  ngOnInit() {
  }

  onSelect(state) {
    this.stateSelected = state;
    this.searchChange.emit(state);
  }

  getResult() {
    switch (this.stateSelected) {
      case "confirmed":
        return 'Đang hẹn';
      case "done":
        return 'Đã tới';
      case "cancel":
        return 'Đã hủy';
      default:
        return 'Tất cả trạng thái';
    }
  }
}



