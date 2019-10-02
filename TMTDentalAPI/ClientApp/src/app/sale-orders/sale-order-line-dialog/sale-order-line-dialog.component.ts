import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { UserSimple } from 'src/app/users/user-simple';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { ProductFilter, ProductService } from 'src/app/products/product.service';
import { UserService } from 'src/app/users/user.service';
import { WindowRef } from '@progress/kendo-angular-dialog';
import { ProductSimple } from 'src/app/products/product-simple';

@Component({
  selector: 'app-sale-order-line-dialog',
  templateUrl: './sale-order-line-dialog.component.html',
  styleUrls: ['./sale-order-line-dialog.component.css']
})
export class SaleOrderLineDialogComponent implements OnInit {
  saleLineForm: FormGroup;
  filteredProducts: ProductSimple[];
  filteredUsers: UserSimple[];
  @ViewChild('productCbx', { static: true }) productCbx: ComboBoxComponent;
  @ViewChild('userCbx', { static: true }) userCbx: ComboBoxComponent;

  constructor(private fb: FormBuilder, private productService: ProductService,
    private userService: UserService, public window: WindowRef) { }

  ngOnInit() {
    this.saleLineForm = this.fb.group({
      product: null,
      priceUnit: 1,
      productUOMQty: 1,
      discount: 0,
      salesman: null,
    });

    this.productCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.productCbx.loading = true)),
      switchMap(value => this.searchProducts(value))
    ).subscribe(result => {
      this.filteredProducts = result;
      this.productCbx.loading = false;
    });

    this.userCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.userCbx.loading = true)),
      switchMap(value => this.searchUsers(value))
    ).subscribe(result => {
      this.filteredUsers = result;
      this.userCbx.loading = false;
    });
  }

  searchUsers(filter: string) {
    return this.userService.autocomplete(filter);
  }

  searchProducts(search: string) {
    return this.productService.autocomplete(search);
  }

  onSave() {
    console.log(this.saleLineForm);
    if (!this.saleLineForm.valid) {
      return;
    }

    this.window.close(this.saleLineForm.value);
  }

  onCancel() {
    this.window.close();
  }
}
