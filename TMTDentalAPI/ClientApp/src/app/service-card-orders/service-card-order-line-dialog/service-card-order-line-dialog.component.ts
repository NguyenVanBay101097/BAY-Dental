import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap, map } from 'rxjs/operators';
import { ProductFilter, ProductService } from 'src/app/products/product.service';
import { ProductSimple } from 'src/app/products/product-simple';
import * as _ from 'lodash';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ToothCategoryBasic, ToothCategoryService } from 'src/app/tooth-categories/tooth-category.service';
import { ToothDisplay, ToothFilter, ToothService } from 'src/app/teeth/tooth.service';
import { ServiceCardTypePaged } from 'src/app/service-card-types/service-card-type-paged';
import { ServiceCardTypeService } from 'src/app/service-card-types/service-card-type.service';
import { ServiceCardOrderLineService } from '../service-card-order-line.service';

@Component({
  selector: 'app-service-card-order-line-dialog',
  templateUrl: './service-card-order-line-dialog.component.html',
  styleUrls: ['./service-card-order-line-dialog.component.css']
})

export class ServiceCardOrderLineDialogComponent implements OnInit {
  formGroup: FormGroup;
  filteredCardTypes: any[];
  line: any;
  @ViewChild('cardTypeCbx', { static: true }) cardTypeCbx: ComboBoxComponent;
  title: string;

  constructor(private fb: FormBuilder, private cardTypeService: ServiceCardTypeService,
    public activeModal: NgbActiveModal, private saleLineService: ServiceCardOrderLineService) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      cardType: [null, Validators.required],
      priceUnit: 0,
      productUOMQty: 1,
      discount: 0,
      discountType: 'percentage',
      discountFixed: 0,
    });

    setTimeout(() => {
      this.cardTypeCbx.focus();
    }, 200);

    if (this.line) {
      setTimeout(() => {
        if (this.line.cardType) {
          this.filteredCardTypes = _.unionBy(this.filteredCardTypes, [this.line.cardType], 'id');
        }

        this.formGroup.patchValue(this.line);
      });
    }

    this.cardTypeCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.cardTypeCbx.loading = true)),
      switchMap(value => this.searchCardTypes(value))
    ).subscribe((result: any) => {
      this.filteredCardTypes = result.items;
      this.cardTypeCbx.loading = false;
    });

    setTimeout(() => {
      this.loadFilteredCardTypes();
    });
  }

  get productUpdatable() {
    if (!this.line) {
      return true;
    }

    var updatable = (this.line.state == "done" || this.line.state == "cancel") || (this.line.state == "sale" && this.line.qtyInvoiced > 0);
    return !updatable;
  }

  get lineState() {
    return this.line ? this.line.state : 'draft';
  }

  get discountTypeValue() {
    return this.formGroup.get('discountType').value;
  }

  loadFilteredCardTypes() {
    this.searchCardTypes().subscribe((result: any) => {
      this.filteredCardTypes = _.unionBy(this.filteredCardTypes, result.items, 'id');
    });
  }

  onChangeDiscountFixed(value) {
    var price = this.getPriceUnit();
    if (value > price) {
      this.formGroup.get('discountFixed').setValue(price);
    }
  }

  searchCardTypes(search?: string) {
    var val = new ServiceCardTypePaged();
    val.search = search || '';
    return this.cardTypeService.getPaged(val);
  }

  getPriceSubTotal() {
    var discountType = this.discountTypeValue;
    var price = discountType == 'percentage' ? this.getPriceUnit() * (1 - this.getDiscount() / 100) :
      Math.max(0, this.getPriceUnit() - this.discountFixedValue);
    var subtotal = price * this.getQuantity();
    return subtotal;
  }

  getPriceUnit() {
    return this.formGroup.get('priceUnit').value;
  }

  getQuantity() {
    return this.formGroup.get('productUOMQty').value;
  }

  getDiscount() {
    return this.formGroup.get('discount').value;
  }

  get discountFixedValue() {
    return this.formGroup.get('discountFixed').value;
  }

  onChangeCardType(value: any) {
    if (value) {
      this.saleLineService.onChangeProduct({ cardTypeId: value.id }).subscribe((result: any) => {
        this.formGroup.patchValue(result);
      });
    }
  }


  onSave() {
    if (!this.formGroup.valid) {
      return;
    }

    var val = this.formGroup.value;
    val.cardTypeId = val.cardType.id;
    val.priceSubTotal = this.getPriceSubTotal();
    this.activeModal.close(val);
  }

  onCancel() {
    this.activeModal.dismiss();
  }
}

