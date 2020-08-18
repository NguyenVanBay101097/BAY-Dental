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
import { IntlService } from '@progress/kendo-angular-intl';

@Component({
  selector: 'app-sale-promotion-program-create-update',
  templateUrl: './sale-promotion-program-create-update.component.html',
  styleUrls: ['./sale-promotion-program-create-update.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class SalePromotionProgramCreateUpdateComponent implements OnInit {
  formGroup: FormGroup;
  id: string;
  program: SaleCouponProgramDisplay = new SaleCouponProgramDisplay();

  filteredProducts: ProductSimple[];
  @ViewChild('productCbx', { static: true }) productCbx: ComboBoxComponent;

  listProducts: ProductSimple[];
  @ViewChild('productMultiSelect', { static: true }) productMultiSelect: MultiSelectComponent;

  constructor(private fb: FormBuilder, private programService: SaleCouponProgramService,
    private router: Router, private route: ActivatedRoute, private notificationService: NotificationService,
    private modalService: NgbModal, private productService: ProductService, private intlService: IntlService) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: [null, Validators.required],
      ruleMinimumAmount: 0,
      discountType: 'percentage',
      discountPercentage: 0,
      discountFixedAmount: 0,
      validityDuration: 1,
      programType: 'promotion_program',
      rewardProduct: null,
      rewardType: 'discount',
      ruleMinQuantity: 1,
      discountApplyOn: 'on_order',
      discountSpecificProducts: null,
      companyId: null,
      discountMaxAmount: 0,
      rewardProductQuantity: 1,
      promoApplicability: 'on_current_order',
      promoCodeUsage: 'no_code_needed',
      ruleDateFromObj: null,
      ruleDateToObj: null,
      maximumUseNumber: 0,
      promoCode: null
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
          programType: 'promotion_program',
          rewardProduct: null,
          rewardType: 'discount',
          ruleMinQuantity: 1,
          discountApplyOn: 'on_order',
          discountSpecificProducts: null,
          companyId: null,
          discountMaxAmount: 0,
          rewardProductQuantity: 1,
          promoApplicability: 'on_current_order',
          promoCodeUsage: 'no_code_needed',
          ruleDateFromObj: null,
          ruleDateToObj: null,
          maximumUseNumber: 0,
          promoCode: null
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
    this.router.navigate(['/coupons'], { queryParams: { program_id: this.id } });
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

  get promoCodeUsage() {
    return this.formGroup.get('promoCodeUsage').value;
  }

  onChangePromoCodeUsage() {
    if (this.promoCodeUsage == 'no_code_needed') {
      this.formGroup.get('promoCode').setValue(null);
    }
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
        this.router.navigate(['programs/promotion-programs/form'], { queryParams: { id: result.id } });
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

      if (result.ruleDateFrom) {
        let ruleDateFrom = new Date(result.ruleDateFrom);
        this.formGroup.get('ruleDateFromObj').patchValue(ruleDateFrom);
      }

      if (result.ruleDateTo) {
        let ruleDateTo = new Date(result.ruleDateTo);
        this.formGroup.get('ruleDateToObj').patchValue(ruleDateTo);
      }

    });
  }

  createNew() {
    this.router.navigate(['programs/promotion-programs/form']);
  }

  onSave() {
    if (!this.formGroup.valid) {
      return false;
    }

    var value = this.formGroup.value;
    value.rewardProductId = value.rewardProduct ? value.rewardProduct.id : null;
    value.ruleDateFrom = value.ruleDateFromObj ? this.intlService.formatDate(value.ruleDateFromObj, 'g', 'en-US') : null;
    value.ruleDateTo = value.ruleDateToObj ? this.intlService.formatDate(value.ruleDateToObj, 'g', 'en-US') : null;
    value.discountSpecificProductIds = value.discountSpecificProducts ? value.discountSpecificProducts.map(x => x.id) : [];
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
        this.router.navigate(['programs/promotion-programs/form'], { queryParams: { id: result.id } });
      });
    }
  }

}


