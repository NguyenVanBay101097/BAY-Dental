import { Component, OnInit, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { debounceTime, distinctUntilChanged, tap, switchMap } from 'rxjs/operators';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ProductService, ProductPaged, ProductFilter } from 'src/app/products/product.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import * as _ from 'lodash';

@Component({
  selector: 'app-audience-filter-service',
  templateUrl: './audience-filter-service.component.html',
  styleUrls: ['./audience-filter-service.component.css']
})
export class AudienceFilterServiceComponent implements OnInit {

  formGroup: FormGroup;
  filteredProducts = [];
  submitted = false;
  type: string;
  name: string;
  @Output() saveClick = new EventEmitter<any>();
  @ViewChild('productCbx', { static: true }) productCbx: ComboBoxComponent;
  data: any;

  constructor(private fb: FormBuilder, private productService: ProductService) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      op: 'contains',
      product: [null, Validators.required]
    });

    setTimeout(() => {
      if (this.data) {
        var pd = this.data.value;

        this.formGroup.patchValue({
          op: this.data.op,
          product: pd
        });

        this.filteredProducts.push(pd);
      }

      this.loadProductList();

      this.productCbx.filterChange.asObservable().pipe(
        debounceTime(300),
        tap(() => (this.productCbx.loading = true)),
        switchMap(value => this.searchProducts(value))
      ).subscribe(result => {
        this.filteredProducts = result;
        this.productCbx.loading = false;
      });
    });
  }

  get productControl() {
    return this.formGroup.get('product');
  }

  productClick(product) {
    this.formGroup.get('product').setValue(product);
  }

  get productSelected() {
    return this.formGroup.get('product').value;
  }

  searchProducts(search?: string) {
    var val = new ProductFilter();
    val.saleOK = true;
    val.limit = 5;
    val.search = search || '';
    return this.productService.autocomplete2(val);
  }

  loadProductList(q?: string) {
    this.searchProducts(q).subscribe(result => {
      this.filteredProducts = _.unionBy(this.filteredProducts, result, 'id');
    });
  }

  getOpDisplay(op) {
    if (op == 'contains') {
      return 'Chứa'
    } else if (op == 'not_contains') {
      return 'Không chứa';
    } else {
      return '';
    }
  }

  onSave() {
    this.submitted = true;

    if (!this.formGroup.valid) {
      return false;
    }

    var value = this.formGroup.value;
    var res = {
      type: this.type,
      op: value.op,
      name: this.name + " " + this.getOpDisplay(value.op) + " " + value.product.name + ". ",
      value: { id: value.product.id, name: value.product.name }
    };

    this.saveClick.emit(res);
  }
}
