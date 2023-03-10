import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import * as _ from 'lodash';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { ProductSimple } from 'src/app/products/product-simple';
import { ProductFilter, ProductService } from 'src/app/products/product.service';
import { SamplePrescriptionLineSave } from './../sample-prescriptions.service';

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
        return 'Tr?????c khi ??n';
      case 'in_meal':
        return 'Trong khi ??n';
      case 'after_wakeup':
        return 'Sau khi th???c d???y';
      case 'before_sleep':
        return 'Tr?????c khi ??i ng???';
      default:
        return 'Sau khi ??n';
    }
  }

  computeName() {
    return `Ng??y u???ng ${this.getNumberOfTimes()} l???n, m???i l???n ${this.getAmountOfTimes()} vi??n, u???ng ${this.getUsedAt()}`
  }
  
}
