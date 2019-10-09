import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';

@Component({
  selector: 'app-account-invoice-advance-search',
  templateUrl: './account-invoice-advance-search.component.html',
  styleUrls: ['./account-invoice-advance-search.component.css']
})
export class AccountInvoiceAdvanceSearchComponent implements OnInit {
  formGroup: FormGroup;
  @Output() searchChange = new EventEmitter<any>();
  constructor(private fb: FormBuilder) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      dateOrderFrom: null,
      dateOrderTo: null
    });
  }

  onSearch() {
    console.log(this.formGroup.value);
    this.searchChange.emit(this.formGroup.value);
  }

}
