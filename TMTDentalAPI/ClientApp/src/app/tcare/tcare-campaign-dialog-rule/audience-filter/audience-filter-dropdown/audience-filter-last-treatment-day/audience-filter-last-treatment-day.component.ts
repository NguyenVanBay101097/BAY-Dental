import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-audience-filter-last-treatment-day',
  templateUrl: './audience-filter-last-treatment-day.component.html',
  styleUrls: ['./audience-filter-last-treatment-day.component.css']
})
export class AudienceFilterLastTreatmentDayComponent implements OnInit {

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
      var day = parseInt(this.data.day) || 0;
      this.formGroup.get('day').setValue(day);
    }
  }

  onSave() {
    if (!this.formGroup.valid) {
      return false;
    }

    var day = this.formGroup.get('day').value;
    var res = {
      type: this.type,
      name: this.name + " sau " + day + " ngày",
      day: day
    };

    this.saveClick.emit(res);
  }
}
