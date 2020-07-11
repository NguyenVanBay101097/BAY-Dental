import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';

@Component({
  selector: 'app-audience-filter-appointment-day',
  templateUrl: './audience-filter-appointment-day.component.html',
  styleUrls: ['./audience-filter-appointment-day.component.css']
})
export class AudienceFilterAppointmentDayComponent implements OnInit {

  formGroup: FormGroup;
  data: any;
  @Output() saveClick = new EventEmitter<any>();
  type: string;
  name: string;

  constructor(private fb: FormBuilder) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      day: 0
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
