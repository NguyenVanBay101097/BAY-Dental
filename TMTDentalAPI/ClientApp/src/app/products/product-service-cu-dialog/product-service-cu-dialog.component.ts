import { Component, OnInit, ViewChild } from '@angular/core';
import { Product } from '../product';
import { ProductService } from '../product.service';
import { WindowRef } from '@progress/kendo-angular-dialog';
import { ProductServiceFormComponent } from '../product-service-form/product-service-form.component';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { debug } from 'util';

@Component({
  selector: 'app-product-service-cu-dialog',
  templateUrl: './product-service-cu-dialog.component.html',
  styleUrls: ['./product-service-cu-dialog.component.css']
})
export class ProductServiceCuDialogComponent implements OnInit {
  opened = false;
  formGroup: FormGroup;
  id: string;
  @ViewChild('productForm', { static: true }) productForm: ProductServiceFormComponent;

  constructor(private productService: ProductService, public window: WindowRef, private fb: FormBuilder) {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      saleOK: false,
      purchaseOK: false,
      categ: [null, Validators.required],
      uomId: null,
      uompoId: null,
      type: 'consu',
      listPrice: 1,
      standardPrice: 0,
      companyId: null,
      defaultCode: '',
      keToaOK: false,
    });
  }

  ngOnInit() {
    if (this.id) {
      this.productService.get(this.id).subscribe(result => {
        this.formGroup.patchValue(result);
      });
    } else {
      this.productService.defaultGet().subscribe(result => {
        this.formGroup.patchValue(result);
        //cập nhật 1 số trường cho phù hợp khi tạo 1 dịch vụ
        this.formGroup.get('purchaseOK').patchValue(false);
        this.formGroup.get('type').patchValue('service');
      });
    }
  }

  onSave() {
    if (!this.formGroup.valid) {
      return;
    }

    var val = this.formGroup.value;
    val.categId = val.categ.id;
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
