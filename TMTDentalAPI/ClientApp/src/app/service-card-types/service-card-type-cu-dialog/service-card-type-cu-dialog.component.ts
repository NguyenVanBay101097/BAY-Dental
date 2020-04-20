import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-service-card-type-cu-dialog',
  templateUrl: './service-card-type-cu-dialog.component.html',
  styleUrls: ['./service-card-type-cu-dialog.component.css']
})
export class ServiceCardTypeCuDialogComponent implements OnInit {
  formGroup: FormGroup;

  constructor(private fb: FormBuilder) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      price: 0,
      amount: 0,
      period: 'month',
      nbrPeriod: 1
    });
  }
}
