import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import * as _ from 'lodash';
import { ProductCategoryBasic, ProductCategoryPaged, ProductCategoryService } from 'src/app/product-categories/product-category.service';
import { ProductSimple } from 'src/app/products/product-simple';
import { ProductFilter, ProductService } from 'src/app/products/product.service';
import { CommissionProductRuleDisplay } from '../commission.service';

@Component({
  selector: 'app-commission-create-update-dialog',
  templateUrl: './commission-create-update-dialog.component.html',
  styleUrls: ['./commission-create-update-dialog.component.css']
})
export class CommissionCreateUpdateDialogComponent implements OnInit {
  formGroup: FormGroup;
  line: CommissionProductRuleDisplay;
  filteredProductCategories: ProductCategoryBasic[];
  categCbxLoading = false;
  filteredProducts: ProductSimple[];
  productCbxLoading = false;
  public min: number = 0;
  public max: number = 100;
  isProduct:boolean = false;
  isCateg:boolean = false;
  @ViewChild('categCbx', { static: true }) categCbx: ComboBoxComponent;
  @ViewChild('productCbx', { static: true }) productCbx: ComboBoxComponent;
  
  get f() {
    return this.formGroup.controls;
  }
  submitted: boolean = false;
  constructor(private fb: FormBuilder, 
    private productCategoryService: ProductCategoryService, 
    private productService: ProductService, 
    public activeModal: NgbActiveModal) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      appliedOn: "3_global",
      productId: null,
      product: [null],
      categId: null,
      categ: [null],
      percentFixed: [0, Validators.required]
    });

    if (this.line)
      this.formGroup.patchValue(this.line);

    // this.categCbx.filterChange.asObservable().pipe(
    //   debounceTime(300),
    //   tap(() => (this.categCbx.loading = true)),
    //   switchMap(value => this.searchProductCategories(value))
    // ).subscribe(result => {
    //   this.filteredProductCategories = result;
    //   this.categCbx.loading = false;
    // });

    // this.productCbx.filterChange.asObservable().pipe(
    //   debounceTime(300),
    //   tap(() => (this.productCbx.loading = true)),
    //   switchMap(value => this.searchProducts(value))
    // ).subscribe(result => {
    //   this.filteredProducts = result;
    //   this.productCbx.loading = false;
    // });

    setTimeout(() => {
      this.loadProductCategories();
      this.loadProducts();
    });
  }

  getValueForm(key) {
    return this.formGroup.get(key).value;
  }

  changeAppliedOn(e) {
    switch (e.target.value) {
      case "3_global":
        this.formGroup.get('product').clearValidators();
        this.formGroup.get('product').updateValueAndValidity();
        this.formGroup.get('categ').clearValidators();
        this.formGroup.get('categ').updateValueAndValidity();
        this.formGroup.get('percentFixed').setValue(0);
        break;
      case "2_product_category":
        this.categCbx.focus();
        this.formGroup.get('product').clearValidators();
        this.formGroup.get('product').updateValueAndValidity();
        this.formGroup.get('categ').setValidators([Validators.required]);
        this.formGroup.get('categ').updateValueAndValidity();
        this.formGroup.get('percentFixed').setValue(0);
        
        break;
      case "0_product_variant":
        this.productCbx.focus();
        this.formGroup.get('categ').clearValidators();
        this.formGroup.get('categ').updateValueAndValidity();
        this.formGroup.get('product').setValidators([Validators.required]);
        this.formGroup.get('product').updateValueAndValidity();
        this.formGroup.get('percentFixed').setValue(0);
        break;
      default:
        break;
    }
    
    
  }

  loadProductCategories() {
    this.searchProductCategories('').subscribe(result => {
      this.filteredProductCategories = _.unionBy(this.filteredProductCategories, result, 'id');
    });
  }

  searchProductCategories(q?: string) {
    var val = new ProductCategoryPaged();
    val.search = q || '';
    val.type = 'service';
    return this.productCategoryService.autocomplete(val);
  }

  loadProducts() {
    this.searchProducts().subscribe(result => {
      this.filteredProducts = _.unionBy(this.filteredProducts, result, 'id');
    });
  }

  searchProducts(search?: string) {
    var val = new ProductFilter();
    val.saleOK = true;
    val.search = search;
    return this.productService.autocomplete2(val);
  }

  onSave() {
    this.submitted = true;
    if (!this.formGroup.valid) {
      return;
    }

    var val = this.formGroup.value;
    val.categId = val.categ ? val.categ.id : null;
    val.productId = val.product ? val.product.id : null;
    this.activeModal.close(val);
    this.submitted = false;
  }
}
