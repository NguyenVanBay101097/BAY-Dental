import { Component, Input, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-product-step-form',
  templateUrl: './product-step-form.component.html',
  styleUrls: ['./product-step-form.component.css']
})
export class ProductStepFormComponent implements OnInit {
  @Input() formGroup: FormGroup;
  constructor() { }

  ngOnInit() {

  }
}


