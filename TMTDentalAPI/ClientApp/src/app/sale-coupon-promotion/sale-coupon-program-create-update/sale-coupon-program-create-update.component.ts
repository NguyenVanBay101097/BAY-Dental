import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { NotificationService } from '@progress/kendo-angular-notification';
import { ComboBoxComponent, MultiSelectComponent } from '@progress/kendo-angular-dropdowns';
import { ProductPriceListBasic, ProductPricelistPaged } from 'src/app/price-list/price-list';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { PriceListService } from 'src/app/price-list/price-list.service';
import * as _ from 'lodash';
import { SaleCouponProgramDisplay, SaleCouponProgramService } from '../sale-coupon-program.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { SaleCouponProgramGenerateCouponsDialogComponent } from '../sale-coupon-program-generate-coupons-dialog/sale-coupon-program-generate-coupons-dialog.component';
import { ProductSimple } from 'src/app/products/product-simple';
import { ProductFilter, ProductService } from 'src/app/products/product.service';
import { SaleCouponListDialogComponent } from '../sale-coupon-list-dialog/sale-coupon-list-dialog.component';

@Component({
  selector: 'app-sale-coupon-program-create-update',
  templateUrl: './sale-coupon-program-create-update.component.html',
  styleUrls: ['./sale-coupon-program-create-update.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class SaleCouponProgramCreateUpdateComponent implements OnInit {
  formGroup: FormGroup;
  id: string;
  program: SaleCouponProgramDisplay = new SaleCouponProgramDisplay();
  submitted = false;
  filteredProducts: ProductSimple[];
  @ViewChild('productCbx', { static: true }) productCbx: ComboBoxComponent;

  listProducts: ProductSimple[];
  @ViewChild('productMultiSelect', { static: true }) productMultiSelect: MultiSelectComponent;

  constructor(private fb: FormBuilder, private programService: SaleCouponProgramService,
    private router: Router, private route: ActivatedRoute, private notificationService: NotificationService,
    private modalService: NgbModal, private productService: ProductService) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: [null, Validators.required],
      ruleMinimumAmount: 0,
      discountType: 'percentage',
      discountPercentage: 0,
      discountFixedAmount: 0,
      validityDuration: 1,
      programType: 'coupon_program',
      rewardProduct: null,
      rewardType: 'discount',
      ruleMinQuantity: 1,
      discountApplyOn: 'on_order',
      discountSpecificProducts: null,
      companyId: null,
      discountMaxAmount: 0,
      rewardProductQuantity: 1,
      promoApplicability: 'on_current_order'
    });

    this.route.queryParamMap.subscribe(params => {
      this.id = params.get("id");
      if (this.id) {
        this.loadRecord();
      } else {
        this.formGroup = this.fb.group({
          name: [null, Validators.required],
          ruleMinimumAmount: 0,
          discountType: 'percentage',
          discountPercentage: 0,
          discountFixedAmount: 0,
          validityDuration: 1,
          programType: 'coupon_program',
          rewardProduct: null,
          rewardType: 'discount',
          ruleMinQuantity: 1,
          discountApplyOn: 'on_order',
          discountSpecificProducts: null,
          companyId: null,
          discountMaxAmount: 0,
          rewardProductQuantity: 1,
          promoApplicability: 'on_current_order'
        });

        this.program = new SaleCouponProgramDisplay();
      }
    });

    this.productCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.productCbx.loading = true)),
      switchMap(value => this.searchProducts(value))
    ).subscribe(result => {
      this.filteredProducts = result;
      this.productCbx.loading = false;
    });

    this.loadFilteredProducts();

    this.loadListProducts();

    this.productMultiSelect.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.productMultiSelect.loading = true)),
      switchMap(value => this.searchProducts(value))
    ).subscribe(result => {
      this.listProducts = result;
      this.productMultiSelect.loading = false;
    });
  }

  get f() {
    return this.formGroup.controls;
  }

  loadFilteredProducts() {
    this.searchProducts().subscribe(result => {
      this.filteredProducts = _.unionBy(this.filteredProducts, result, 'id');
    });
  }

  loadListProducts() {
    this.searchProducts().subscribe(result => {
      this.listProducts = _.unionBy(this.listProducts, result, 'id');
    });
  }

  searchProducts(search?: string) {
    var val = new ProductFilter();
    val.saleOK = true;
    val.search = search;
    return this.productService.autocomplete2(val);
  }

  viewCoupons() {
    if (this.id) {
      let modalRef = this.modalService.open(SaleCouponListDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.programId = this.id;
      modalRef.result.then(() => {
      }, () => {
      });
    }
  }

  get discountType() {
    return this.formGroup.get('discountType').value;
  }

  get rewardProduct() {
    return this.formGroup.get('rewardProduct').value;
  }

  get rewardType() {
    return this.formGroup.get('rewardType').value;
  }

  get discountApplyOn() {
    return this.formGroup.get('discountApplyOn').value;
  }

  generateCoupons() {
    if (this.id) {
      let modalRef = this.modalService.open(SaleCouponProgramGenerateCouponsDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.programId = this.id;
      modalRef.result.then(() => {
        this.loadRecord();
      }, () => {
      });
    } else {
      if (!this.formGroup.valid) {
        return false;
      }

      var value = this.formGroup.value;
      this.programService.create(value).subscribe(result => {
        this.router.navigate(['programs/coupon-programs/form'], { queryParams: { id: result.id } });
        let modalRef = this.modalService.open(SaleCouponProgramGenerateCouponsDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
        modalRef.componentInstance.programId = result.id;
        modalRef.result.then(() => {
          this.loadRecord();
        }, () => {
        });
      });
    }

  }


  loadRecord() {
    this.programService.get(this.id).subscribe(result => {
      this.program = result;
      this.formGroup.patchValue(result);
      if (result.rewardProduct) {
        this.filteredProducts = _.unionBy(this.filteredProducts, [result.rewardProduct], 'id');
      }
    });
  }

  createNew() {
    this.router.navigate(['programs/coupon-programs/form']);
  }

  onSave() {
    this.submitted = true;
    if (!this.formGroup.valid) {
      return false;
    }

    var value = this.formGroup.value;
    if (value.rewardType == 'product' && value.rewardProduct == null) {
      this.notificationService.show({
        content: 'Vui lòng chọn dịch vụ miễn phí',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });
      return false;
    }

    if (value.rewardType == 'discount' && value.discountApplyOn == 'specific_products' &&
      (value.discountSpecificProducts == null || value.discountSpecificProducts.length == 0)) {
      this.notificationService.show({
        content: 'Vui lòng chọn dịch vụ chiết khấu',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });
      return false;
    }

    value.rewardProductId = value.rewardProduct ? value.rewardProduct.id : null;
    value.discountSpecificProductIds = value.discountSpecificProducts ? value.discountSpecificProducts.map(x => x.id) : [];
    value.ruleMinimumAmount = value.ruleMinimumAmount || 0;
    value.ruleMinQuantity = value.ruleMinQuantity || 0;
    value.validityDuration = value.validityDuration || 0;
    value.rewardProductQuantity = value.rewardProductQuantity || 0;

    if (this.id) {
      this.programService.update(this.id, value).subscribe(() => {
        this.notificationService.show({
          content: 'Lưu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.loadRecord();
      });
    } else {
      this.programService.create(value).subscribe(result => {
        this.router.navigate(['programs/coupon-programs/form'], { queryParams: { id: result.id } });
      });
    }
  }

}

