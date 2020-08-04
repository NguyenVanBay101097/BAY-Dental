import { Component, OnInit, ViewChild } from "@angular/core";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import { ComboBoxComponent } from "@progress/kendo-angular-dropdowns";
import { debounceTime, tap, switchMap, map } from "rxjs/operators";
import {
  ProductFilter,
  ProductService,
} from "src/app/products/product.service";
import { ProductSimple } from "src/app/products/product-simple";
import * as _ from "lodash";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import {
  ToothCategoryBasic,
  ToothCategoryService,
} from "src/app/tooth-categories/tooth-category.service";
import {
  ToothDisplay,
  ToothFilter,
  ToothService,
  ToothBasic,
} from "src/app/teeth/tooth.service";
import {
  LaboOrderLineDisplay,
  LaboOrderLineService,
  LaboOrderLineOnChangeProduct,
  LaboOrderLineDefaultGet,
} from "src/app/labo-order-lines/labo-order-line.service";
import { IntlService } from "@progress/kendo-angular-intl";
import { SaleOrderLineDisplay } from 'src/app/sale-orders/sale-order-line-display';
import { SaleOrderLineService } from 'src/app/sale-orders/sale-order-line.service';

@Component({
  selector: "app-labo-order-cu-line-dialog",
  templateUrl: "./labo-order-cu-line-dialog.component.html",
  styleUrls: ["./labo-order-cu-line-dialog.component.css"],
})
export class LaboOrderCuLineDialogComponent implements OnInit {
  saleOrderLineId: string;
  saleOrderLine: SaleOrderLineDisplay;

  formGroup: FormGroup;
  submitted = false;
  filteredProducts: ProductSimple[];
  line: LaboOrderLineDisplay;
  title: string;
  filteredToothCategories: ToothCategoryBasic[] = [];

  hamList: { [key: string]: {} };
  teethSelected: ToothDisplay[] = [];
  teethList: ToothDisplay[] = [];
  toothList: ToothBasic[] = [];

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    public activeModal: NgbActiveModal,
    private toothService: ToothService,
    private toothCategoryService: ToothCategoryService,
    private laboOrderLineService: LaboOrderLineService,
    private intlService: IntlService, private saleLineService: SaleOrderLineService
  ) {}

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: "",
      product: [this.line ? this.line.product : null, Validators.required],
      productId: null,
      priceUnit: [0, Validators.required],
      productQty: [1, Validators.required],
      priceSubTotal: 1,
      // toothCategory: this.line ? this.line.toothCategory : this.saleOrderLine.toothCategory,
      color: null,
      note: null,
      warrantyCode: null,
      warrantyPeriodObj: null,
      state: "draft",
      saleOrderLineId: this.saleOrderLineId
    });

    if (this.line) {
      setTimeout(() => {
        debugger;
        this.formGroup.patchValue(this.line);
        this.teethSelected = [...this.line.teeth];
        if (this.line.warrantyPeriod) {
          let warrantyPeriod = this.intlService.parseDate(
            this.line.warrantyPeriod
          );
          this.formGroup.get("warrantyPeriodObj").patchValue(warrantyPeriod);
        }
      });
    } else {
      setTimeout(() => {
        var val = new LaboOrderLineDefaultGet();
        val.saleOrderLineId = this.saleOrderLineId;

        this.laboOrderLineService.defaultGet(val).subscribe((result: any) => {
          this.formGroup.patchValue(result);
        });
      });
    }

    setTimeout(() => {
      this.loadToothList(); 
    });
  }

  loadToothList() {
    if (this.saleOrderLineId) {
      this.saleLineService.getTeeth(this.saleOrderLineId).subscribe((result: any) => {
        this.toothList = result;
      });
    }
  }

  get f() {
    return this.formGroup.controls;
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

    //update quantity combobox
    if (this.teethSelected.length > 0) {
      this.formGroup.get("productQty").setValue(this.teethSelected.length);
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
    return this.toothCategoryService.getAll().subscribe((result) => {
      this.filteredToothCategories = result;
    });
  }

  loadDefaultToothCategory() {
    return this.toothCategoryService.getDefaultCategory();
  }

  processTeeth(teeth: ToothDisplay[]) {
    this.hamList = {
      "0_up": { "0_right": [], "1_left": [] },
      "1_down": { "0_right": [], "1_left": [] },
    };

    for (var i = 0; i < teeth.length; i++) {
      var tooth = teeth[i];
      if (tooth.position === "1_left") {
        this.hamList[tooth.viTriHam][tooth.position].push(tooth);
      } else {
        this.hamList[tooth.viTriHam][tooth.position].unshift(tooth);
      }
    }
  }

  loadTeethMap() {
    this.teethSelected = this.line ? [...this.line.teeth] : [...this.saleOrderLine.teeth];
    this.teethList = this.line ? ((this.line.state === 'draft' && this.line.teethListVirtual) ?
    this.line.teethListVirtual : this.line.teeth) : this.saleOrderLine.teeth;
  }

  loadFilteredProducts() {
    this.searchProducts().subscribe((result) => {
      this.filteredProducts = result;
    });
  }

  searchProducts(search?: string) {
    var val = new ProductFilter();
    val.isLabo = true;
    val.search = search || "";
    return this.productService.autocomplete2(val);
  }

  get lineState() {
    return this.line ? this.line.state : "draft";
  }

  getPriceSubTotal() {
    return this.getPriceUnit() * this.getQuantity();
  }

  getPriceUnit() {
    return this.formGroup.get("priceUnit").value;
  }

  getQuantity() {
    return this.formGroup.get("productQty").value;
  }

  getDiscount() {
    return this.formGroup.get("discount").value;
  }

  onChangeProduct(value: any) {
    if (value) {
      var val = new LaboOrderLineOnChangeProduct();
      val.productId = value.id;
      this.laboOrderLineService.onChangeProduct(val).subscribe((result) => {
        this.formGroup.patchValue(result);
      });
    }
  }

  onChangeToothCategory(value: any) {
    if (value.id) {
      this.teethSelected = [];
      // this.loadTeethMap(value);
    }
  }

  onSave() {
    this.submitted = true;

    if (!this.formGroup.valid) {
      return;
    }

    var val = this.formGroup.value;
    val.productId = val.product.id;
    val.toothCategoryId = val.toothCategory ? val.toothCategory.id : null;
    val.priceSubtotal = this.getPriceSubTotal();
    val.teeth = this.teethSelected;
    val.teethListVirtual = this.teethList;
    val.warrantyPeriod = val.warrantyPeriodObj
      ? this.intlService.formatDate(
        val.warrantyPeriodObj,
        "yyyy-MM-ddTHH:mm:ss"
      )
      : null;
    this.activeModal.close(val);
  }

  onCancel() {
    this.submitted = false;
    this.activeModal.dismiss();
  }
}
