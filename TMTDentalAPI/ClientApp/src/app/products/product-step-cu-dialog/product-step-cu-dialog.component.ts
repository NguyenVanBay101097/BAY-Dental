import { Component, OnInit, ViewChild } from '@angular/core';
import { Product } from '../product';
import { ProductService } from '../product.service';
import { WindowRef, WindowService, WindowCloseResult } from '@progress/kendo-angular-dialog';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { debug } from 'util';
import { ProductMedicineFormComponent } from '../product-medicine-form/product-medicine-form.component';
import { ProductCategoryDisplay, ProductCategoryBasic, ProductCategoryPaged, ProductCategoryService } from 'src/app/product-categories/product-category.service';
import * as _ from 'lodash';
import { ProductStepFormComponent } from '../product-step-form/product-step-form.component';

@Component({
  selector: 'app-product-step-cu-dialog',
  templateUrl: './product-step-cu-dialog.component.html',
  styleUrls: ['./product-step-cu-dialog.component.css']
})
export class ProductStepCuDialogComponent implements OnInit {
  opened = false;
  formGroup: FormGroup;
  id: string;
  @ViewChild('productForm', { static: true }) productForm: ProductStepFormComponent;

  constructor(private productService: ProductService, public window: WindowRef, private fb: FormBuilder,
    private windowService: WindowService, private productCategoryService: ProductCategoryService) {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      saleOK: false,
      purchaseOK: false,
      categId: null,
      uomId: null,
      uompoId: null,
      type: 'consu',
      listPrice: 1,
      standardPrice: 0,
      companyId: null,
      defaultCode: '',
      keToaOK: false,
      description: null
    });
  }

  ngOnInit() {
    if (this.id) {
      this.productService.get(this.id).subscribe(result => {
        this.formGroup.patchValue(result);
      });
    } else {
      this.productService.defaultProductStepGet().subscribe(result => {
        this.formGroup.patchValue(result);
        //cập nhật 1 số trường cho phù hợp khi tạo 1 dịch vụ
        this.formGroup.get('purchaseOK').patchValue(false);
        this.formGroup.get('saleOK').patchValue(false);
        this.formGroup.get('keToaOK').patchValue(false);
        this.formGroup.get('type').patchValue('service');
      });
    }
  }

  onSave() {
    if (!this.formGroup.valid) {
      return;
    }

    var val = this.formGroup.value;
    if (this.id) {
      this.productService.update(this.id, val).subscribe(() => {
        this.window.close(true);
      });
    } else {
      return this.productService.create(val).subscribe(result => {
        this.window.close(result);
      });;
    }
  }

  onCancel() {
    this.window.close();
  }
}



