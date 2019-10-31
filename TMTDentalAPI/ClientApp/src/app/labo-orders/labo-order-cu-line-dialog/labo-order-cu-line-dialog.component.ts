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
import { LaboOrderLineDisplay } from 'src/app/labo-order-lines/labo-order-line.service';

@Component({
  selector: 'app-labo-order-cu-line-dialog',
  templateUrl: './labo-order-cu-line-dialog.component.html',
  styleUrls: ['./labo-order-cu-line-dialog.component.css']
})
export class LaboOrderCuLineDialogComponent implements OnInit {
  formGroup: FormGroup;
  filteredProducts: ProductSimple[];
  line: LaboOrderLineDisplay;
  @ViewChild('productCbx', { static: true }) productCbx: ComboBoxComponent;
  title: string;
  filteredToothCategories: ToothCategoryBasic[] = [];

  hamList: { [key: string]: {} };
  teethSelected: ToothDisplay[] = [];

  constructor(private fb: FormBuilder, private productService: ProductService,
    public activeModal: NgbActiveModal,
    private toothService: ToothService, private toothCategoryService: ToothCategoryService) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: '',
      product: [null, Validators.required],
      productId: null,
      priceUnit: 0,
      productQty: 1,
      priceSubTotal: 1,
      toothCategory: null,
      color: null,
      note: null,
    });

    setTimeout(() => {
      this.productCbx.focus();
    }, 200);

    if (this.line) {
      if (this.line.product) {
        this.filteredProducts = _.unionBy(this.filteredProducts, [this.line.product], 'id');
      }

      this.formGroup.patchValue(this.line);
      this.teethSelected = [...this.line.teeth];
      if (this.line.toothCategory) {
        this.loadTeethMap(this.line.toothCategory);
      }
    } else {
      setTimeout(() => {
        this.loadDefaultToothCategory().subscribe(result => {
          this.formGroup.get('toothCategory').patchValue(result);
          this.loadTeethMap(result);
        })
      });
    }

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
      this.loadToothCategories();
    });

  }

  onSelected(tooth: ToothDisplay) {
    if (this.isSelected(tooth)) {
      var index = this.getSelectedIndex(tooth);
      this.teethSelected.splice(index, 1);
    } else {
      this.teethSelected.push(tooth);
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

  loadToothCategories() {
    return this.toothCategoryService.getAll().subscribe(result => {
      this.filteredToothCategories = result;
    });
  }

  loadDefaultToothCategory() {
    return this.toothCategoryService.getDefaultCategory();
  }

  processTeeth(teeth: ToothDisplay[]) {
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

  loadFilteredProducts() {
    this.searchProducts().subscribe(result => {
      this.filteredProducts = result;
    });
  }

  searchProducts(search?: string) {
    var val = new ProductFilter();
    val.type2 = 'labo';
    val.search = search;
    return this.productService.autocomplete2(val);
  }

  getPriceSubTotal() {
    return this.getPriceUnit() * this.getQuantity();
  }

  getPriceUnit() {
    return this.formGroup.get('priceUnit').value;
  }

  getQuantity() {
    return this.formGroup.get('productQty').value;
  }

  getDiscount() {
    return this.formGroup.get('discount').value;
  }

  onChangeProduct(value: any) {
    var val = this.formGroup.value;
    // this.saleLineService.onChangeProduct(val).subscribe(result => {
    //   this.formGroup.patchValue(result);
    // });
  }

  onChangeToothCategory(value: any) {
    if (value.id) {
      this.teethSelected = [];
      this.loadTeethMap(value);
    }
  }


  onSave() {
    if (!this.formGroup.valid) {
      return;
    }

    var val = this.formGroup.value;
    val.productId = val.product.id;
    val.toothCategoryId = val.toothCategory ? val.toothCategory.id : null;
    val.priceSubtotal = this.getPriceSubTotal();
    val.teeth = this.teethSelected;
    this.activeModal.close(val);
  }

  onCancel() {
    this.activeModal.dismiss();
  }
}


