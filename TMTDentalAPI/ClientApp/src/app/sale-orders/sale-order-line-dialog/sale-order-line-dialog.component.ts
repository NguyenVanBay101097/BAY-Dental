
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap, map } from 'rxjs/operators';
import { ProductFilter, ProductService } from 'src/app/products/product.service';
import { ProductSimple } from 'src/app/products/product-simple';
import * as _ from 'lodash';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { SaleOrderLineDisplay } from '../sale-order-line-display';
import { SaleOrderLineService } from '../sale-order-line.service';

@Component({
  selector: 'app-sale-order-line-dialog',
  templateUrl: './sale-order-line-dialog.component.html',
  styleUrls: ['./sale-order-line-dialog.component.css']
})
export class SaleOrderLineDialogComponent implements OnInit {
  saleLineForm: FormGroup;
  filteredProducts: ProductSimple[];
  line: SaleOrderLineDisplay;
  @ViewChild('productCbx', { static: true }) productCbx: ComboBoxComponent;
  title: string;

  constructor(private fb: FormBuilder, private productService: ProductService,
    public activeModal: NgbActiveModal, private saleLineService: SaleOrderLineService) { }

  ngOnInit() {
    this.saleLineForm = this.fb.group({
      name: '',
      product: [null, Validators.required],
      productId: null,
      priceUnit: 0,
      productUOMQty: 1,
      discount: 0,
      priceSubTotal: 1,
    });

    setTimeout(() => {
      this.productCbx.focus();
    }, 200);

    if (this.line) {
      if (this.line.product) {
        this.filteredProducts = _.unionBy(this.filteredProducts, [this.line.product], 'id');
      }

      this.saleLineForm.patchValue(this.line);
    }

    this.productCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.productCbx.loading = true)),
      switchMap(value => this.searchProducts(value))
    ).subscribe(result => {
      this.filteredProducts = result;
      this.productCbx.loading = false;
    });

    this.loadFilteredProducts();
  }

  loadFilteredProducts() {
    this.searchProducts().subscribe(result => this.filteredProducts = result);
  }

  searchProducts(search?: string) {
    var val = new ProductFilter();
    val.saleOK = true;
    val.type = 'service';
    val.search = search;
    return this.productService.autocomplete2(val);
  }

  getPriceSubTotal() {
    return (this.getPriceUnit() * (1 - this.getDiscount() / 100)) * this.getQuantity();
  }

  getPriceUnit() {
    return this.saleLineForm.get('priceUnit').value;
  }

  getQuantity() {
    return this.saleLineForm.get('productUOMQty').value;
  }

  getDiscount() {
    return this.saleLineForm.get('discount').value;
  }

  onChangeProduct(value: any) {
    var val = this.saleLineForm.value;
    this.saleLineService.onChangeProduct(val).subscribe(result => {
      this.saleLineForm.patchValue(result);
    });
  }

  onSave() {
    if (!this.saleLineForm.valid) {
      return;
    }

    var val = this.saleLineForm.value;
    val.productId = val.product.id;
    val.priceSubTotal = this.getPriceSubTotal();
    this.activeModal.close(val);
  }

  onCancel() {
    this.activeModal.dismiss();
  }
}

