import { Component, OnInit, ViewChild, AfterViewInit, ElementRef } from '@angular/core';
import { FormGroup, FormBuilder, FormArray, Validators, FormControl, AbstractControl } from '@angular/forms';
import { ProductBasic2, ProductService, ProductPaged } from 'src/app/products/product.service';
import { ActivatedRoute, Router } from '@angular/router';
import { PurchaseOrderService, PurchaseOrderDisplay, PurchaseOrderLineDisplay } from '../purchase-order.service';
import { PartnerSimple, PartnerPaged } from 'src/app/partners/partner-simple';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap, distinctUntilChanged } from 'rxjs/operators';
import { PartnerService } from 'src/app/partners/partner.service';
import * as _ from 'lodash';
import { ProductSimple } from 'src/app/products/product-simple';
import { PurchaseOrderLineService, PurchaseOrderLineOnChangeProduct, PurchaseOrderLineOnChangeProductResult } from '../purchase-order-line.service';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { AuthService } from 'src/app/auth/auth.service';
import { PermissionService } from 'src/app/shared/permission.service';
import { UoMDisplay } from 'src/app/uoms/uom.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { SelectUomProductDialogComponent } from 'src/app/shared/select-uom-product-dialog/select-uom-product-dialog.component';
import { AccountPaymentService } from 'src/app/account-payments/account-payment.service';
import { AccountInvoiceRegisterPaymentDialogV2Component } from 'src/app/shared/account-invoice-register-payment-dialog-v2/account-invoice-register-payment-dialog-v2.component';
import { PrintService } from 'src/app/shared/services/print.service';
import { AccountJournalFilter, AccountJournalService } from 'src/app/account-journals/account-journal.service';
declare var $: any;

