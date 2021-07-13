import { NotifyService } from 'src/app/shared/services/notify.service';
import { Component, OnInit, ViewChild, AfterViewInit, ElementRef } from '@angular/core';
import { FormGroup, FormBuilder, FormArray, Validators, FormControl, AbstractControl } from '@angular/forms';
import { ProductBasic2, ProductService, ProductPaged } from 'src/app/products/product.service';
import { ActivatedRoute, Router } from '@angular/router';
import { PurchaseOrderService, PurchaseOrderDisplay, PurchaseOrderLineDisplay } from '../purchase-order.service';
import { PartnerSimple, PartnerPaged } from 'src/app/partners/partner-simple';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap, distinctUntilChanged, mergeMap } from 'rxjs/operators';
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
import { PurchaseOrderAlmostOutDialogComponent } from '../purchase-order-almost-out-dialog/purchase-order-almost-out-dialog.component';
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
  orderId: string;
  purchaseOrderLineOnChangeProductResult: PurchaseOrderLineOnChangeProductResult = new PurchaseOrderLineOnChangeProductResult();
  purchaseOrder: any;
  hasDefined = false;
  filteredPartners: PartnerSimple[];

  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;
  @ViewChild('journalCbx', { static: true }) journalCbx: ComboBoxComponent;
  @ViewChild('searchInput', { static: true }) searchInput: ElementRef;

  productList: ProductSimple[] = [];
  filteredJournals: any = [];
  productSearch: string;
  searchUpdate = new Subject<string>();
  productSelectedIndex = 0;
  uomByProduct: { [id: string]: UoMDisplay[] } = {};
  listType: string = 'medicine,product'
  submitted = false;
  amountTotal = 0;
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
    private notifyService: NotifyService,
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
      journal: null,
      amountPayment: [0, Validators.required],
      notes: null,
      orderLines: this.fb.array([]),
    });

    this.id = this.route.snapshot.paramMap.get('id');
    this.type = this.route.snapshot.queryParamMap.get('type');
    this.orderId = this.route.snapshot.queryParamMap.get('orderId');

    this.loadRecord();

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
    this.loadProductList();

    this.authService.getGroups().subscribe((result: any) => {
      this.permissionService.define(result);
      this.hasDefined = this.permissionService.hasOneDefined(['product.group_uom']);

    });
  }



  loadRecord() {
    this.loadDataApi().subscribe((result: any) => {
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

  loadDataApi() {
    if (this.id) {
      return this.purchaseOrderService.get(this.id);
    }
    else if (this.orderId && this.type == "refund") {
      return this.purchaseOrderService.getRefundByOrder(this.orderId);
    } else {
      return this.purchaseOrderService.defaultGet({ type: this.type });
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

  loadProductList() {
    var val = new ProductPaged();
    val.limit = 20;
    val.offset = 0;
    val.purchaseOK = true;
    val.type = 'product';
    val.type2 = this.listType;
    this.productService
      .autocomplete2(val).subscribe(
        (res) => {
          this.productList = res;
        },
        (err) => {
          console.log(err);
        }
      );
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
    this.countAmountTotal();
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

  countAmountTotal() {
    this.amountTotal = this.orderLines.value.reduce((total, cur) => {
      return total + cur.priceUnit * (1 - cur.discount / 100) * cur.productQty;
    }, 0);
    this.onChangeAmountTotal();
  }

  get getAmountTotal() {
    return this.orderLines.value.reduce((total, cur) => {
      return total + cur.priceUnit * (1 - cur.discount / 100) * cur.productQty;
    }, 0);
  }


  onSaveConfirm() {
    var index = _.findIndex(this.orderLines.controls, o => {
      return o.get('productQty').value == null || o.get('priceUnit').value == null;
    });
    if (index !== -1) {
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
      this.purchaseOrderService.update(this.id, data)
        .pipe(
          mergeMap(() => {
            return this.purchaseOrderService.buttonConfirm([this.id]);
          })
        )
        .subscribe(() => {
          this.notifyService.notify('success', 'Xác nhận thành công');
          this.loadRecord();
        });
    } else {
      this.purchaseOrderService.create(data)
        .pipe(
          mergeMap((rs: any) => {
            this.id = rs.id;
            return this.purchaseOrderService.buttonConfirm([rs.id]);
          })
        )
        .subscribe(rs => {
          this.notifyService.notify('success', 'Xác nhận thành công');
          this.router.navigate(['/purchase/orders/edit/' + this.id]);
        });
    }
  }

  onChangeAmountPayment(value) {
    var amountTotal = this.getAmountTotal;
    if (value > amountTotal) {
      this.f.amountPayment.setValue(amountTotal);
    }
  }

  onChangeAmountTotal() {
    var amountPayment = this.f.amountPayment.value || 0;
    var amountTotal = this.getAmountTotal;
    if (amountPayment > amountTotal) {
      this.f.amountPayment.setValue(amountTotal);
    }
  }

  getPicking(pickingid) {
    if (this.type == 'order') {
      this.router.navigate(['/stock/incoming-pickings/edit/' + pickingid]);
    } else {
      this.router.navigate(['/stock/outgoing-pickings/edit/' + pickingid]);
    }

  }


  buttonConfirm() {
    if (this.id) {
      this.purchaseOrderService.buttonConfirm([this.id]).subscribe(() => {
        this.notifyService.notify('success', 'Xác nhận thành công');
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

  actionRefund() {
    this.router.navigate(['/purchase/orders/create'], { queryParams: { type: 'refund', orderId: this.id } });
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

  selectProduct(item) {
    var product = item;
    const qty = item.qty ? item.qty : 1;
    // var index = _.findIndex(this.orderLines.controls, o => {
    //   return o.get('product').value.id == product.id;
    // });

    var val = new PurchaseOrderLineOnChangeProduct();
    val.productId = product.id;

    var productSimple = new ProductSimple();
    productSimple.id = product.id;
    productSimple.name = product.name;
    this.purchaseLineService.onChangeProduct(val).subscribe(result => {
      var group = this.fb.group({
        name: result.name,
        priceUnit: [result.priceUnit, Validators.required],
        productUOMId: result.productUOMId,
        productUOM: result.productUOM,
        product: productSimple,
        productId: product.id,
        priceSubtotal: null,
        productQty: [qty, Validators.required],
        discount: [0, Validators.required],
      });

      this.orderLines.push(group);
      this.focusLastRow();
      this.countAmountTotal();
    });
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
    //line.get('oldPriceUnit').patchValue(price);
    this.countAmountTotal();
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
    // val.productId = this.purchaseOrder.product ? this.purchaseOrder.prpduct.id : null;
    // val.productUOMId = this.purchaseOrder.productUOM ? this.purchaseOrder.productUOM.id : null;
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
      this.purchaseOrderService.create(data).subscribe((result: any) => {
        this.router.navigate(['/purchase/orders/edit/' + result.id]);
      });
    }
  }

  getPrint(id) {
    this.purchaseOrderService.getPrint(id).subscribe((data: any) => {
      this.printService.printHtml(data);
    });
  }

  purchaseOrderAlmostOut() {
    let modalRef = this.modalService.open(PurchaseOrderAlmostOutDialogComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Hàng sắp hết';
    modalRef.result.then((result) => {
      for (const item of result) {
        this.selectProduct(item)
      }
    }, () => {
    });
  }

}
