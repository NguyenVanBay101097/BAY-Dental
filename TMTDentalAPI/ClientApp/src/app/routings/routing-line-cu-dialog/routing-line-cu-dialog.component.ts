import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ProductSimple } from 'src/app/products/product-simple';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { ProductService, ProductFilter } from 'src/app/products/product.service';
import { RoutingService, RoutingLineDisplay } from '../routing.service';
import { WindowRef, WindowService, WindowCloseResult } from '@progress/kendo-angular-dialog';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import * as _ from 'lodash';
import { ProductDialogComponent } from 'src/app/products/product-dialog/product-dialog.component';
import { Product } from 'src/app/products/product';
import { ProductStepCuDialogComponent } from 'src/app/products/product-step-cu-dialog/product-step-cu-dialog.component';

@Component({
  selector: 'app-routing-line-cu-dialog',
  templateUrl: './routing-line-cu-dialog.component.html',
  styleUrls: ['./routing-line-cu-dialog.component.css']
})
export class RoutingLineCuDialogComponent implements OnInit {
  routingLineForm: FormGroup;
  item: RoutingLineDisplay;
  filteredProducts: ProductSimple[];
  @ViewChild('productCbx', { static: true }) productCbx: ComboBoxComponent;
  opened = false;

  constructor(private fb: FormBuilder, private windowRef: WindowRef, private productService: ProductService,
    private windowService: WindowService) { }

  ngOnInit() {
    this.routingLineForm = this.fb.group({
      product: [null, Validators.required],
      note: null,
    });

    if (this.item) {
      if (this.item.product) {
        this.filteredProducts = _.unionBy(this.filteredProducts, [this.item.product], 'id');
      }
      this.routingLineForm.patchValue(this.item);
    }

    this.productCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.productCbx.loading = true)),
      switchMap(value => this.searchProducts(value))
    ).subscribe(result => {
      this.filteredProducts = result;
      this.productCbx.loading = false;
    });

    setTimeout(() => {
      this.productCbx.focus();
    }, 200);

    this.loadFilteredProducts();
  }

  loadFilteredProducts() {
    this.searchProducts().subscribe(result => this.filteredProducts = result);
  }

  quickCreateProduct() {
    const windowRef = this.windowService.open({
      title: 'Thêm công đoạn',
      content: ProductStepCuDialogComponent,
      resizable: false,
      autoFocusedElement: '[name="name"]',
    });

    this.opened = true;

    windowRef.result.subscribe((result) => {
      this.opened = false;
      if (!(result instanceof WindowCloseResult)) {
        console.log(result);
        this.filteredProducts = _.unionBy(this.filteredProducts, [result as ProductSimple], 'id');
        this.routingLineForm.patchValue({ product: result });
      }
    });
  }

  searchProducts(search?: string) {
    var val = new ProductFilter();
    val.search = search;
    val.type = 'service';
    val.saleOK = false;
    return this.productService.autocomplete2(val);
  }


  onSave() {
    if (!this.routingLineForm.valid) {
      return;
    }

    var val = this.routingLineForm.value;
    val.productId = val.product ? val.product.id : null;

    this.windowRef.close(val);
  }

  onCancel() {
    this.windowRef.close();
  }
}