@Component({
  selector: 'app-purchase-order-create-update',
  templateUrl: './purchase-order-create-update.component.html',
  styleUrls: ['./purchase-order-create-update.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class PurchaseOrderCreateUpdateComponent implements OnInit {
  formGroup: FormGroup;
  id: string;
  type: string;
  purchaseOrderLineOnChangeProductResult: PurchaseOrderLineOnChangeProductResult = new PurchaseOrderLineOnChangeProductResult();
  purchaseOrder: PurchaseOrderDisplay = new PurchaseOrderDisplay();
  hasDefined = false;
  filteredPartners: PartnerSimple[];

  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;
  @ViewChild('journalCbx', { static: true }) journalCbx: ComboBoxComponent;
  @ViewChild('searchInput', { static: true }) searchInput: ElementRef;

  productList: ProductBasic2[] = [];
  filteredJournals: any = [];
  productSearch: string;
  searchUpdate = new Subject<string>();
  productSelectedIndex = 0;
  uomByProduct: { [id: string]: UoMDisplay[] } = {};
  listType: string = 'medicine'
  submitted = false;

  get f() { return this.formGroup.controls; }

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private route: ActivatedRoute,
    private purchaseOrderService: PurchaseOrderService,
    private partnerService: PartnerService,
    private purchaseLineService: PurchaseOrderLineService,
    private intlService: IntlService,
    private notificationService: NotificationService,
    private accountJournalService: AccountJournalService,
    private router: Router,
    private permissionService: PermissionService,
    private authService: AuthService,
    private modalService: NgbModal,
    private paymentService: AccountPaymentService,
    private printService: PrintService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      partner: [null, Validators.required],
      dateOrderObj: [null, Validators.required],
      // pickingTypeId: null,
      journal: null,
      amountPayment: [0, Validators.required],
      notes: null,
      orderLines: this.fb.array([]),
    });

    this.id = this.route.snapshot.paramMap.get('id');
    this.type = this.route.snapshot.queryParamMap.get('type');

    if (this.id) {
      this.loadRecord();
    } else {
      this.purchaseOrderService.defaultGet({ type: this.type }).subscribe(result => {
        this.purchaseOrder = result;

        this.formGroup.patchValue(result);
        let dateOrder = new Date(result.dateOrder);
        this.formGroup.get('dateOrderObj').patchValue(dateOrder);

        const control = this.formGroup.get('orderLines') as FormArray;
        control.clear();
        result.orderLines.forEach(line => {
          var g = this.fb.group(line);
          control.push(g);
        });
      });
    }

    this.partnerCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.partnerCbx.loading = true)),
      switchMap(value => this.searchPartners(value))
    ).subscribe(result => {
      this.filteredPartners = result;
      this.partnerCbx.loading = false;
    });

    this.loadPartners();
    this.loadFilteredJournals();
    this.loadProductList('medicine');

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadProductList(this.listType);
      });

    $('#productSearchInput').focus(function () {
      $(this).select();
    });

    this.authService.getGroups().subscribe((result: any) => {
      this.permissionService.define(result);
      this.hasDefined = this.permissionService.hasOneDefined(['product.group_uom']);
    });
  }

  loadRecord() {
    if (this.id) {
      this.purchaseOrderService.get(this.id).subscribe(result => {
        this.purchaseOrder = result;
        this.formGroup.patchValue(this.purchaseOrder);
        let dateOrder = new Date(result.dateOrder);
        this.formGroup.get('dateOrderObj').patchValue(dateOrder);

        let control = this.formGroup.get('orderLines') as FormArray;
        control.clear();
        result.orderLines.forEach(line => {
          var g = this.fb.group(line);
          control.push(g);
        });
      });
    }
  }

  loadPartners() {
    this.searchPartners().subscribe(result => {
      this.filteredPartners = _.unionBy(this.filteredPartners, result, 'id');
    });
  }

  actionRegisterPayment() {
    if (this.id) {
      this.paymentService.purchaseDefaultGet([this.id]).subscribe(rs2 => {
        let modalRef = this.modalService.open(AccountInvoiceRegisterPaymentDialogV2Component, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
        modalRef.componentInstance.title = 'Thanh toán';
        modalRef.componentInstance.defaultVal = rs2;
        modalRef.result.then(() => {
          this.notificationService.show({
            content: 'Thanh toán thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });

          this.loadRecord();
        }, () => {
        });
      })
    }
  }

  searchPartners(filter?: string) {
    var val = new PartnerPaged();
    val.supplier = true;
    val.search = filter;
    val.active = true;
    return this.partnerService.getAutocompleteSimple(val);
  }

  loadProductList(type?: string) {
    this.listType = type;
    var val = new ProductPaged();
    val.limit = 10;
    val.offset = 0;
    val.purchaseOK = true;
    val.search = this.productSearch || '';
    val.type2 = this.listType;
    val.type = 'product';
    this.productService.getPaged(val).subscribe(res => {
      this.productList = res.items;
    }, err => {
    });
  }

  loadFilteredJournals() {
    var val = new AccountJournalFilter();
    val.type = "bank,cash";
    val.companyId = this.authService.userInfo.companyId;
    this.accountJournalService.autocomplete(val).subscribe((res) => {
      this.filteredJournals = _.unionBy(this.filteredJournals, res, 'id');
    },
      (error) => {
        console.log(error);
      }
    );
  }

  get orderLines() {
    return this.formGroup.get('orderLines') as FormArray;
  }

  removeOrderLine(index) {
    this.orderLines.removeAt(index);
  }

  removeAllLine() {
    this.orderLines.clear();
  }

  duplicateLine(index, line: FormControl) {
    var valueCopy = Object.assign({}, line.value);
    delete valueCopy['id'];
    var copy = this.fb.group(valueCopy);
    this.orderLines.insert(index, copy);
  }

  get getAmountTotal() {
    var total = 0;
    this.orderLines.controls.forEach(c => {
      total += this.computeLinePriceSubtotal(c);
    });
    return total;
  }

  onSaveConfirm() {
    var index = _.findIndex(this.orderLines.controls, o => {
      return o.get('productQty').value == null || o.get('priceUnit').value == null;
    });
    if (index !== -1) {
      this.notificationService.show({
        content: 'Vui lòng nhập số lượng và đơn giá',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'warning', icon: true }
      });
      return false;
    }

    this.submitted = true;

    if (!this.formGroup.valid) {
      return false;
    }

    var val = this.formGroup.value;
    val.dateOrder = this.intlService.formatDate(val.dateOrderObj, 'yyyy-MM-ddTHH:mm:ss');
    val.partnerId = val.partner.id;
    var data = Object.assign(this.purchaseOrder, val);
    this.purchaseOrderService.create(val).subscribe((result:any) => {
      this.purchaseOrderService.buttonConfirm([result.id]).subscribe(() => {
        this.router.navigate(['/purchase/orders/edit/' + result.id]);
      }, () => {
        this.router.navigate(['/purchase/orders/edit/' + result.id]);
      });
    });
  }


  buttonConfirm() {
    if (this.id) {
      this.purchaseOrderService.buttonConfirm([this.id]).subscribe(() => {
        this.loadRecord();
      });
    }
  }

  buttonCancel() {
    if (this.id) {
      this.purchaseOrderService.buttonCancel([this.id]).subscribe(() => {
        this.loadRecord();
      });
    }
  }

  createNew() {
    this.router.navigate(['/purchase/orders/create'], { queryParams: { type: this.purchaseOrder.type } });
  }

  focusProductSearchInput() {
    $('#productSearchInput').focus();
  }

  onProductSearchKeydown(e) {
    if (e.keyCode == 40) {
      this.productSelectedIndex += 1;
      if (this.productSelectedIndex > this.productList.length - 1) {
        this.productSelectedIndex = 0;
      }
    } else if (e.keyCode == 38) {
      this.productSelectedIndex -= 1;
      if (this.productSelectedIndex < 0) {
        this.productSelectedIndex = this.productList.length - 1;
      }
    } else if (e.keyCode == 13) {
      this.selectProduct(this.productSelectedIndex);
    } else {
      this.productSelectedIndex = 0;
    }
  }

  selectProduct(i) {
    var product = this.productList[i];
    var index = _.findIndex(this.orderLines.controls, o => {
      return o.get('product').value.id == product.id;
    });

    if (index !== -1) {
      var control = this.orderLines.controls[index];
      control.patchValue({ productQty: control.get('productQty').value + 1 });
    } else {
      var val = new PurchaseOrderLineOnChangeProduct();
      val.productId = product.id;

      var productSimple = new ProductSimple();
      productSimple.id = product.id;
      productSimple.name = product.name;
      this.purchaseLineService.onChangeProduct(val).subscribe(result => {
        var group = this.fb.group({
          name: result.name,
          priceUnit: [result.priceUnit,Validators.required],
          productUOMId: result.productUOMId,
          productUOM: result.productUOM,
          product: productSimple,
          productId: product.id,
          priceSubtotal: null,
          productQty: [1,Validators.required],
          discount: 0,
        });

        this.orderLines.push(group);
        this.focusLastRow();
      });
    }
  }

  changeUoM(line: AbstractControl) {
    var product = line.get('product').value;
    let modalRef = this.modalService.open(SelectUomProductDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', scrollable: true, backdrop: 'static', keyboard: false });
    modalRef.componentInstance.title = 'Chọn đơn vị';
    modalRef.componentInstance.productId = product.id;
    modalRef.result.then((res: any) => {
      var uom = line.get('productUOM').value;
      if (uom.id != res.id) {
        line.get('productUOM').setValue(res);
        this.purchaseLineService.onChangeUOM({ productId: product.id, productUOMId: res.id }).subscribe((result: any) => {
          line.patchValue(result);
        });
      }
    }, () => {
    });
  }

  changePrice(price, line: AbstractControl) {
    line.get('oldPriceUnit').patchValue(price)
  }

  focusLastRow() {
    setTimeout(() => {
      var $lastTr = $('tr:last', $('#table_details tbody'));
      $('input:first', $lastTr).focus();
    }, 70);
  }

  computeLinePriceSubtotal(line: AbstractControl) {
    var priceUnit = line.get('priceUnit').value || 0;
    var productQty = line.get('productQty').value || 0;
    var discount = line.get('discount').value || 0;
    return priceUnit * (1 - discount / 100) * productQty;
  }

  onSave() {
    var index = _.findIndex(this.orderLines.controls, o => {
      return o.get('productQty').value == null || o.get('priceUnit').value == null;
    });
    if (index !== -1) {
      // this.notificationService.show({
      //   content: 'Vui lòng nhập số lượng và đơn giá',
      //   hideAfter: 3000,
      //   position: { horizontal: 'center', vertical: 'top' },
      //   animation: { type: 'fade', duration: 400 },
      //   type: { style: 'warning', icon: true }
      // });
      return false;
    }

    this.submitted = true;

    if (!this.formGroup.valid) {
      return false;
    }

    var val = this.formGroup.value;
    val.dateOrder = this.intlService.formatDate(val.dateOrderObj, 'yyyy-MM-ddTHH:mm:ss');
    val.partnerId = val.partner.id;
    val.journalId = val.journal.id;
    var data = Object.assign(this.purchaseOrder, val);
    if (this.id) {
      this.purchaseOrderService.update(this.id, data).subscribe(() => {
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
      this.purchaseOrderService.create(data).subscribe((result:any) => {
        this.router.navigate(['/purchase/orders/edit/' + result.id]);
      });
    }
  }

  printPhieu(id: string) {
    this.purchaseOrderService.getPrint(id).subscribe((data: any) => {
      this.printService.printHtml(data);
    });
  }

}
