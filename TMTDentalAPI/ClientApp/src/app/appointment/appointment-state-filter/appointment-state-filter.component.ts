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
    { text: 'Chờ khám', value: 'waiting', class: 'text-warning' },
    { text: 'Đang khám', value: 'examination', class: 'text-info' },
    { text: 'Hoàn thành', value: 'done', class: 'text-success' },
    { text: 'Hủy hẹn', value: 'cancel', class: 'text-dark' },
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
      case "waiting":
        return 'Chờ khám';
      case "examination":
        return 'Đang khám';
      case "done":
        return 'Hoàn thành';
      case "cancel":
        return 'Hủy hẹn';
      default:
        return 'Tất cả trạng thái';
    }
  }
}



