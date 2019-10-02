import { Component, OnInit, ViewChild } from '@angular/core';
import { Product } from '../product';
import { ProductService } from '../product.service';
import { WindowRef, WindowService, WindowCloseResult } from '@progress/kendo-angular-dialog';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { debug } from 'util';
import { ProductMedicineFormComponent } from '../product-medicine-form/product-medicine-form.component';
import { ProductCategoryDialogComponent } from 'src/app/product-categories/product-category-dialog/product-category-dialog.component';
import { ProductCategoryDisplay, ProductCategoryBasic, ProductCategoryPaged, ProductCategoryService } from 'src/app/product-categories/product-category.service';
import * as _ from 'lodash';

@Component({
  selector: 'app-product-product-cu-dialog',
  templateUrl: './product-product-cu-dialog.component.html',
  styleUrls: ['./product-product-cu-dialog.component.css']
})
export class ProductProductCuDialogComponent implements OnInit {
  opened = false;
  formGroup: FormGroup;
  filteredCategories: ProductCategoryBasic[] = [];
  id: string;
  @ViewChild('productForm', { static: true }) productForm: ProductMedicineFormComponent;

  constructor(private productService: ProductService, public window: WindowRef, private fb: FormBuilder,
    private windowService: WindowService, private productCategoryService: ProductCategoryService) {
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
      description: null
    });
  }

  ngOnInit() {
    if (this.id) {
      this.productService.get(this.id).subscribe(result => {
        if (result.categ) {
          this.filteredCategories = _.unionBy(this.filteredCategories, [result.categ as ProductCategoryBasic], 'id');
        }

        this.formGroup.patchValue(result);
      });
    } else {
      this.productService.defaultGet().subscribe(result => {
        this.formGroup.patchValue(result);
        //cập nhật 1 số trường cho phù hợp khi tạo 1 dịch vụ
        this.formGroup.get('purchaseOK').patchValue(false);
        this.formGroup.get('saleOK').patchValue(false);
        this.formGroup.get('keToaOK').patchValue(false);
        this.formGroup.get('type').patchValue('product');
      });
    }

    this.loadFilteredCategories();
  }

  onFilterChangeCateg(value) {
    this.searchCategories(value).subscribe(result => this.filteredCategories = result);
  }

  searchCategories(q?: string) {
    var val = new ProductCategoryPaged();
    val.search = q;
    val.productCateg = true;
    val.limit = 20;
    return this.productCategoryService.autocomplete(val);
  }

  loadFilteredCategories() {
    this.searchCategories().subscribe(result => this.filteredCategories = result);
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

  onBtnCreateCategClick() {
    const windowRef = this.windowService.open({
      title: 'Thêm nhóm vật tư',
      content: ProductCategoryDialogComponent,
      resizable: false,
      autoFocusedElement: '[name="name"]',
    });

    const instance = windowRef.content.instance;
    let defaultCateg = new ProductCategoryDisplay();
    defaultCateg.productCateg = true;
    instance.defaultCateg = defaultCateg;

    this.opened = true;

    windowRef.result.subscribe((result) => {
      this.opened = false;
      if (!(result instanceof WindowCloseResult)) {
        this.filteredCategories.push(result as ProductCategoryBasic);
        this.formGroup.patchValue({ categ: result });
      }
    });
  }
}


