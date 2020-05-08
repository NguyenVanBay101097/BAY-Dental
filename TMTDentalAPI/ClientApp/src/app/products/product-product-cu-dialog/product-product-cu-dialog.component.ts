import { Component, OnInit, Inject, ViewChild, ElementRef, Input } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormBuilder, FormGroup, NgForm, Validators } from '@angular/forms';
import { ProductService } from '../product.service';
import { Product } from '../product';
import { ProductCategoryService, ProductCategoryPaged, ProductCategoryBasic } from 'src/app/product-categories/product-category.service';
import { ProductCategory } from 'src/app/product-categories/product-category';
import { debounceTime, switchMap, tap, map, distinctUntilChanged } from 'rxjs/operators';
import { WindowRef, WindowService, WindowCloseResult } from '@progress/kendo-angular-dialog';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { Observable, Subject } from 'rxjs';
import { ProductCategoryDialogComponent } from 'src/app/product-categories/product-category-dialog/product-category-dialog.component';
import * as _ from 'lodash';
import { ProductStepDisplay } from '../product-step';
import { or } from '@progress/kendo-angular-grid/dist/es2015/utils';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { UoMBasic, UomService, UoMPaged } from 'src/app/uoms/uom.service';

@Component({
  selector: 'app-product-product-cu-dialog',
  templateUrl: './product-product-cu-dialog.component.html',
  styleUrls: ['./product-product-cu-dialog.component.css']
})

export class ProductProductCuDialogComponent implements OnInit {
  title: string;
  id: string;
  productForm: FormGroup;
  filterdCategories: ProductCategoryBasic[] = [];
  filterdUoMs: UoMBasic[] = [];
  opened = false;
  @ViewChild('categCbx', { static: true }) categCbx: ComboBoxComponent;
  @ViewChild('uoMCbx', { static: true }) uoMCbx: ComboBoxComponent;

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private productCategoryService: ProductCategoryService,
    public activeModal: NgbActiveModal,
    private modalService: NgbModal,
    private uoMService: UomService
  ) {
  }

  ngOnInit() {
    this.productForm = this.fb.group({
      name: ['', Validators.required],
      saleOK: false,
      purchaseOK: true,
      categ: [null, Validators.required],
      uomId: null,
      uompoId: null,
      type: 'consu',
      type2: 'product',
      listPrice: 1,
      standardPrice: 0,
      companyId: null,
      defaultCode: '',
      keToaNote: null,
      keToaOK: true,
      isLabo: false,
      purchasePrice: 0,
    });

    setTimeout(() => {
      this.default();

      this.searchCategories('').subscribe(result => {
        this.filterdCategories = _.unionBy(this.filterdCategories, result, 'id');
      });

      this.searchUoMs('').subscribe(result => {
        this.filterdUoMs = _.unionBy(this.filterdUoMs, result, 'id');
      });

      this.categCbxFilterChange();
      // this.uoMCbxFilterChange();
    });
  }

  default() {
    if (this.id) {
      this.productService.get(this.id).subscribe(result => {
        this.filterdCategories = _.unionBy(this.filterdCategories, [result.categ as ProductCategoryBasic], 'id');
        this.productForm.patchValue(result);
      });
    } else {
      this.productService.defaultGet().subscribe(result => {
        if (result.categ) {
          this.filterdCategories = _.unionBy(this.filterdCategories, [result.categ as ProductCategoryBasic], 'id');
        }
        this.productForm.patchValue(result);
        this.productForm.get('type').setValue('consu');
        this.productForm.get('type2').setValue('product');
        this.productForm.get('saleOK').setValue(false);
        this.productForm.get('purchaseOK').setValue(true);
        this.productForm.get('purchasePrice').setValue(0);
      });
    }
  }

  categCbxFilterChange() {
    this.categCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.categCbx.loading = true)),
      switchMap(value => this.searchCategories(value))
    ).subscribe(result => {
      this.filterdCategories = result;
      this.categCbx.loading = false;
    });
  }

  // uoMCbxFilterChange() {
  //   this.uoMCbx.filterChange.asObservable().pipe(
  //     debounceTime(300),
  //     tap(() => (this.categCbx.loading = true)),
  //     switchMap(value => this.searchUoMs(value))
  //   ).subscribe(result => {
  //     this.filterdUoMs = result;
  //     this.categCbx.loading = false;
  //   });
  // }

  searchUoMs(q?: string) {
    var val = new UoMPaged();
    val.search = q || '';
    return this.uoMService.autocomplete(val);
  }

  searchCategories(q?: string) {
    var val = new ProductCategoryPaged();
    val.search = q || '';
    val.type = 'product';
    return this.productCategoryService.autocomplete(val);
  }

  quickCreateCateg() {
    let modalRef = this.modalService.open(ProductCategoryDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm nhóm vật tư';
    modalRef.componentInstance.type = 'product';
    modalRef.result.then(result => {
      this.filterdCategories.push(result as ProductCategoryBasic);
      this.productForm.patchValue({ categ: result });
    }, () => {
    });
  }

  onSave() {
    if (!this.productForm.valid) {
      return;
    }

    this.saveOrUpdate().subscribe(result => {
      if (result) {
        this.activeModal.close(result);
      } else {
        this.activeModal.close(true);
      }
    }, err => {
      console.log(err);
    });
  }

  saveOrUpdate() {
    var data = this.getBodyData();
    if (this.id) {
      return this.productService.update(this.id, data);
    } else {
      return this.productService.create(data);
    }
  }

  getBodyData() {
    var data = this.productForm.value;
    data.categId = data.categ.id;
    return data;
  }

  onCancel() {
    this.activeModal.dismiss();
  }
}







