import { Component, OnInit, ViewChild } from "@angular/core";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import {
  LaboOrderLineService,
  LaboOrderLineDefaultGet,
  LaboOrderLineOnChangeProduct,
} from "../labo-order-line.service";
import * as _ from "lodash";
import { IntlService } from "@progress/kendo-angular-intl";
import { PartnerSimple, PartnerPaged } from "src/app/partners/partner-simple";
import { ProductSimple } from "src/app/products/product-simple";
import { ComboBoxComponent } from "@progress/kendo-angular-dropdowns";
import {
  PartnerFilter,
  PartnerService,
} from "src/app/partners/partner.service";
import {
  ProductService,
  ProductFilter,
  ProductPaged,
} from "src/app/products/product.service";
import { debounceTime, tap, switchMap } from "rxjs/operators";
import { WindowRef } from "@progress/kendo-angular-dialog";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";

@Component({
  selector: "app-labo-order-line-cu-dialog",
  templateUrl: "./labo-order-line-cu-dialog.component.html",
  styleUrls: ["./labo-order-line-cu-dialog.component.css"],
})
export class LaboOrderLineCuDialogComponent implements OnInit {
  lineForm: FormGroup;
  id: string;
  submitted = false;
  invoiceId: string;
  dotKhamId: string;
  filteredCustomers: PartnerSimple[] = [];
  filteredSuppliers: PartnerSimple[] = [];
  filteredProducts: ProductSimple[] = [];
  title: string;

  @ViewChild("customerCbx", { static: true }) customerCbx: ComboBoxComponent;
  @ViewChild("supplierCbx", { static: true }) supplierCbx: ComboBoxComponent;
  @ViewChild("productCbx", { static: true }) productCbx: ComboBoxComponent;

  constructor(
    private fb: FormBuilder,
    private laboOrderLineService: LaboOrderLineService,
    private intlService: IntlService,
    private partnerService: PartnerService,
    private productService: ProductService,
    public activeModal: NgbActiveModal
  ) {}

