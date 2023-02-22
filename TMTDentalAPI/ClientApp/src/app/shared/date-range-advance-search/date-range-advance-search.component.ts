import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';

@Component({
  selector: 'app-date-range-advance-search',
  templateUrl: './date-range-advance-search.component.html',
  styleUrls: ['./date-range-advance-search.component.css']
})
export class DateRangeAdvanceSearchComponent implements OnInit {
  @Input() dateFrom: Date;
  @Input() dateTo: Date;
  formGroup: FormGroup;
  constructor(private fb: FormBuilder) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      dateFrom: this.dateFrom,
      dateTo: this.dateTo
    })
  }

}
