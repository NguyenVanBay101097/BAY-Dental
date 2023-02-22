import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ProductSimple } from 'src/app/products/product-simple';
import { StockMoveDisplay } from '../stock-picking.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { ProductService, ProductFilter } from 'src/app/products/product.service';
import { WindowRef } from '@progress/kendo-angular-dialog';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { StockMoveService, StockMoveOnChangeProduct } from 'src/app/stock-moves/stock-move.service';

@Component({
  selector: 'app-stock-picking-ml-dialog',
  templateUrl: './stock-picking-ml-dialog.component.html',
  styleUrls: ['./stock-picking-ml-dialog.component.css']
})
export class StockPickingMlDialogComponent implements OnInit {
  lineForm: FormGroup;
  filteredProducts: ProductSimple[] = [];
  line: StockMoveDisplay;
  @ViewChild('productCbx', { static: true }) productCbx: ComboBoxComponent;

  constructor(private fb: FormBuilder, private productService: ProductService, public window: WindowRef,
    private stockMoveService: StockMoveService) { }

  ngOnInit() {
    this.lineForm = this.fb.group({
      name: [null, Validators.required],
      product: [null, Validators.required],
      productUOMQty: 1,
    });

    setTimeout(() => {
      this.productCbx.focus();
    }, 200);

    if (this.line) {
      if (this.line.product) {
        this.filteredProducts.push(this.line.product);
      }
      this.lineForm.patchValue(this.line);
    } else {
    }

    this.productCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.productCbx.loading = true)),
      switchMap(value => this.searchProducts(value))
    ).subscribe(result => {
      this.filteredProducts = result;
      this.productCbx.loading = false;
    });
  }

  searchProducts(search?: string) {
    var val = new ProductFilter();
    val.search = search;
    val.type = 'product';
    return this.productService.autocomplete2(val);
  }

  onChangeProduct(value: any) {
    if (value) {
      var val = new StockMoveOnChangeProduct();
      val.productId = value.id;
      this.stockMoveService.onChangeProduct(val).subscribe(result => {
        this.lineForm.patchValue({ ...result });
      });
    }
  }

  onSave() {
    if (!this.lineForm.valid) {
      return;
    }

    var val = this.lineForm.value;
    val.productId = val.product.id;
    this.window.close(val);
  }

  onCancel() {
    this.window.close();
  }
}
