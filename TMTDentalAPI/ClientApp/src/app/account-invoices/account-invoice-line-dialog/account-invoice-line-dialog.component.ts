import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { UserSimple } from 'src/app/users/user-simple';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap, map } from 'rxjs/operators';
import { ProductFilter, ProductService } from 'src/app/products/product.service';
import { UserService } from 'src/app/users/user.service';
import { WindowRef } from '@progress/kendo-angular-dialog';
import { ProductSimple } from 'src/app/products/product-simple';
import { AccountInvoiceLineService, AccountInvoiceLineOnChangeProduct } from '../account-invoice-line.service';
import { Observable } from 'rxjs';
import { AccountInvoiceLineDisplay } from '../account-invoice-line-display';
import { PartnerService, PartnerFilter } from 'src/app/partners/partner.service';
import { PartnerSimple } from 'src/app/partners/partner-simple';
import { ToothService, ToothFilter, ToothDisplay } from 'src/app/teeth/tooth.service';
import { ToothCategoryService, ToothCategoryBasic } from 'src/app/tooth-categories/tooth-category.service';
import * as _ from 'lodash';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-account-invoice-line-dialog',
  templateUrl: './account-invoice-line-dialog.component.html',
  styleUrls: ['./account-invoice-line-dialog.component.css']
})
export class AccountInvoiceLineDialogComponent implements OnInit {

  partnerId: string;
  invLineForm: FormGroup;
  filteredProducts: ProductSimple[];
  //filteredEmployees: PartnerSimple[];
  invoiceType: string;
  line: AccountInvoiceLineDisplay;
  filteredToothCategories: ToothCategoryBasic[];
  @ViewChild('productCbx', { static: true }) productCbx: ComboBoxComponent;
  //@ViewChild('employeeCbx', { static: true }) employeeCbx: ComboBoxComponent;

  hamList: { [key: string]: {} };
  teethSelected: ToothDisplay[] = [];

  title: string;


  constructor(private fb: FormBuilder, private productService: ProductService,
    private userService: UserService, public activeModal: NgbActiveModal, private invLineService: AccountInvoiceLineService,
    private partnerService: PartnerService, private toothService: ToothService, private toothCategoryService: ToothCategoryService) { }

