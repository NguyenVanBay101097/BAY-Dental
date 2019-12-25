
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap, map } from 'rxjs/operators';
import { ProductFilter, ProductService } from 'src/app/products/product.service';
import { ProductSimple } from 'src/app/products/product-simple';
import * as _ from 'lodash';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { SaleOrderLineDisplay } from '../sale-order-line-display';
import { SaleOrderLineService, SaleOrderLineOnChangeProduct } from '../sale-order-line.service';
import { ToothCategoryBasic, ToothCategoryService } from 'src/app/tooth-categories/tooth-category.service';
import { ToothDisplay, ToothFilter, ToothService } from 'src/app/teeth/tooth.service';

@Component({
  selector: 'app-sale-order-line-dialog',
  templateUrl: './sale-order-line-dialog.component.html',
  styleUrls: ['./sale-order-line-dialog.component.css']
})
export class SaleOrderLineDialogComponent implements OnInit {
  saleLineForm: FormGroup;
  filteredProducts: ProductSimple[];
  line: SaleOrderLineDisplay;
  @ViewChild('productCbx', { static: true }) productCbx: ComboBoxComponent;
  title: string;
  filteredToothCategories: ToothCategoryBasic[] = [];

  hamList: { [key: string]: {} };
  teethSelected: ToothDisplay[] = [];

  partnerId: string;
  pricelistId: string;

  constructor(private fb: FormBuilder, private productService: ProductService,
    public activeModal: NgbActiveModal, private saleLineService: SaleOrderLineService,
    private toothService: ToothService, private toothCategoryService: ToothCategoryService) { }

  ngOnInit() {
    this.saleLineForm = this.fb.group({
      name: '',
      product: [null, Validators.required],
      productId: null,
      priceUnit: 0,
      productUOMQty: 1,
      discount: 0,
      priceSubTotal: 1,
      diagnostic: null,
      toothCategory: null,
      state: 'draft'
    });

    setTimeout(() => {
      this.productCbx.focus();
    }, 200);

    if (this.line) {
      setTimeout(() => {
        if (this.line.product) {
          this.filteredProducts = _.unionBy(this.filteredProducts, [this.line.product], 'id');
        }

        this.saleLineForm.patchValue(this.line);
        this.teethSelected = [...this.line.teeth];
        console.log(this.line);
        if (this.line.toothCategory) {
          this.loadTeethMap(this.line.toothCategory);
        }
      });
    } else {
      setTimeout(() => {
        this.loadDefaultToothCategory().subscribe(result => {
          this.saleLineForm.get('toothCategory').patchValue(result);
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

  get lineState() {
    return this.line ? this.line.state : 'draft';
  }

  onSelected(tooth: ToothDisplay) {
    if (this.lineState != "draft") {
      return false;
    }

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
    return this.toothCategoryService.getAll().subscribe(result => this.filteredToothCategories = result);
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
      this.filteredProducts = _.unionBy(this.filteredProducts, result, 'id');
    });
  }

  searchProducts(search?: string) {
    var val = new ProductFilter();
    val.saleOK = true;
    val.search = search;
    return this.productService.autocomplete2(val);
  }

  getPriceSubTotal() {
    var subtotal = (this.getPriceUnit() * (1 - this.getDiscount() / 100)) * this.getQuantity();
    return subtotal;
  }

  getPriceUnit() {
    return this.saleLineForm.get('priceUnit').value;
  }

  getQuantity() {
    return this.saleLineForm.get('productUOMQty').value;
  }

  getDiscount() {
    return this.saleLineForm.get('discount').value;
  }

  onChangeProduct(value: any) {
    if (value) {
      var val = new SaleOrderLineOnChangeProduct();
      val.productId = value.id;
      val.partnerId = this.partnerId;
      val.pricelistId = this.pricelistId;
      this.saleLineService.onChangeProduct(val).subscribe(result => {
        this.saleLineForm.patchValue(result);
      });
    }
  }

  onChangeToothCategory(value: any) {
    if (value.id) {
      this.teethSelected = [];
      this.loadTeethMap(value);
    }
  }


  onSave() {
    if (!this.saleLineForm.valid) {
      return;
    }

    var val = this.saleLineForm.value;
    val.productId = val.product.id;
    val.toothCategoryId = val.toothCategory ? val.toothCategory.id : null;
    val.priceSubTotal = this.getPriceSubTotal();
    val.teeth = this.teethSelected;
    this.activeModal.close(val);
  }

  onCancel() {
    this.activeModal.dismiss();
  }
}

