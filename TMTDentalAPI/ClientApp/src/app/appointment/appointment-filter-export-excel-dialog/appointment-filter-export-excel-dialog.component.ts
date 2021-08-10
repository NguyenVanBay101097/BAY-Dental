import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-appointment-filter-export-excel-dialog',
  templateUrl: './appointment-filter-export-excel-dialog.component.html',
  styleUrls: ['./appointment-filter-export-excel-dialog.component.css']
})
export class AppointmentFilterExportExcelDialogComponent implements OnInit {
  title: string;
  dateFrom: Date;
  dateTo: Date;
  state: string = 'default';
  states: { text: string, value: string }[] = [
    { text: 'Mặc định', value: 'default' },
    { text: 'Theo khoảng thời gian', value: 'period' }
  ];
  stateSelected: string = this.states[0].value;

  constructor(
    public activeModal: NgbActiveModal
  ) { }

  ngOnInit() { }

  onConfirm() {
    var val = {
      dateFrom: this.dateFrom,
      dateTo: this.dateTo,
      state: this.state
    }
    this.activeModal.close(val);
  }

  onChangeState(event) {
    this.state = event.target.value;
  }

  onChangeDate(event) {
    this.dateFrom = event.dateFrom;
    this.dateTo = event.dateTo;
  }

  onCancel() {
    this.activeModal.dismiss();
  }
}
