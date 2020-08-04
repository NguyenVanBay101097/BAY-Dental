import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-commission-create-update',
  templateUrl: './commission-create-update.component.html',
  styleUrls: ['./commission-create-update.component.css']
})
export class CommissionCreateUpdateComponent implements OnInit {
  formGroup: FormGroup;
  id: string;

  constructor(private fb: FormBuilder) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
    });


  }

  getValueForm(key) {
    return this.formGroup.get(key).value;
  }
}
