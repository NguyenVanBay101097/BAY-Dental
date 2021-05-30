import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { PageChangeEvent, GridDataResult } from '@progress/kendo-angular-grid';
import { PriceListService } from '../price-list.service';
import { HttpParams } from '@angular/common/http';
import { ProductPaged, ProductBasic2 } from 'src/app/products/product.service';
import { map, switchMap } from 'rxjs/operators';
import { ProductCategoryPaged, ProductCategoryBasic } from 'src/app/product-categories/product-category.service';
import { PartnerCategoryBasic, PartnerCategoryPaged } from 'src/app/partner-categories/partner-category.service';
import * as _ from 'lodash';
import { NotificationService } from '@progress/kendo-angular-notification';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { DialogService } from '@progress/kendo-angular-dialog';
import { ProductSimple } from 'src/app/products/product-simple';
import { ProductPriceListItemSave, ProductPriceListSave } from '../price-list';
import { IntlService } from '@progress/kendo-angular-intl';
import { Observable } from 'rxjs';
import { CompanyPaged, CompanyService, CompanyBasic } from 'src/app/companies/company.service';

@Component({
  selector: 'app-price-list-create-update',
  templateUrl: './price-list-create-update.component.html',
  styleUrls: ['./price-list-create-update.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class PriceListCreateUpdateComponent implements OnInit {
  id: string;//Id bảng giá
  priceListName: string; //Tên bảng giá
  formPrice: FormGroup;
  formItem: FormGroup;

  categoryShow = false;
  serviceShow = false;

  skip = 0;
  pageSize = 6;

  maxDateStart: Date;
  minDateEnd: Date;

  pnCateg: PartnerCategoryBasic[] = [];
  serviceCategories: ProductCategoryBasic[] = [];
  services: ProductSimple[] = [];
  items: ProductPriceListItemSave[] = [];
  selectItem: ProductPriceListItemSave;
  selectIndex: number;

  optionData = [];

  listCompanies: CompanyBasic[] = [];

  constructor(private fb: FormBuilder, private service: PriceListService, private notificationService: NotificationService,
    private dialogService: DialogService, private route: ActivatedRoute, private router: Router, private intlService: IntlService,
    private companyService: CompanyService) { }


  ngOnInit() {
    this.optionData = [{ value: '3_global', text: 'Tất cả dịch vụ' }, { value: '2_product_category', text: 'Nhóm dịch vụ' }, { value: '0_product_variant', text: 'Dịch vụ' }];
    this.formPrice = this.fb.group({
      name: [null, Validators.required],
      dateStart: null,
      dateStartObj: [null, Validators.required],
      dateEnd: null,
      dateEndObj: [null, Validators.required],
      partnerCateg: null,
      company: null,
      items: this.fb.array([]),
      discountPolicy: ['with_discount']
    })

    this.formItem = this.fb.group({
      computePrice: ['fixed', Validators.required],
      appliedOn: [null, Validators.required],
      fixedPrice: 0,
      percentPrice: 0,
      product: null,
      categ: null
    })

    this.getActiveRoute();
    this.maxDateStart = this.formPrice.get('dateEndObj').value;
    this.minDateEnd = this.formPrice.get('dateStartObj').value;
    // this.loadPartnerCategories();
    this.loadListCompanies();
  }


  loadListCompanies() {
    var val = new CompanyPaged();
    val.active = true;
    this.companyService.getPaged(val).subscribe(result => {
      this.listCompanies = result.items;
    });
  }

  getActiveRoute() {
    this.route.paramMap.pipe(
      switchMap((params: ParamMap) => {
        this.id = params.get("id");
        if (this.id) {
          return this.service.getPriceList(this.id);
        } else {
          return new Observable<ProductPriceListSave>();
        }
      })).subscribe(result => {
        this.formPrice.patchValue(result);
        this.priceListName = result.name;
        let dateStart = this.intlService.parseDate(result.dateStart);
        this.formPrice.get('dateStartObj').patchValue(dateStart);
        let dateEnd = this.intlService.parseDate(result.dateEnd);
        this.formPrice.get('dateEndObj').patchValue(dateEnd);
        this.items = result.items;
        console.log(this.formPrice);

      });

    // var par: ParamMap;
    // this.id = par.get('id');
    // if (this.id) {
    //   this.service.getPriceList(this.id).subscribe(result => {
    //     this.formPrice.patchValue(result);
    //     let dateStart = this.intlService.parseDate(result.dateStart);
    //     this.formPrice.get('dateStartObj').patchValue(dateStart);
    //     let dateEnd = this.intlService.parseDate(result.dateEnd);
    //     this.formPrice.get('dateEndObj').patchValue(dateEnd);
    //     this.items = result.items;
    //     console.log(this.formPrice);
    //   })
    // }
  }

  savePriceList() {
    var val = this.formPrice.value;
    val.dateStart = this.intlService.formatDate(val.dateStartObj, 'g', 'en-US');
    val.dateEnd = this.intlService.formatDate(val.dateEndObj, 'g', 'en-US');
    val.partnerCategId = val.partnerCateg ? val.partnerCateg.id : null;
    val.companyId = val.company ? val.company.id : null;
    this.items.forEach(e => {
      e.dateStart = val.dateStart;
      e.dateEnd = val.dateEnd;
      e.partnerCategId = val.partnerCateg ? val.partnerCateg.id : '';
    });
    val.items = this.items;
    if (this.id) {
      this.service.updatePriceList(val, this.id).subscribe(
        rs => {
          this.router.navigate(['/pricelists/edit/' + this.id]);
          this.notificationService.show({
            content: 'Lưu thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
        }
      )
    } else {
      this.service.createPriceList(val).subscribe(
        rs => {
          this.router.navigate(['/pricelists/edit/' + rs.id]);
          this.notificationService.show({
            content: 'Lưu thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
        }
      )
    }
  }

  changeApply(val: string) {
    console.log(val);
    if (val) {
      switch (val) {
        case "2_product_category":
          this.categoryShow = true;
          this.serviceShow = false;
          this.loadServiceCategories();
          break;
        case "0_product_variant":
          this.serviceShow = true;
          this.categoryShow = false;
          this.loadServices();
          break;
        case "3_global":
          this.serviceShow = false;
          this.categoryShow = false;
          break;
      }
    }
  }

  transAppliedOn(str: string) {
    switch (str) {
      case "3_global":
        return 'Tất cả dịch vụ';
      case "2_product_category":
        return 'Nhóm dịch vụ';
      case "0_product_variant":
        return 'Dịch vụ';
    }
  }

  loadServices() {
    var pdPage = new ProductPaged();
    pdPage.type = 'service';
    this.service.loadServicesAutocomplete2(pdPage).subscribe(
      rs => {
        this.services = rs;
      }
    );;
  }

  loadServiceCategories() {
    var pdCategPg = new ProductCategoryPaged;
    // pdCategPg.serviceCateg = true;
    this.service.loadServiceCategories(pdCategPg).subscribe(
      rs => {
        this.serviceCategories = rs;
      }
    );
  }

  loadPartnerCategories() {
    var val = new PartnerCategoryPaged;
    this.service.partnerCategoryAutocomplete(val).subscribe(
      rs => {
        this.pnCateg = rs;
      }
    )
  }

  rowSelectionChange(e) {
    var item = e.selectedRows[0].dataItem;
    if (item.type == 'service') {
      this.loadServiceDetail(item.id);
    } else {
      this.loadProductByCategory(item.id);
    }
    // this.getInvoiceDetail(e.selectedRows[0].dataItem.id);
  }

  loadProductByCategory(id: string) {
    var pdPaged = new ProductPaged();
    pdPaged.categId = id;
    this.service.loadServicesAutocomplete2(pdPaged).subscribe(
      rs => {
        this.services = rs;
      }
    )
  }


  loadServiceDetail(id) {
    this.service.loadServiceDetail(id);
  }

  changeDateEnd(e) {
    console.log(e);
    this.maxDateStart = e;
  }

  changeDateStart(e) {
    console.log(e);
    this.minDateEnd = e;
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.pageSize = event.take;
  }

  exit() {
    this.router.navigate(['/pricelists']);
  }


  addItems() {
    if (this.checkItem()) {
      this.prepareItems();
      var val = this.formItem.value;
      val.categId = val.categ ? val.categ.id : '';
      val.productId = val.product ? val.product.id : '';

      this.items.push(val);
      this.resetForm(this.formItem);
    } else {
      this.notificationService.show({
        content: 'Quy định giá này đã tồn tại',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });
    }
  }

  checkItem() {
    switch (this.getApplyOn.value) {
      case "3_global":
        if (this.items.filter(x => x.appliedOn == "3_global").length > 0
          && this.items.filter(x => x.appliedOn == "3_global")[0] !== this.selectItem) {
          console.log(this.items.filter(x => x.appliedOn == "3_global")[0]);
          return false;
        } else {
          return true;
        }
      case "2_product_category":
        if (this.items.filter(x => (x.categ && x.categ.id == this.getCateg.value.id)).length > 0
          && this.items.filter(x => (x.categ && x.categ.id == this.getCateg.value.id))[0] !== this.selectItem) {
          console.log(this.items.filter(x => (x.categ && x.categ.id == this.getCateg.value.id))[0]);
          return false;
        } else {
          return true;
        }
      case "0_product_variant":
        if (this.items.filter(x => (x.product && x.product.id == this.getProduct.value.id)).length > 0
          && this.items.filter(x => (x.product && x.product.id == this.getProduct.value.id))[0] !== this.selectItem) {
          console.log(this.items.filter(x => (x.product && x.product.id == this.getProduct.value.id))[0]);
          return false;
        } else {
          return true;
        }
      default:
        return false;
    }
  }


  prepareItems() {
    this.getApplyOn.setValue(this.getApplyOn.value);
    if (this.formItem.get('computePrice').value == 'fixed') {
      this.formItem.get('percentPrice').setValue(0);
    } else {
      this.formItem.get('fixedPrice').setValue(0);
    }
    switch (this.getApplyOn.value) {
      case "3_global":
        this.getCateg.setValue(null);
        this.getProduct.setValue(null);
        break;
      case "2_product_category":
        this.getProduct.setValue(null);
        break;
      case "0_product_variant":
        this.getCateg.setValue(null);
        break;
    }
  }

  editItem(item) {
    this.selectItem = item;
    this.selectIndex = this.items.indexOf(this.selectItem);
    this.formItem.patchValue(this.selectItem);
    if (this.selectItem.fixedPrice > 0) {
      this.formItem.get('computePrice').setValue('fixed');
    } else if (this.selectItem.percentPrice > 0) {
      this.formItem.get('computePrice').setValue('percentage');
    }
    console.log(this.formItem.value);
    this.changeApply(item.appliedOn);
  }

  updateItem() {
    if (this.checkItem()) {
      this.prepareItems();
      var val = this.formItem.value;
      val.categId = val.categ ? val.categ.id : '';
      val.productId = val.product ? val.product.id : '';
      this.items[this.selectIndex] = val;
      this.resetForm(this.formItem);
    } else {
      this.notificationService.show({
        content: 'Quy định giá này đã tồn tại',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });
    }
  }

  removeItem(index) {
    this.items.splice(index, 1);
    this.resetForm(this.formItem);
  }

  get getProduct() {
    return this.formItem.get('product');
  }

  get getCateg() {
    return this.formItem.get('categ');
  }

  get getApplyOn() {
    return this.formItem.get('appliedOn');
  }

  get getName() {
    return this.formPrice.get('name');
  }

  get getdateStartObj() {
    return this.formItem.get('dateStartObj');
  }

  get getdateEndObj() {
    return this.formItem.get('dateEndObj');
  }

  resetForm(form: FormGroup) {
    console.log(this.items);
    this.selectItem = null;
    form.reset();
    this.serviceShow = false;
    this.categoryShow = false;
    this.formItem.get('computePrice').setValue('fixed');
  }
}
