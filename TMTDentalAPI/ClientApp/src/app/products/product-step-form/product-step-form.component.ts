import { Component, OnInit, ViewChild, Input, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, Validators, NgForm } from '@angular/forms';
import { ProductCategoryBasic, ProductCategoryService, ProductCategoryPaged } from 'src/app/product-categories/product-category.service';

@Component({
  selector: 'app-product-step-form',
  templateUrl: './product-step-form.component.html',
  styleUrls: ['./product-step-form.component.css']
})
export class ProductStepFormComponent implements OnInit {
  @Input() formGroup: FormGroup;
  constructor(private fb: FormBuilder, private productCategoryService: ProductCategoryService) { }

  ngOnInit() {

  }
}


