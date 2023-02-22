import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ProductService, ProductFilter } from 'src/app/products/product.service';
import { ProductCategoryService, ProductCategoryBasic, ProductCategoryPaged } from 'src/app/product-categories/product-category.service';
import { Product } from 'src/app/products/product';
import { MultiSelectComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { ProductSimple } from 'src/app/products/product-simple';

@Component({
  selector: 'app-promotion-program-rule-cu-dialog',
  templateUrl: './promotion-program-rule-cu-dialog.component.html',
  styleUrls: ['./promotion-program-rule-cu-dialog.component.css']
})
export class PromotionProgramRuleCuDialogComponent implements OnInit {
  formGroup: FormGroup;
  title: string;
  listCategories: ProductCategoryBasic[];
  listProducts: ProductSimple[];
  @ViewChild('productMultiSelect', { static: true }) productMultiSelect: MultiSelectComponent;

  constructor(private fb: FormBuilder, public activeModal: NgbActiveModal, private productService: ProductService,
    private productCategoryService: ProductCategoryService) { }

  ngOnInit() {
    if (!this.formGroup) {
      this.formGroup = this.fb.group({
        minQuantity: 1,
        discountType: 'percentage',
        discountPercentage: 0,
        discountFixedAmount: 0,
        discountApplyOn: '3_global',
        categories: [[]],
        products: [[]],
      });
    }

    setTimeout(() => {
      this.loadListCategories();
      this.loadListProducts();

      this.productMultiSelect.filterChange.asObservable().pipe(
        debounceTime(300),
        tap(() => (this.productMultiSelect.loading = true)),
        switchMap(value => this.searchProducts(value))
      ).subscribe(result => {
        this.listProducts = result;
        this.productMultiSelect.loading = false;
      });
    });
  }

  get discountType() {
    return this.formGroup.get('discountType').value;
  }

  get discountApplyOn() {
    return this.formGroup.get('discountApplyOn').value;
  }

  loadListCategories() {
    this.searchCategories().subscribe(result => {
      this.listCategories = result;
    });
  }

  loadListProducts() {
    this.searchProducts().subscribe(result => {
      this.listProducts = result;
    });
  }

  searchCategories(q?: string) {
    var val = new ProductCategoryPaged();
    val.search = q || '';
    val.type = 'service';
    return this.productCategoryService.autocomplete(val);
  }

  searchProducts(search?: string) {
    var val = new ProductFilter();
    val.saleOK = true;
    val.type = 'service';
    val.search = search || '';
    return this.productService.autocomplete2(val);
  }

  onSave() {
    if (!this.formGroup.valid) {
      return false;
    }

    this.activeModal.close(this.formGroup);
  }
}
