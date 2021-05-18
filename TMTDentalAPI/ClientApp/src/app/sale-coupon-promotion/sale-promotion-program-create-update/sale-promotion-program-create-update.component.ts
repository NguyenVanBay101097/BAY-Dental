import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ValidatorFn, AbstractControl, ValidationErrors } from '@angular/forms';
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
import { ProductCategory } from 'src/app/product-categories/product-category';
import { ProductCategoryBasic, ProductCategoryFilter, ProductCategoryPaged, ProductCategoryService } from 'src/app/product-categories/product-category.service';
import { element } from 'protractor';
import { result } from 'lodash';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { DiscountPricePopoverComponent } from './discount-price-popover/discount-price-popover.component';

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
  program: any = new SaleCouponProgramDisplay();
  submitted = false;
  listDay: any[] = [
    { id: '0', name: 'Chủ Nhật' },
    { id: '1', name: 'Thứ 2' },
    { id: '2', name: 'Thứ 3' },
    { id: '3', name: 'Thứ 4' },
    { id: '4', name: 'Thứ 5' },
    { id: '5', name: 'Thứ 6' },
    { id: '6', name: 'Thứ 7' },
  ];
  listProducts: ProductSimple[];
  listProductCategories: ProductCategory[];

  @ViewChild('productCbx', { static: true }) productCbx: ComboBoxComponent;
  @ViewChild('productMultiSelect', { static: true }) productMultiSelect: MultiSelectComponent;
  @ViewChild('productCategoriesMultiSelect', { static: true }) productCategoriesMultiSelect: MultiSelectComponent;

  constructor(private fb: FormBuilder, private programService: SaleCouponProgramService,
    private router: Router, private route: ActivatedRoute, private notificationService: NotificationService,
    private modalService: NgbModal, private productService: ProductService, private productCategoryService: ProductCategoryService, private intlService: IntlService,
  ) { }

  ngOnInit() {
    var startDate = new Date();
    var endDate = new Date();
    startDate.setHours(0, 0, 0, 0);
    endDate.setHours(23, 59, 59, 999);

    this.formGroup = this.fb.group({
      name: [null, Validators.required],
      ruleMinimumAmount: 0,
      discountType: 'percentage',
      discountPercentage: 0,
      discountFixedAmount: 0,
      validityDuration: 1,
      rewardType: 'discount',
      discountSpecificProducts: [null],
      discountSpecificProductCategories: [null],
      notIncremental: false,
      discountMaxAmount: 0,
      promoCodeUsage: 'no_code_needed',
      ruleDateToObj: [startDate, Validators.required],
      ruleDateFromObj: [endDate, Validators.required],
      maximumUseNumber: 0,
      promoCode: null,
      daysSelected: null,
      isSelectDay: false,
      discountApplyOn: 'on_order'
    });

    this.route.queryParamMap.subscribe(params => {
      this.id = params.get("id");
      if (this.id) {
        this.loadRecord();
      } else {
        this.programService.defaultGet('promotion_program').subscribe((result: any) => {
          console.log(result);
          this.program = result;
        })
      }
    });

    this.loadListProducts();
    this.loadListProductCategories();

    this.productMultiSelect.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.productMultiSelect.loading = true)),
      switchMap(value => this.searchProducts(value))
    ).subscribe(result => {
      this.listProducts = result;
      this.productMultiSelect.loading = false;
    });

    this.productCategoriesMultiSelect.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.productCategoriesMultiSelect.loading = true)),
      switchMap(value => this.searchProductCategories(value))
    ).subscribe(result => {
      this.listProductCategories = result;
      this.productCategoriesMultiSelect.loading = false;
    });
  }

  get f() {
    return this.formGroup.controls;
  }

  loadListProducts() {
    this.searchProducts().subscribe(result => {
      this.listProducts = _.unionBy(this.listProducts, result, 'id');
    });
  }

  loadListProductCategories() {
    this.searchProductCategories().subscribe(result => {
      this.listProductCategories = _.unionBy(this.listProductCategories, result, 'id');
    })
  }

  searchProductCategories(search?: string) {
    var val = new ProductCategoryPaged();
    val.search = search;
    val.type = 'service';
    return this.productCategoryService.autocomplete(val);
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

  get active() {
    return this.formGroup.get('active').value;
  }

  get statusDisplay() {
    return this.formGroup.get('statusDisplay').value;
  }

  get isSelectDay() {
    return this.formGroup.get('isSelectDay').value;
  }

  getDataFromBody() {
    //program
    //value
    var value = this.formGroup.value;
    this.program = Object.assign(this.program, value);
    this.program.ruleDateFrom = value.ruleDateFromObj ? this.intlService.formatDate(value.ruleDateFromObj, 'g', 'en-US') : null;
    this.program.ruleDateTo = value.ruleDateToObj ? this.intlService.formatDate(value.ruleDateToObj, 'g', 'en-US') : null;
    // value.discountSpecificProductIds = value.discountSpecificProducts ? value.discountSpecificProducts.map(x => x.id) : [];
    // value.discountSpecificProductCategoryIds = value.discountSpecificProductCategories ? value.discountSpecificProductCategories.map(x => x.id) : [];
    // var days = value.daysSelected ? value.daysSelected : [];
    // value.days = days.toString();
    // return value;
    var result = Object.assign({}, this.program);
    result.discountSpecificProductIds = result.discountSpecificProducts.map(x => x.id);
    result.discountSpecificProductCategoryIds = result.discountSpecificProductCategories.map(x => x.id);
    result.days = result.daysSelected;
    console.log(result);
    return result;
  }

  changeCheckbox(value) {
    this.f.notIncremental.setValue(value);
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

  showHistoryApplyPromotion() {
    let modalRef = this.modalService.open(DiscountPricePopoverComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Giá trị khuyến mãi';
    modalRef.componentInstance.id = this.id;
    modalRef.componentInstance.typeApply = this.discountApplyOn;
    modalRef.componentInstance.name = this.f.name.value;
  }

  loadRecord() {
    this.programService.get(this.id).subscribe(result => {
      console.log(result);
      this.program = result;
      this.formGroup.patchValue(result);

      this.formGroup.get('daysSelected').setValue(result.days || []);
      if (result.ruleDateFrom) {
        let ruleDateFrom = new Date(result.ruleDateFrom);
        this.formGroup.get('ruleDateFromObj').patchValue(ruleDateFrom);
      }

      if (result.ruleDateTo) {
        let ruleDateTo = new Date(result.ruleDateTo);
        this.formGroup.get('ruleDateToObj').patchValue(ruleDateTo);
      }

      if (result.active) {
        this.disabled();
      }
    });
  }

  disabled() {
    this.f.name.disable();
    this.f.promoCodeUsage.disable();
    this.f.promoCode.disable();
    this.f.notIncremental.disable();
    this.f.maximumUseNumber.disable();
    this.f.ruleDateFromObj.disable();
    this.f.ruleDateToObj.disable();
    this.f.isSelectDay.disable();
    this.f.daysSelected.disable();
    this.f.discountType.disable();
    this.f.discountPercentage.disable();
    this.f.discountMaxAmount.disable();
    this.f.discountFixedAmount.disable();
    this.f.discountApplyOn.disable();
    this.f.discountSpecificProducts.disable();
    this.f.discountSpecificProductCategories.disable();
    this.f.ruleMinimumAmount.disable();
  }

  createNew() {
    this.router.navigate(['programs/promotion-programs/form']);
  }

  onSave() {
    this.submitted = true;
    if (!this.formGroup.valid) {
      return false;
    }

    var value = this.getDataFromBody();

    if (this.id) {
      this.programService.update(this.id, value).subscribe(() => {
        this.notify('Lưu thành công');
        this.loadRecord();
      });
    } else {
      this.programService.create(value).subscribe(result => {
        this.notify('Lưu thành công');
        this.router.navigate(['programs/promotion-programs/form'], { queryParams: { id: result.id } });
      });
    }
  }

  onSaveActive() {
    this.submitted = true;
    if (!this.formGroup.valid) {
      return false;
    }

    var value = this.getDataFromBody();
    if (this.id) {
      this.programService.update(this.id, value).subscribe(result => {
        this.showActiveDialog();
      })
    }
    else {
      this.programService.create(value).subscribe(result => {
        this.id = result.id;
        this.router.navigate(['programs/promotion-programs/form'], { queryParams: { id: result.id } });
        this.showActiveDialog();
      });
    }
  }

  showActiveDialog() {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'md', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Kích hoạt chương trình khuyến mãi';
    modalRef.componentInstance.body = 'Bạn có muốn kích hoạt chương trình khuyến mãi?';
    modalRef.result.then(() => {
      this.programService.actionUnArchive([this.id]).subscribe(() => {
        this.notify('CTKM đã kích hoạt thành công');
        this.loadRecord();
      });
    })
  }

  onPause() {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Tạm ngừng chương trình khuyến mãi';
    modalRef.componentInstance.body = 'Bạn có muốn tạm ngừng chương trình khuyến mãi?';
    modalRef.result.then(() => {
      this.programService.actionArchive([this.id]).subscribe(() => {
        this.notify('CTKM đã tạm ngừng thành công');
        this.loadRecord();
      });
    })
  }

  onActive() {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Kích hoạt chương trình khuyến mãi';
    modalRef.componentInstance.body = 'Bạn có muốn kích hoạt chương trình khuyến mãi?';
    modalRef.result.then(() => {
      this.programService.actionUnArchive([this.id]).subscribe(() => {
        this.notify('CTKM đã kích hoạt thành công');
        this.loadRecord();
      });
    })
  }

  onChangeSelect(value) {
    if (value == 'percentage') {
      this.f.discountFixedAmount.clearValidators();
      this.f.discountFixedAmount.updateValueAndValidity();
      this.f.discountPercentage.setValidators(Validators.required);
      this.f.discountPercentage.updateValueAndValidity();
    }
    else {
      this.f.discountPercentage.clearValidators();
      this.f.discountPercentage.updateValueAndValidity();
      this.f.discountFixedAmount.setValidators(Validators.required);
      this.f.discountFixedAmount.updateValueAndValidity();
    }
  }

  onChangeOption(e) {
    var value = e.target.value;
    if (value == 'on_order') {
      this.f.discountSpecificProducts.clearValidators();
      this.f.discountSpecificProducts.updateValueAndValidity();
      this.f.discountSpecificProductCategories.clearValidators();
      this.f.discountSpecificProductCategories.updateValueAndValidity();
      this.f.ruleMinimumAmount.setValidators(Validators.required);
      this.f.ruleMinimumAmount.updateValueAndValidity();
    }
    else if (value == 'specific_products') {
      this.f.ruleMinimumAmount.clearValidators();
      this.f.ruleMinimumAmount.updateValueAndValidity();
      this.f.discountSpecificProductCategories.clearValidators();
      this.f.discountSpecificProductCategories.updateValueAndValidity();
      this.f.discountSpecificProducts.setValidators(Validators.required);
      this.f.discountSpecificProducts.updateValueAndValidity();
    }
    else {
      this.f.ruleMinimumAmount.clearValidators();
      this.f.ruleMinimumAmount.updateValueAndValidity();
      this.f.discountSpecificProducts.clearValidators();
      this.f.discountSpecificProducts.updateValueAndValidity();
      this.f.discountSpecificProductCategories.setValidators(Validators.required);
      this.f.discountSpecificProductCategories.updateValueAndValidity();
    }
  }

  notify(alert) {
    this.notificationService.show({
      content: alert,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: 'success', icon: true }
    });
  }

  getColorStatus(status) {
    switch (status) {
      case "Chưa chạy":
        return "text-dark";
      case "Đang chạy":
        return "text-success";
      case "Tạm ngừng":
        return "text-warning";
      case "Hết hạn":
        return "text-danger";
      default:
        return "text-dark";
    }
  }

}



