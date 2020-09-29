import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';

@Component({
  selector: 'app-audience-filter-last-examination',
  templateUrl: './audience-filter-last-examination.component.html',
  styleUrls: ['./audience-filter-last-examination.component.css']
})
export class AudienceFilterLastExaminationComponent implements OnInit {

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
