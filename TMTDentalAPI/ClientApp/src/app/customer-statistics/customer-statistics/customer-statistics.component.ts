import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-customer-statistics',
  templateUrl: './customer-statistics.component.html',
  styleUrls: ['./customer-statistics.component.css']
})
export class CustomerStatisticsComponent implements OnInit {
  dateFrom: Date;
  dateTo: Date;

  monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());

  formGroup: FormGroup;
  get details() { return this.formGroup.get('details') as FormArray; }

  constructor(private fb: FormBuilder) { }

  ngOnInit() {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;

    this.formGroup = this.fb.group({

      details: this.fb.array([])
    });
  }

  loadDataFromApi() {
   
  }

  onSearchDateChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.loadDataFromApi();
  }
}
