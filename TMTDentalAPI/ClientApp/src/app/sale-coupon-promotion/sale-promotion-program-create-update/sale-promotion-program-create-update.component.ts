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
  program: SaleCouponProgramDisplay = new SaleCouponProgramDisplay();
  submitted = false;
  filteredProducts: ProductSimple[];
  isSelectedDay = true;
  listDay: any[] = [];
  days: any[] = [];
  startDate = new Date();
  endDate = new Date();
  codeNeed = false;
  discountFixed = true;
  @ViewChild('productCbx', { static: true }) productCbx: ComboBoxComponent;

  listProducts: ProductSimple[];
  listProductCategories: ProductCategory[];
  @ViewChild('productMultiSelect', { static: true }) productMultiSelect: MultiSelectComponent;
  @ViewChild('productCategoriesMultiSelect', {static:true}) productCategoriesMultiSelect: MultiSelectComponent;
  constructor(private fb: FormBuilder, private programService: SaleCouponProgramService,
    private router: Router, private route: ActivatedRoute, private notificationService: NotificationService,
    private modalService: NgbModal, private productService: ProductService, private productCategoryService: ProductCategoryService, private intlService: IntlService,
    ) { }

  ngOnInit() {
   this.startDate.setHours(0,0,0,0);
   this.endDate.setHours(23,59,59,999);
    this.formGroup = this.fb.group({
      name: [null, Validators.required],
      ruleMinimumAmount: 0,
      discountType: 'percentage',
      discountPercentage: [0, Validators.required],
      discountFixedAmount:[0, Validators.required],
      validityDuration: 1,
      programType: 'promotion_program',
      rewardProduct: null,
      rewardType: 'discount',
      ruleMinQuantity: 1,
      discountApplyOn: 'on_order',
      discountSpecificProducts: [null],
      discountSpecificProductCategories: [null],
      active: false,
      notIncremental: true,
      companyId: null,
      discountMaxAmount: 0,
      rewardProductQuantity: 1,
      promoApplicability: 'on_current_order',
      promoCodeUsage: 'no_code_needed',
      ruleDateToObj: [this.endDate],
      ruleDateFromObj: [this.startDate],
      maximumUseNumber: 0,
      promoCode: null,
      daysSelected: null,
      isSelectDay: true,
      status: 'waitting'
    });
    this.route.queryParamMap.subscribe(params => {
      this.id = params.get("id");
      if (this.id) {
        this.loadRecord();
      } else {
        this.formGroup = this.fb.group({
          name: [null, Validators.required],
          ruleMinimumAmount: 0,
          discountType: 'fixed_amount',
          discountPercentage: [0, Validators.required],
          discountFixedAmount: 0,
          validityDuration: 1,
          programType: 'promotion_program',
          rewardProduct: null,
          rewardType: 'discount',
          ruleMinQuantity: 1,
          discountApplyOn: 'on_order',
          discountSpecificProducts: [null],
          discountSpecificProductCategories: [null],
          companyId: null,
          active: false,
          discountMaxAmount: 0,
          notIncremental: true,
          rewardProductQuantity: 1,
          promoApplicability: 'on_current_order',
          promoCodeUsage: 'no_code_needed',
          ruleDateToObj: [this.endDate],
          ruleDateFromObj: [this.startDate],
          maximumUseNumber: 0,
          promoCode: null,
          daysSelected: null,
          isSelectDay: true,
          status: 'waitting'
        });

        this.program = new SaleCouponProgramDisplay();
      }
    });

    
    // this.productCbx.filterChange.asObservable().pipe(
    //   debounceTime(300),
    //   tap(() => (this.productCbx.loading = true)),
    //   switchMap(value => this.searchProducts(value))
    // ).subscribe(result => {
    //   this.filteredProducts = result;
    //   this.productCbx.loading = false;
    // });

    this.loadFilteredProducts();

    this.loadListProducts();
    this.loadListProductCategories();
    this.loadListDay();
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

  loadListProductCategories(){
    this.searchProductCategories().subscribe(result => {
      this.listProductCategories = _.unionBy(this.listProductCategories,result, 'id');
    })
  }

  searchProductCategories(search?: string){
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

  get status(){
    return this.formGroup.get('status').value;
  }

  getDataFromBody(){
    var value = this.formGroup.value;
    value.rewardProductId = value.rewardProduct ? value.rewardProduct.id : null;
    value.ruleDateFrom = value.ruleDateFromObj ? this.intlService.formatDate(value.ruleDateFromObj, 'g', 'en-US') : null;
    value.ruleDateTo = value.ruleDateToObj ? this.intlService.formatDate(value.ruleDateToObj, 'g', 'en-US') : null;
    value.discountSpecificProductIds = value.discountSpecificProducts ? value.discountSpecificProducts.map(x => x.id) : [];
    value.discountSpecificProductCategoryIds = value.discountSpecificProductCategories ? value.discountSpecificProductCategories.map(x => x.id) : [];
    var days = value.daysSelected ? value.daysSelected : [];
    value.days = days.toString();
    return value;
  }

  onChangePromoCodeUsage() {
    if (this.promoCodeUsage == 'no_code_needed') {
      this.formGroup.get('promoCode').setValue(null);
      this.codeNeed = false;
    }
    else{
      this.codeNeed = true;
    }
  }

  onChangeDate(){
    var start = new Date(this.f.ruleDateFromObj.value);
    var end = new Date(this.f.ruleDateToObj.value);
    if (end<start){
      this.f.ruleDateToObj.setErrors({'dateIncorrect': true});
    }
    else{
      this.f.daysSelected.reset();
      this.loadListDay();
      this.f.ruleDateToObj.clearValidators();
      this.f.ruleDateToObj.updateValueAndValidity();
    }
    
  }

  changeCheckbox(value){
    this.f.notIncremental.setValue(value);
  }

  changeCheckboxSelectDay(value){
    this.isSelectedDay = value;
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

  showStatus(){
    switch(this.status){
      case 'waitting':
        return 'Chưa chạy';
      case 'running':
        return 'Đang chạy';
      case 'paused':
        return 'Tạm dừng'
      case 'expired':
        return 'Hết hạn';
    }
  }

  showHistoryApplyPromotion(){
    let modalRef = this.modalService.open(DiscountPricePopoverComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Giá trị khuyến mãi';
    modalRef.componentInstance.id = this.id;
    modalRef.componentInstance.typeApply = this.discountApplyOn;
  }

  loadRecord() {
    this.programService.get(this.id).subscribe(result => {
      this.program = result;
      var dayIds = result.days ? result.days.split(",") : [];
      this.f.daysSelected.setValue(dayIds.map(Number));
      
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

      this.loadListDay();
      if (result.active){
        this.disabled();
      }


    });
  }
  loadListDay(){
    this.days = [
      {id:0,name:'Chủ Nhật'},
      {id:1,name:'Thứ 2'},
      {id:2,name:'Thứ 3'},
      {id:3,name:'Thứ 4'},
      {id:4,name:'Thứ 5'},
      {id:5,name:'Thứ 6'},
      {id:6,name:'Thứ 7'},
    ]

    var start = new Date(this.f.ruleDateFromObj.value);
    var end = new Date(this.f.ruleDateToObj.value);
    var list = [];
      for (var i = start; i <= end ; i.setDate(i.getDate() + 1)) {
          if(!list.includes(i.getDay())){
            list.push(i.getDay());
          }
          
      }
      this.listDay = this.days.filter(x => {
        return list.includes(x.id);
      });
  }

  disabled(){
    this.f.name.disable();
    this.f.ruleMinQuantity.disable();
    this.f.promoCodeUsage.disable();
    this.f.promoCodeUsage.disable();
    this.f.promoCode.disable();
    this.f.notIncremental.disable();
    this.f.maximumUseNumber.disable();
    this.f.ruleDateFromObj.disable();
    this.f.ruleDateToObj.disable();
    this.f.isSelectDay.disable();
    this.f.daysSelected.disable();
    this.f.rewardProduct.disable();
    this.f.rewardProductQuantity.disable();
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

  onActive(){
    this.submitted = true;
    if (!this.formGroup.valid) {
      return false;
    }
      let modalRef =this.modalService.open(ConfirmDialogComponent, { size: 'lg', windowClass: 'o_technical_modal' });
      modalRef.componentInstance.title = 'Kích hoạt chương trình khuyến mãi';
      modalRef.componentInstance.body = 'Bạn có muốn kích hoạt chương trình khuyến mãi?';
      modalRef.result.then(()=>{
        if (this.id){
          this.programService.actionUnArchive([this.id]).subscribe(()=>{
            this.notify('Đã kích hoạt CTKM thành công');
            this.loadRecord();
          });
        }
        else {
          var value = this.getDataFromBody();
          this.programService.create(value).subscribe(result => {
            this.programService.actionUnArchive([result.id]).subscribe(()=>{
              this.notify('Đã kích hoạt CTKM thành công');
              this.loadRecord();
            });
          });
        }
      })
      
  }

  onChangeSelect(value){
    if(value == 'percentage'){
      this.f.discountFixedAmount.clearValidators();
      this.f.discountFixedAmount.updateValueAndValidity();
      this.f.discountPercentage.setValidators(Validators.required);
      this.f.discountPercentage.updateValueAndValidity();
    }
    else{
      this.f.discountPercentage.clearValidators();
      this.f.discountPercentage.updateValueAndValidity();
      this.f.discountFixedAmount.setValidators(Validators.required);
      this.f.discountFixedAmount.updateValueAndValidity();
    }
  }

  onChangeOption(e){
    var value = e.target.value;
    if(value == 'on_order'){
      this.f.discountSpecificProducts.clearValidators();
      this.f.discountSpecificProducts.updateValueAndValidity();
      this.f.discountSpecificProductCategories.clearValidators();
      this.f.discountSpecificProductCategories.updateValueAndValidity();
      this.f.ruleMinimumAmount.setValidators(Validators.required);
      this.f.ruleMinimumAmount.updateValueAndValidity();
    }
    else if(value == 'specific_products'){
      this.f.ruleMinimumAmount.clearValidators();
      this.f.ruleMinimumAmount.updateValueAndValidity();
      this.f.discountSpecificProductCategories.clearValidators();
      this.f.discountSpecificProductCategories.updateValueAndValidity();
      this.f.discountSpecificProducts.setValidators(Validators.required);
      this.f.discountSpecificProducts.updateValueAndValidity();
    }
    else{
      this.f.ruleMinimumAmount.clearValidators();
      this.f.ruleMinimumAmount.updateValueAndValidity();
      this.f.discountSpecificProducts.clearValidators();
      this.f.discountSpecificProducts.updateValueAndValidity();
      this.f.discountSpecificProductCategories.setValidators(Validators.required);
      this.f.discountSpecificProductCategories.updateValueAndValidity();
    }
  }

  onChangeDiscountType(){
    var type = this.f.discountType.value;
    if (type == "percentage"){
      this.discountFixed = false;
    }
    else{
      this.discountFixed = true;
    }
  }

  notify(alert){
    this.notificationService.show({
      content: alert,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: 'success', icon: true }
    });
  }

}



