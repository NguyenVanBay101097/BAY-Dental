import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { UserSimple } from 'src/app/users/user-simple';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap, map } from 'rxjs/operators';
import { ProductFilter, ProductService } from 'src/app/products/product.service';
import { UserService } from 'src/app/users/user.service';
import { WindowRef } from '@progress/kendo-angular-dialog';
import { ProductSimple } from 'src/app/products/product-simple';
import { Observable } from 'rxjs';
import { PartnerService, PartnerFilter } from 'src/app/partners/partner.service';
import { PartnerSimple } from 'src/app/partners/partner-simple';
import { ToothService, ToothFilter, ToothDisplay } from 'src/app/teeth/tooth.service';
import { ToothCategoryService, ToothCategoryBasic } from 'src/app/tooth-categories/tooth-category.service';
import * as _ from 'lodash';
import { ToaThuocLineDisplay } from '../toa-thuoc.service';

@Component({
  selector: 'app-toa-thuoc-line-dialog',
  templateUrl: './toa-thuoc-line-dialog.component.html',
  styleUrls: ['./toa-thuoc-line-dialog.component.css']
})
export class ToaThuocLineDialogComponent implements OnInit {

  lineForm: FormGroup;
  filteredProducts: ProductSimple[];
  line: ToaThuocLineDisplay;
  @ViewChild('productCbx', { static: true }) productCbx: ComboBoxComponent;

  constructor(private fb: FormBuilder, private productService: ProductService,
    private userService: UserService, public window: WindowRef) { }

  ngOnInit() {
    this.lineForm = this.fb.group({
      product: [null, Validators.required],
      numberOfTimes: 1,
      numberOfDays: 1,
      amountOfTimes: 1,
      quantity: 1,
      useAt: 'after_meal',
      note: null,
    });

    setTimeout(() => {
      this.productCbx.focus();
    }, 200);

    if (this.line) {
      if (this.line.product) {
        this.filteredProducts = _.unionBy(this.filteredProducts, [this.line.product], 'id');
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
    val.keToaOK = true;
    val.search = search;
    return this.productService.autocomplete2(val);
  }

  onSave() {
    if (!this.lineForm.valid) {
      return;
    }

    var val = this.lineForm.value;
    val.productId = val.product.id;
    val.name = this.computeName();
    this.window.close(val);
  }

  onCancel() {
    this.window.close();
  }

  public onChange(value: number) {
    setTimeout(() => {
      this.updateQuantity();
    }, 200);
  }

  updateQuantity() {
    var numberOfTimes = this.lineForm.get('numberOfTimes').value || 0;
    var numberOfDays = this.lineForm.get('numberOfDays').value || 0;
    var amountOfTimes = this.lineForm.get('amountOfTimes').value || 0;
    var quantity = numberOfTimes * amountOfTimes * numberOfDays;
    this.lineForm.get('quantity').setValue(quantity);
  }

  getNumberOfTimes() {
    return this.lineForm.get('numberOfTimes').value || 0;
  }

  getAmountOfTimes() {
    return this.lineForm.get('amountOfTimes').value || 0;
  }

  getUsedAt() {
    var useAt = this.lineForm.get('useAt').value;
    switch (useAt) {
      case 'before_meal':
        return 'trước khi ăn';
      case 'in_meal':
        return 'trong khi ăn';
      case 'after_wakeup':
        return 'sau khi thức dậy';
      case 'before_sleep':
        return 'trước khi đi ngủ';
      default:
        return 'sau khi ăn';
    }
  }

  computeName() {
    return `Ngày uống ${this.getNumberOfTimes()} lần, mỗi lần ${this.getAmountOfTimes()} viên, uống ${this.getUsedAt()}`
  }

}
