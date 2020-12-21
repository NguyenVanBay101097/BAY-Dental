import { Component, OnInit, ViewChild } from '@angular/core';
import { ProductService } from '../product.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { ProductCategoryBasic, ProductCategoryPaged, ProductCategoryService } from 'src/app/product-categories/product-category.service';
import * as _ from 'lodash';
import { UomService } from 'src/app/uoms/uom.service';

@Component({
  selector: 'app-product-labo-cu-dialog',
  templateUrl: './product-labo-cu-dialog.component.html',
  styleUrls: ['./product-labo-cu-dialog.component.css']
})
export class ProductLaboCuDialogComponent implements OnInit {
  formGroup: FormGroup;
  id: string;
  title: string;
  filterdCategories: ProductCategoryBasic[] = [];
  filterdUoMs = [];
  filterdUoMPOs = [];

  constructor(private productService: ProductService, public activeModal: NgbActiveModal,
    private fb: FormBuilder) {
  }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      saleOK: false,
      purchaseOK: false,
      categ: [null],
      uom: null,
      uompo: null,
      type: 'consu',
      type2: 'labo',
      listPrice: 1,
      standardPrice: 0,
      companyId: null,
      defaultCode: '',
      keToaNote: null,
      keToaOK: false,
      isLabo: false,
      purchasePrice: 0,
      laboPrice:0
    });

    this.default();
  }

  get form() {return this.formGroup;}
  get nameC() {return this.formGroup.get('name');}

  default() {
    if (this.id) {
      this.productService.get(this.id).subscribe((result: any) => {
        this.formGroup.patchValue(result);
      });
    } else {
      this.productService.defaultGet().subscribe((result: any) => {
        this.formGroup.patchValue(result);
        this.formGroup.get('type').setValue('consu');
        this.formGroup.get('type2').setValue('labo');
        this.formGroup.get('saleOK').setValue(false);
        this.formGroup.get('purchaseOK').setValue(false);
        this.formGroup.get('keToaOK').setValue(false);
      });
    } }

  onSave() {
    if (!this.formGroup.valid) {
      return;
    }
    var val = this.formGroup.value;
    val.categId = val.categ? val.categ.id: null;
    val.uoMIds = [];
    val.uomId = val.uom? val.uom.id: null;
    val.uompoId = val.uompo? val.uompo.id: null;
    val.uoMIds.push(val.uompo.id);
    val.uoMIds.push(val.uom.id);
    if (this.id) {
      this.productService.update(this.id, val).subscribe(() => {
        this.activeModal.close(true);
      });
    } else {
      return this.productService.create(val).subscribe(result => {
        this.activeModal.close(result);
      });;
    }
  }
}