  ngOnInit() {
    this.lineForm = this.fb.group({
      name: null,
      customer: [null, Validators.required],
      product: [null, Validators.required],
      supplier: [null, Validators.required],
      color: null,
      quantity: [null, Validators.required],
      priceUnit: [null, Validators.required],
      priceSubtotal: null,
      warrantyCode: null,
      warrantyPeriodObj: null,
      companyId: null,
      note: null,
      invoiceId: null,
      dotKhamId: null,
      sentDateObj: null,
      receivedDateObj: null,
    });

    setTimeout(() => {
      this.supplierCbx.focus();
    }, 200);

    this.loadData();

    this.productCbx.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.productCbx.loading = true)),
        switchMap((value) => this.searchProducts(value))
      )
      .subscribe((result) => {
        this.filteredProducts = result;
        this.productCbx.loading = false;
      });

    this.customerCbx.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.customerCbx.loading = true)),
        switchMap((value) => this.searchCustomers(value))
      )
      .subscribe((result) => {
        this.filteredCustomers = result;
        this.customerCbx.loading = false;
      });

    this.supplierCbx.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.supplierCbx.loading = true)),
        switchMap((value) => this.searchSuppliers(value))
      )
      .subscribe((result) => {
        this.filteredSuppliers = result;
        this.supplierCbx.loading = false;
      });

    this.loadFilteredCustomers();
    this.loadFilteredSuppliers();
    this.loadFilteredProducts();
  }

  get f() {
    return this.lineForm.controls;
  }

  loadFilteredCustomers() {
    this.searchCustomers().subscribe(
      (result) => (this.filteredCustomers = result)
    );
  }

  loadFilteredSuppliers() {
    this.searchSuppliers().subscribe(
      (result) => (this.filteredSuppliers = result)
    );
  }

  loadFilteredProducts() {
    this.searchProducts().subscribe(
      (result) => (this.filteredProducts = result)
    );
  }

  loadData() {
    if (this.id) {
      this.laboOrderLineService.get(this.id).subscribe((result) => {
        console.log(result);
        if (result.customer) {
          this.filteredCustomers.push(result.customer);
        }
        if (result.supplier) {
          this.filteredSuppliers.push(result.supplier);
        }
        if (result.product) {
          this.filteredProducts.push(result.product);
        }

        this.lineForm.patchValue(result);

        if (result.sentDate) {
          let sentDate = this.intlService.parseDate(result.sentDate);
          this.lineForm.get("sentDateObj").patchValue(sentDate);
        }

        if (result.receivedDate) {
          let receivedDate = this.intlService.parseDate(result.receivedDate);
          this.lineForm.get("receivedDateObj").patchValue(receivedDate);
        }

        if (result.warrantyPeriod) {
          let warrantyPeriod = this.intlService.parseDate(
            result.warrantyPeriod
          );
          this.lineForm.get("warrantyPeriodObj").patchValue(warrantyPeriod);
        }
      });
    } else {
      var val = new LaboOrderLineDefaultGet();
      val.invoiceId = this.invoiceId;
      val.dotKhamId = this.dotKhamId;
      this.laboOrderLineService.defaultGet(val).subscribe((result) => {
        if (result.customer) {
          this.filteredCustomers.push(result.customer);
        }

        this.lineForm.patchValue(result);
        if (result.sentDate) {
          let sentDate = this.intlService.parseDate(result.sentDate);
          this.lineForm.get("sentDateObj").patchValue(sentDate);
        }
      });
    }
  }

  searchCustomers(search?: string) {
    var val = new PartnerPaged();
    val.search = search;
    val.customer = true;
    return this.partnerService.getAutocompleteSimple(val);
  }

  searchSuppliers(search?: string) {
    var val = new PartnerPaged();
    val.search = search;
    val.supplier = true;
    return this.partnerService.getAutocompleteSimple(val);
  }

  searchProducts(search?: string) {
    var val = new ProductPaged();
    val.isLabo = true;
    val.search = search;
    return this.productService.autocomplete2(val);
  }

  onChangeProduct(value) {
    if (value) {
      var val = new LaboOrderLineOnChangeProduct();
      val.productId = value.id;
      this.laboOrderLineService.onChangeProduct(val).subscribe((result) => {
        this.lineForm.get("priceUnit").patchValue(result.priceUnit);
        this.computePriceSubtotal();
      });
    }
  }

  onChangeQuantity(value) {
    this.computePriceSubtotal();
  }

  onChangePriceUnit(value) {
    this.computePriceSubtotal();
  }

  computePriceSubtotal() {
    var quantity = this.lineForm.get("quantity").value || 0;
    var priceUnit = this.lineForm.get("priceUnit").value || 0;
    this.lineForm.get("priceSubtotal").patchValue(quantity * priceUnit);
  }

  onSave() {
    this.submitted = true;
    if (!this.lineForm.valid) {
      return;
    }

    var val = this.lineForm.value;
    val.productId = val.product.id;
    val.customerId = val.customer.id;
    val.supplierId = val.supplier.id;
    val.sentDate = val.sentDateObj
      ? this.intlService.formatDate(val.sentDateObj, "g", "en-US")
      : null;
    val.receivedDate = val.receivedDateObj
      ? this.intlService.formatDate(val.receivedDateObj, "g", "en-US")
      : null;
    val.warrantyPeriod = val.warrantyPeriodObj
      ? this.intlService.formatDate(val.warrantyPeriodObj, "g", "en-US")
      : null;

    if (this.id) {
      this.laboOrderLineService.update(this.id, val).subscribe((result) => {
        this.activeModal.close(true);
      });
    } else {
      this.laboOrderLineService.create(val).subscribe((result) => {
        this.activeModal.close(result);
      });
    }
  }

  onCancel() {
    this.submitted = false;
    this.activeModal.dismiss();
  }
}
