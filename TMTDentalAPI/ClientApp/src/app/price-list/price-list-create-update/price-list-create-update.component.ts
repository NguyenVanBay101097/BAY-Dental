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
import { MatGridTileHeaderCssMatStyler } from '@angular/material/grid-list';
import { ProductSimple } from 'src/app/products/product-simple';
import { ProductPriceListItemSave } from '../price-list';
import { FlexAlignStyleBuilder } from '@angular/flex-layout';

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

  today = new Date;
  constructor(private fb: FormBuilder, private service: PriceListService, private notificationService: NotificationService,
    private dialogService: DialogService, private route: ActivatedRoute, private router: Router) { }


  ngOnInit() {
    this.optionData = [{ value: '3_global', text: 'Tất cả' }, { value: '2_product_category', text: 'Nhóm dịch vụ' }, { value: '0_product_variant', text: 'Dịch vụ' }];
    this.formPrice = this.fb.group({
      name: [null, Validators.required],
      dateStart: null,
      dateStartObj: [this.today, Validators.required],
      dateEnd: null,
      dateEndObj: [null, Validators.required],
      partnerCatergory: null,
      items: this.fb.array([])
    })

    this.formItem = this.fb.group({
      discountType: ['fixed', Validators.required],
      appliedOn: null,
      appliedOnObj: [null, Validators.required],
      fixedPrice: 0,
      percentPrice: 0,
      product: null,
      categ: null
    })

    this.maxDateStart = this.formPrice.get('dateEndObj').value;
    this.minDateEnd = this.formPrice.get('dateStartObj').value;
    this.loadPartnerCategories();
    this.getActiveRoute();

  }

  getActiveRoute() {
    // this.route.paramMap.pipe(
    //   switchMap((params: ParamMap) => {
    //     this.id = params.get("id");
    //     if (this.id) {

    //     } else {
    //     }
    //   })).subscribe(result => {
    //     this.formPrice.patchValue(result);        
    //   });
  }

  savePriceList() {
    var val = this.formPrice.value;
    val.items = this.items;
    if (this.id) {
      this.service.updatePriceList(val, this.id).subscribe(
        rs => {
          this.router.navigate(['/price-list/edit/' + this.id]);
        }
      )
    } else {
      this.service.createPriceList(val).subscribe(
        rs => {
          this.router.navigate(['/price-list/edit/' + val.id]);
        }
      )
    }
  }

  changeApply(val: string) {
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
        return 'Tất cả';
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
    pdCategPg.serviceCateg = true;
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
    this.router.navigate(['/price-list-list']);
  }


  addItems() {
    if (this.checkItem()) {
      this.prepareItems();

      this.items.push(this.formItem.value);
      this.resetForm(this.formItem);
    } else {
      this.notificationService.show({
        content: 'Giảm giá này đã tồn tại',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });
    }
  }

  checkItem() {
    debugger;
    switch (this.getApplyOnObj.value.value) {
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
    this.getApplyOn.setValue(this.getApplyOnObj.value.value);
    if (this.formItem.get('discountType').value == 'fixed') {
      this.formItem.get('percentPrice').setValue(0);
    } else {
      this.formItem.get('fixedPrice').setValue(0);
    }
    switch (this.getApplyOnObj.value.value) {
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

  editItem(index) {
    var item = this.items[index];
    this.selectIndex = index;
    this.selectItem = item;
    this.formItem.patchValue(item);
    this.changeApply(item.appliedOn);
  }

  updateItem() {
    if (this.checkItem()) {
      this.prepareItems();
      this.items[this.selectIndex] = this.formItem.value;
      this.resetForm(this.formItem);
    } else {
      this.notificationService.show({
        content: 'Giảm giá này đã tồn tại',
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

  get getApplyOnObj() {
    return this.formItem.get('appliedOnObj');
  }

  get getApplyOn() {
    return this.formItem.get('appliedOn');
  }


  resetForm(form: FormGroup) {
    console.log(this.items);
    this.selectItem = null;
    form.reset();
    this.serviceShow = false;
    this.categoryShow = false;
    this.formItem.get('discountType').setValue('fixed');
  }
}
