import { SamplePrescriptionLineSave } from './../sample-prescriptions.service';
import { Component, OnInit, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { ProductSimple } from 'src/app/products/product-simple';
import { FormArray, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ProductService, ProductFilter } from 'src/app/products/product.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import * as _ from 'lodash';

@Component({
  selector: 'app-sample-prescription-line-create-update-dialog',
  templateUrl: './sample-prescription-line-create-update-dialog.component.html',
  styleUrls: ['./sample-prescription-line-create-update-dialog.component.css']
})
export class SamplePrescriptionLineCreateUpdateDialogComponent implements OnInit {
  lineForm: FormGroup;
  filteredProducts: ProductSimple[];
  @Input() line: SamplePrescriptionLineSave;
  @Output() created = new EventEmitter<SamplePrescriptionLineSave>();
  @Output() updated = new EventEmitter<any>();
  
  @ViewChild('productCbx', { static: true }) productCbx: ComboBoxComponent;
  defaultVal: Object;

  constructor(private fb: FormBuilder, private productService: ProductService,) { }

  ngOnInit() {
    this.defaultVal = {
      product: [null, Validators.required],
      numberOfTimes: 1,
      numberOfDays: 1,
      amountOfTimes: 1,
      quantity: 1,
      useAt: 'after_meal',
      note: null,
    };

    this.lineForm = this.fb.group(this.defaultVal);

    setTimeout(() => {
      this.productCbx.focus();
    }, 200);

    this.productCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.productCbx.loading = true)),
      switchMap(value => this.searchProducts(value))
    ).subscribe(result => {
      this.filteredProducts = result;
      this.productCbx.loading = false;
    });

    setTimeout(() => {
      this.loadFilteredProducts();
    });

  }

  ngOnChanges(changes) {
    if (changes.line) {
      var value = changes.line.currentValue;
      if (value) {
        if (value.product) {
          this.filteredProducts = _.unionBy(this.filteredProducts, [value.product], 'id');
        }
        this.lineForm.patchValue(value);
      }
    }
  }

  searchProducts(search?: string) {
    var val = new ProductFilter();
    val.keToaOK = true;
    val.search = search;
    return this.productService.autocomplete2(val);
  }

  loadFilteredProducts() {
    return this.searchProducts().subscribe(result => {
      this.filteredProducts = result;
    });
  }

  
  onSave() {
    if (!this.lineForm.valid) {
      return;
    }

    var val = this.lineForm.value;
    val.productId = val.product.id;
    val.name = this.computeName();
    val.numberOfTimes = val.numberOfTimes;
    var clone = { ...val };
    this.created.emit(clone);
    this.lineForm = this.fb.group(this.defaultVal); //reset
  }

  onUpdate() {
    if (!this.lineForm.valid) {
      return;
    }

    var val = this.lineForm.value;
    val.productId = val.product.id;
    val.name = this.computeName();
    val.
    // var clone = { ...val };
    // this.created.emit(clone);
    this.line = Object.assign(this.line, val);
    this.updated.emit(null);
    this.lineForm = this.fb.group(this.defaultVal); //reset
  }

  onCancel() {
    this.updated.emit(null);
    this.lineForm = this.fb.group(this.defaultVal); //reset
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