  ngOnInit() {
    this.invLineForm = this.fb.group({
      name: '',
      product: [null, Validators.required],
      employee: null,
      productId: null,
      priceUnit: 0,
      quantity: 1,
      discount: 0,
      priceSubTotal: 1,
      accountId: null,
      uoMId: null,
      diagnostic: null,
      note: null,
      toothCategory: null,
      type: this.teethSelected.length > 0 ? 'single' : 'all'
    });

    setTimeout(() => {
      this.productCbx.focus();
    }, 200);

    if (this.line) {
      if (this.line.product) {
        this.filteredProducts = _.unionBy(this.filteredProducts, [this.line.product], 'id');
      }

      // if (this.line.employee) {
      //   this.filteredEmployees = _.unionBy(this.filteredEmployees, [this.line.employee], 'id');
      // }

      this.invLineForm.patchValue(this.line);
      this.teethSelected = [...this.line.teeth];
      if (this.line.toothCategory) {
        this.loadTeethMap(this.line.toothCategory);
      }
    } else {
      this.loadDefaultToothCategory().subscribe(result => {
        this.invLineForm.get('toothCategory').patchValue(result);
        this.loadTeethMap(result);
      })
    }

    this.productCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.productCbx.loading = true)),
      switchMap(value => this.searchProducts(value))
    ).subscribe(result => {
      this.filteredProducts = result;
      this.productCbx.loading = false;
    });

    // this.employeeCbx.filterChange.asObservable().pipe(
    //   debounceTime(300),
    //   tap(() => (this.employeeCbx.loading = true)),
    //   switchMap(value => this.searchEmployees(value))
    // ).subscribe(result => {
    //   this.filteredEmployees = result;
    //   this.employeeCbx.loading = false;
    // });

    this.loadToothCategories();

    this.loadFilteredProducts();
  }

  loadFilteredProducts() {
    this.searchProducts().subscribe(result => this.filteredProducts = result);
  }

  processTeeth(teeth: ToothDisplay[]) {
    console.log(teeth);
    this.hamList = {
      '0_up': { '0_right': [], '1_left': [] },
      '1_down': { '0_right': [], '1_left': [] }
    };

    for (var i = 0; i < teeth.length; i++) {
      var tooth = teeth[i];
      if (tooth.position === '1_left') {
        this.hamList[tooth.viTriHam][tooth.position].push(tooth);
      } else {
        this.hamList[tooth.viTriHam][tooth.position].unshift(tooth);
      }
    }
  }

  loadTeethMap(categ: ToothCategoryBasic) {
    var val = new ToothFilter();
    val.categoryId = categ.id;
    return this.toothService.getAllBasic(val).subscribe(result => this.processTeeth(result));
  }

  onSelected(tooth: ToothDisplay) {
    if (this.isSelected(tooth)) {
      var index = this.getSelectedIndex(tooth);
      this.teethSelected.splice(index, 1);
      // this.invLineForm.get('quantity').setValue(this.getQuantity() - 1);
    } else {
      this.teethSelected.push(tooth);
      // this.invLineForm.get('quantity').setValue(this.getQuantity() + 1);
    }
  }

  getSelectedIndex(tooth: ToothDisplay) {
    for (var i = 0; i < this.teethSelected.length; i++) {
      if (this.teethSelected[i].id === tooth.id) {
        return i;
      }
    }

    return null;
  }

  isSelected(tooth: ToothDisplay) {
    for (var i = 0; i < this.teethSelected.length; i++) {
      if (this.teethSelected[i].id === tooth.id) {
        return true;
      }
    }

    return false;
  }

  loadDefaultToothCategory() {
    return this.toothCategoryService.getDefaultCategory();
  }

  loadToothCategories() {
    return this.toothCategoryService.getAll().subscribe(result => this.filteredToothCategories = result);
  }

  // loadEmployees() {
  //   return this.searchEmployees().subscribe(result => this.filteredEmployees = result);
  // }

  searchUsers(filter: string) {
    return this.userService.autocomplete(filter);
  }

  searchEmployees(search?: string) {
    var val = new PartnerFilter();
    val.search = search;
    val.employee = true;
    return this.partnerService.autocomplete2(val);
  }

  searchProducts(search?: string) {
    var val = new ProductFilter();
    val.saleOK = true;
    val.type = 'service';
    val.search = search;
    return this.productService.autocomplete2(val);
  }

  getPriceSubTotal() {
    return (this.getPriceUnit() * (1 - this.getDiscount() / 100)) * this.getQuantity();
  }

  getPriceUnit() {
    return this.invLineForm.get('priceUnit').value;
  }

  getQuantity() {
    return this.invLineForm.get('quantity').value;
  }

  getDiscount() {
    return this.invLineForm.get('discount').value;
  }

  onChangeProduct(value: any) {
    if (value && value.id) {
      var val = new AccountInvoiceLineOnChangeProduct();
      val.productId = value.id;
      val.partnerId = this.partnerId ? this.partnerId : '';
      val.invoiceType = this.invoiceType;
      this.invLineService.onChangeProduct(val).subscribe(result => {
        this.invLineForm.patchValue(result);
      });
    }
  }

  onChangeToothCategory(value: any) {
    if (value.id) {
      this.teethSelected = [];
      // this.invLineForm.get('quantity').setValue(0);
      this.loadTeethMap(value);
    }
  }

  onSave() {
    if (!this.invLineForm.valid) {
      return;
    }

    var val = this.invLineForm.value;
    val.productId = val.product.id;
    val.employeeId = val.employee ? val.employee.id : null;
    val.toothCategoryId = val.toothCategory ? val.toothCategory.id : null;
    val.priceSubTotal = this.getPriceSubTotal();
    val.teeth = this.teethSelected;
    // if (this.getUnitType == 'single') {
    //   val.teeth = this.teethSelected.sort(this.compareTeeth);
    // }
    // else {
    //   val.teeth = [];
    // }

    this.activeModal.close(val);
  }

  compareTeeth(r1: ToothDisplay, r2: ToothDisplay) {
    if (parseInt(r1.name) > parseInt(r2.name)) {
      return 1;
    } else if (parseInt(r1.name) < parseInt(r2.name)) {
      return -1;
    } else {
      return 0;
    }
  }

  onCancel() {
    this.activeModal.dismiss();
  }

  get getUnitType() {
    return this.invLineForm.get('type').value;
  }


  changeUnit() {
    if (this.getUnitType == 'single') {
      if (this.teethSelected.length > 0) {
        this.invLineForm.get('quantity').setValue(this.teethSelected.length);
      } else {
        this.invLineForm.get('quantity').setValue(0);
      }
    } else {
      this.invLineForm.get('quantity').setValue(1);
    }
  }
}
