import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import * as _ from 'lodash';
import { of } from 'rxjs';
import { debounceTime, mergeMap, switchMap, tap } from 'rxjs/operators';
import { PaymentInfoContent } from 'src/app/account-invoices/account-invoice.service';
import { AccountPaymentBasic, AccountPaymentService } from 'src/app/account-payments/account-payment.service';
import { AccountRegisterPaymentService } from 'src/app/account-payments/account-register-payment.service';
import { CardCardPaged, CardCardService } from 'src/app/card-cards/card-card.service';
import { SaleOrderLineService } from 'src/app/core/services/sale-order-line.service';
import { DiscountDefault, SaleOrderPaged, SaleOrderService } from 'src/app/core/services/sale-order.service';
import { DotKhamService } from 'src/app/dot-khams/dot-kham.service';
import { DotKhamBasic } from 'src/app/dot-khams/dot-khams';
import { EmployeeBasic, EmployeePaged } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { LaboOrderBasic, LaboOrderPaged, LaboOrderService } from 'src/app/labo-orders/labo-order.service';
import { ProductPriceListBasic, ProductPricelistPaged } from 'src/app/price-list/price-list';
import { PriceListService } from 'src/app/price-list/price-list.service';
import { LaboOrderCuDialogComponent } from 'src/app/sale-orders/labo-order-cu-dialog/labo-order-cu-dialog.component';
import { SaleOrderApplyCouponDialogComponent } from 'src/app/sale-orders/sale-order-apply-coupon-dialog/sale-order-apply-coupon-dialog.component';
import { SaleOrderApplyServiceCardsDialogComponent } from 'src/app/sale-orders/sale-order-apply-service-cards-dialog/sale-order-apply-service-cards-dialog.component';
import { SaleOrderBasic } from 'src/app/sale-orders/sale-order-basic';
import { SaleOrderCreateDotKhamDialogComponent } from 'src/app/sale-orders/sale-order-create-dot-kham-dialog/sale-order-create-dot-kham-dialog.component';
import { SaleOrderDisplay } from 'src/app/sale-orders/sale-order-display';
import { SaleOrderLineLaboOrdersDialogComponent } from 'src/app/sale-orders/sale-order-line-labo-orders-dialog/sale-order-line-labo-orders-dialog.component';
import { SaleOrderPaymentDialogComponent } from 'src/app/sale-orders/sale-order-payment-dialog/sale-order-payment-dialog.component';
import { AccountPaymentPrintComponent } from 'src/app/shared/account-payment-print/account-payment-print.component';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { DotKhamCreateUpdateDialogComponent } from 'src/app/shared/dot-kham-create-update-dialog/dot-kham-create-update-dialog.component';
import { PartnerCustomerCuDialogComponent } from 'src/app/shared/partner-customer-cu-dialog/partner-customer-cu-dialog.component';
import { SaleOrderLineDialogComponent } from 'src/app/shared/sale-order-line-dialog/sale-order-line-dialog.component';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';
import { UserSimple } from 'src/app/users/user-simple';
import { UserPaged, UserService } from 'src/app/users/user.service';
import { PartnerSearchDialogComponent } from '../partner-search-dialog/partner-search-dialog.component';
import { PartnerPaged, PartnerSimple } from '../partner-simple';
import { PartnerService } from '../partner.service';

@Component({
  selector: 'app-partner-customer-treatment-history',
  templateUrl: './partner-customer-treatment-history.component.html',
  styleUrls: ['./partner-customer-treatment-history.component.css']
})
export class PartnerCustomerTreatmentHistoryComponent implements OnInit {
  partnerId: string;
  date: Date = new Date();
  saleOrderId: string;
  limit = 20;
  skip = 0;
  isReload: boolean;
  listSaleOrder: SaleOrderBasic[] = [];
  formGroup: FormGroup;
  filteredPartners: PartnerSimple[];
  filteredUsers: UserSimple[];
  filteredPricelists: ProductPriceListBasic[];
  discountDefault: DiscountDefault;

  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;
  @ViewChild('userCbx', { static: true }) userCbx: ComboBoxComponent;
  @ViewChild('pricelistCbx', { static: true }) pricelistCbx: ComboBoxComponent;
  @ViewChild(AccountPaymentPrintComponent, { static: true }) accountPaymentPrintComponent: AccountPaymentPrintComponent;
  @ViewChild('employeeCbx', { static: false }) employeeCbx: ComboBoxComponent;

  saleOrder: SaleOrderDisplay = new SaleOrderDisplay();
  saleOrderPrint: any;
  saleOrderLine: any;
  payments: AccountPaymentBasic[] = [];
  paymentsInfo: PaymentInfoContent[] = [];
  filteredEmployees: EmployeeBasic[] = [];

  searchCardBarcode: string;
  partnerSend: any;
  type: string;

  constructor(
    private activeRoute: ActivatedRoute,
    private fb: FormBuilder, private partnerService: PartnerService,
    private userService: UserService, private route: ActivatedRoute, private saleOrderService: SaleOrderService,
    private saleOrderLineService: SaleOrderLineService, private intlService: IntlService, private modalService: NgbModal,
    private router: Router, private notificationService: NotificationService, private cardCardService: CardCardService,
    private pricelistService: PriceListService, private errorService: AppSharedShowErrorService,
    private registerPaymentService: AccountRegisterPaymentService, private paymentService: AccountPaymentService,
    private laboOrderService: LaboOrderService, private dotKhamService: DotKhamService, private employeeService: EmployeeService
  ) { }

  ngOnInit() {
    this.partnerId = this.activeRoute.parent.snapshot.paramMap.get('id');
    this.formGroup = this.fb.group({
      partner: [null, Validators.required],
      dateOrderObj: [new Date, Validators.required],
      orderLines: this.fb.array([]),
      companyId: null,
      amountTotal: 0,
      state: null,
      residual: null,
      card: null,
      pricelist: [null, Validators.required],
    });
    this.loadSaleOrder();

    this.loadEmployees();

    this.employeeCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.employeeCbx.loading = true)),
      switchMap(value => this.searchEmployees(value))
    ).subscribe(result => {
      this.filteredEmployees = result.items;
      this.employeeCbx.loading = false;
    });

    // this.getAccountPaymentReconcicles();
    // this.loadLaboOrderList();
    this.loadPayments();
    // this.loadPricelists();

  }

  changeSaleOrder(event) {
    if (event) {
      this.saleOrderId = event;
    } else {
      this.saleOrderId = null;
    }
    this.routeActive()
  }

  loadSaleOrder() {
    var val = new SaleOrderPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.partnerId = this.partnerId;

    this.saleOrderService.getPaged(val).subscribe(res => {
      this.listSaleOrder = res.items;
      console.log(res.items);
      
      if (this.listSaleOrder && this.listSaleOrder.length) {
        this.saleOrderId = this.listSaleOrder[0].id;
      }
      this.routeActive();
    }, err => {
      console.log(err);
    })
  }

  changeIsReload(event) {
    if (event) {
      this.isReload = event;
    } else {
      this.isReload = false;
    }
  }

  loadEmployees() {
    this.searchEmployees().subscribe(result => {
      this.filteredEmployees = _.unionBy(this.filteredEmployees, result.items, 'id');
    });
  }


  searchEmployees(filter?: string) {
    var val = new EmployeePaged();
    val.search = filter || '';
    val.isDoctor = true;
    return this.employeeService.getEmployeePaged(val);
  }

  routeActive() {
    this.route.queryParamMap.pipe(
      switchMap((params: ParamMap) => {
        if (this.saleOrderId) {
          return this.saleOrderService.get(this.saleOrderId);
        } else {
          return this.saleOrderService.defaultGet({ partnerId: this.partnerId || '' });
        }
      })).subscribe(result => {
        this.saleOrder = result;
        this.partnerSend = result.partner;
        this.formGroup.patchValue(result);
        let dateOrder = new Date(result.dateOrder);
        this.formGroup.get('dateOrderObj').patchValue(dateOrder);

        if (result.user) {
          this.filteredUsers = _.unionBy(this.filteredUsers, [result.user], 'id');
        }

        if (result.partner) {
          this.filteredPartners = _.unionBy(this.filteredPartners, [result.partner], 'id');
          if (!this.saleOrderId) {
            this.onChangePartner(result.partner);
          }
        }

        // if (result.pricelist) {
        //   this.filteredPricelists = _.unionBy(this.filteredPricelists, [result.pricelist], 'id');
        // }

        const control = this.formGroup.get('orderLines') as FormArray;
        control.clear();
        result.orderLines.forEach(line => {
          var g = this.fb.group(line);
          g.setControl('teeth', this.fb.array(line.teeth));
          control.push(g);
        });

        this.formGroup.markAsPristine();
      });
  }

  quickCreateCustomer() {
    let modalRef = this.modalService.open(PartnerCustomerCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm khách hàng';

    modalRef.result.then(result => {
      var p = new PartnerSimple();
      p.id = result.id;
      p.name = result.name;
      p.displayName = result.displayName;
      this.formGroup.get('partner').patchValue(p);
      this.filteredPartners = _.unionBy(this.filteredPartners, [p], 'id');
      this.onChangePartner(p);
    }, () => {
    });
  }

  searchCustomerDialog() {
    let modalRef = this.modalService.open(PartnerSearchDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Tìm khách hàng';
    modalRef.componentInstance.domain = { customer: true };

    modalRef.result.then(result => {
      if (result.length) {
        var p = result[0].dataItem;
        this.formGroup.get('partner').patchValue(p);
        this.filteredPartners = _.unionBy(this.filteredPartners, [p], 'id');
        this.onChangePartner(p);
      }
    }, () => {
    });
  }





  get cardValue() {
    return this.formGroup.get('card').value;
  }

  loadPartners() {
    this.searchPartners().subscribe(result => {
      this.filteredPartners = _.unionBy(this.filteredPartners, result, 'id');
    });
  }

  loadPricelists() {
    this.searchPricelists().subscribe(result => {
      this.filteredPricelists = _.unionBy(this.filteredPricelists, result.items, 'id');
    });
  }

  searchCard() {
    var barcode = this.searchCardBarcode;
    var val = new CardCardPaged();
    val.limit = 1;
    val.barcode = barcode;
    val.state = "in_use";
    val.isExpired = false;
    this.cardCardService.getPaged(val).subscribe(result => {
      if (result.items.length) {
        this.formGroup.get('card').patchValue(result.items[0]);
        this.searchCardBarcode = '';
      } else {
        this.notificationService.show({
          content: 'Không tìm thấy thẻ thành viên hoặc thẻ thành viên không còn khả dụng.',
          hideAfter: 3000,
          position: { horizontal: 'right', vertical: 'bottom' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'error', icon: true }
        });
      }
    });
  }

  showApplyCardDialog() {
    var partner = this.formGroup.get('partner').value;
    if (!partner) {
      this.notificationService.show({
        content: 'Vui lòng chọn khách hàng',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });

      return false;
    }

    if (this.saleOrderId) {
      if (this.formGroup.dirty) {
        this.saveRecord().subscribe(() => {
          let modalRef = this.modalService.open(SaleOrderApplyServiceCardsDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
          modalRef.componentInstance.orderId = this.saleOrderId;
          modalRef.componentInstance.amountTotal = this.formGroup.get('amountTotal').value;
          modalRef.result.then(() => {
            this.loadRecord();
          }, () => {
          });
        })
      } else {
        let modalRef = this.modalService.open(SaleOrderApplyServiceCardsDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
        modalRef.componentInstance.orderId = this.saleOrderId;
        modalRef.componentInstance.amountTotal = this.formGroup.get('amountTotal').value;
        modalRef.result.then(() => {
          this.loadRecord();
        }, () => {
        });
      }
    } else {
      if (!this.formGroup.valid) {
        return false;
      }

      this.createRecord().subscribe(result => {
        this.router.navigate(['/sale-orders/form'], {
          queryParams: {
            id: result.id
          },
        });

        let modalRef = this.modalService.open(SaleOrderApplyServiceCardsDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
        modalRef.componentInstance.orderId = result.id;
        modalRef.componentInstance.amountTotal = this.formGroup.get('amountTotal').value;
        modalRef.result.then(() => {
          this.loadRecord();
        }, () => {
        });
      });
    }
  }

  lineEditable(line: FormControl) {
    if (line.get('isRewardLine')) {
      return !line.get('isRewardLine').value;
    }

    return true;
  }

  showApplyCouponDialog() {
    if (this.saleOrderId) {
      if (this.formGroup.dirty) {
        this.saveRecord().subscribe(() => {
          let modalRef = this.modalService.open(SaleOrderApplyCouponDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
          modalRef.componentInstance.orderId = this.saleOrderId;
          modalRef.result.then(() => {
            this.loadRecord();
          }, () => {
          });
        })
      } else {
        let modalRef = this.modalService.open(SaleOrderApplyCouponDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
        modalRef.componentInstance.orderId = this.saleOrderId;
        modalRef.result.then(() => {
          this.loadRecord();
        }, () => {
        });
      }
    } else {
      if (!this.formGroup.valid) {
        return false;
      }

      this.createRecord().subscribe(result => {
        this.router.navigate(['/sale-orders/form'], {
          queryParams: {
            id: result.id
          },
        });

        let modalRef = this.modalService.open(SaleOrderApplyCouponDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
        modalRef.componentInstance.orderId = result.id;
        modalRef.result.then(() => {
          this.loadRecord();
        }, () => {
        });
      });
    }
  }

  getCouponLines() {
    var lines = this.orderLines.value;
    var list = [];
    for (var i = 0; i < lines.length; i++) {
      var line = lines[i];
      if (line.couponId) {
        list.push(line);
      }
    }

    return list;
  }

  getPromotionLines() {
    var lines = this.orderLines.value;
    var list = [];
    for (var i = 0; i < lines.length; i++) {
      var line = lines[i];
      if (line.promotionId) {
        list.push(line);
      }
    }

    return list;
  }

  getTotalDiscountCoupon() {
    var total = 0;
    var lines = this.getCouponLines();
    for (var i = 0; i < lines.length; i++) {
      var line = lines[i];
      total = total + line.priceSubTotal;
    }

    return total;
  }

  getTotalDiscountPromotion() {
    var total = 0;
    var lines = this.getPromotionLines();
    for (var i = 0; i < lines.length; i++) {
      var line = lines[i];
      total = total + line.priceSubTotal;
    }

    return total;
  }

  isCouponLine(line: FormControl) {
    var c = line.get('couponId');
    if (c) {
      return c.value != null;
    }
    return false;
  }

  isPromotionLine(line: FormControl) {
    var c = line.get('promotionId');
    if (c) {
      return c.value != null;
    }
    return false;
  }

  deleteCouponLine(line) {
    var index = _.findIndex(this.orderLines.controls, o => o.get('id').value == line.id);
    if (index != -1) {
      this.orderLines.removeAt(index);

      this.saveRecord().subscribe(() => {
        this.loadRecord();
      }, () => {
        this.loadRecord();
      });
    }
  }

  applyPromotion() {
    if (this.saleOrderId) {
      if (this.formGroup.dirty) {
        this.saveRecord().subscribe(() => {
          this.saleOrderService.applyPromotion(this.saleOrderId).subscribe(() => {
            this.loadRecord();
          }, (error) => {
            this.errorService.show(error);
          });
        });
      } else {
        this.saleOrderService.applyPromotion(this.saleOrderId).subscribe(() => {
          this.loadRecord();
        }, (error) => {
          this.errorService.show(error);
        });
      }
    } else {
      this.createRecord().subscribe((result) => {
        this.router.navigate(['/sale-orders/form'], {
          queryParams: {
            id: result.id
          },
        });

        this.saleOrderService.applyPromotion(result.id).subscribe(() => {
          this.loadRecord();
        });
      });
    }
  }

  onApplyDiscount(val: any) {
    if (this.saleOrderId) {
      this.discountDefault = val;
      this.discountDefault.saleOrderId = this.saleOrderId;
      this.saleOrderService.applyDiscountDefault(this.discountDefault).subscribe(() => {
        this.loadRecord();
      }, (error) => {
        this.errorService.show(error);
      });
    }
    else {
      if (!this.formGroup.valid) {
        return false;
      }

      this.createRecord().subscribe(result => {
        this.router.navigate(['/sale-orders/form'], {
          queryParams: {
            id: result.id
          },
        });

        val.saleOrderId = result.id;
        this.saleOrderService.applyDiscountDefault(val).subscribe(() => {
          this.loadRecord();
        }, (error) => {
          this.errorService.show(error);
        });
      });
    }
  }

  createRecord() {
    var val = this.formGroup.value;
    val.dateOrder = this.intlService.formatDate(val.dateOrderObj, 'yyyy-MM-ddTHH:mm:ss');
    val.partnerId = val.partner.id;
    val.pricelistId = val.pricelist.id;
    val.userId = val.user ? val.user.id : null;
    val.cardId = val.card ? val.card.id : null;
    val.orderLines.forEach(line => {
      line.toothIds = line.teeth.map(x => x.id);
    });
    return this.saleOrderService.create(val);
  }

  getDiscountNumber(line: FormGroup) {
    var discountType = line.get('discountType') ? line.get('discountType').value : 'percentage';
    if (discountType == 'fixed') {
      return line.get('discountFixed').value;
    } else {
      return line.get('discount').value;
    }
  }

  getDiscountTypeDisplay(line: FormGroup) {
    var discountType = line.get('discountType') ? line.get('discountType').value : 'percentage';
    if (discountType == 'fixed') {
      return "";
    } else {
      return '%';
    }
  }

  saveRecord() {
    var val = this.formGroup.value;
    val.dateOrder = this.intlService.formatDate(val.dateOrderObj, 'yyyy-MM-ddTHH:mm:ss');
    val.partnerId = val.partner.id;
    val.pricelistId = val.pricelist.id;
    val.userId = val.user ? val.user.id : null;
    val.cardId = val.card ? val.card.id : null;
    val.orderLines.forEach(line => {
      line.toothIds = line.teeth.map(x => x.id);
    });
    return this.saleOrderService.update(this.saleOrderId, val);
  }

  removeCard() {
    this.formGroup.get('card').patchValue(null);
  }

  loadUsers() {
    this.searchUsers().subscribe(result => {
      this.filteredUsers = _.unionBy(this.filteredUsers, result, 'id');
    });
  }

  searchUsers(filter?: string) {
    var val = new UserPaged();
    val.search = filter;
    return this.userService.autocompleteSimple(val);
  }

  searchPricelists(filter?: string) {
    var val = new ProductPricelistPaged();
    val.search = filter || '';
    return this.pricelistService.loadPriceListList(val);
  }

  searchPartners(filter?: string) {
    var val = new PartnerPaged();
    val.customer = true;
    val.search = filter;
    return this.partnerService.getAutocompleteSimple(val);
  }

  actionConfirm() {
    if (!this.formGroup.valid) {
      return false;
    }

    if (!this.saleOrderId) {
      return false;
    }

    of(this.formGroup.dirty)
      .pipe(
        mergeMap(r => {
          if (r) {
            var val = this.getFormDataSave();
            return this.saleOrderService.update(this.saleOrderId, val);
          } else {
            return of(true);
          }
        }),
        mergeMap(() => {
          return this.saleOrderService.actionConfirm([this.saleOrderId]);
        }),
      )
      .subscribe(() => {
        this.loadRecord();
        this.loadSaleOrder();
      })
  }

  actionViewInvoice() {
    if (this.saleOrderId) {
      this.router.navigate(['/sale-orders/' + this.saleOrderId + '/invoices']);
    }
  }

  actionInvoiceCreateV2() {
    if (this.saleOrderId) {
      this.saleOrderService.actionInvoiceCreateV2(this.saleOrderId).subscribe(() => {
        this.notificationService.show({
          content: 'Cập nhật thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.loadRecord();
      });
    }
  }




  blurSave() {

  }

  printSaleOrder() {
    if (this.saleOrderId) {
      this.saleOrderService.getPrint(this.saleOrderId).subscribe((result: any) => {
        this.saleOrderPrint = result;
        setTimeout(() => {
          var printContents = document.getElementById('printSaleOrderDiv').innerHTML;
          var popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
          popupWin.document.open();
          popupWin.document.write(`
              <html>
                <head>
                  <title>Print tab</title>
                  <link rel="stylesheet" type="text/css" href="/assets/css/bootstrap.min.css" />
                  <link rel="stylesheet" type="text/css" href="/assets/css/print.css" />
                </head>
            <body onload="window.print();window.close()">${printContents}</body>
              </html>`
          );
          popupWin.document.close();
          this.saleOrderPrint = null;
        });
      });
    }
  }

  actionDone() {
    if (this.saleOrderId) {
      this.saleOrderService.actionDone([this.saleOrderId]).subscribe(() => {
        this.loadRecord();
      });
    }
  }

  actionUnlock() {
    if (this.saleOrderId) {
      this.saleOrderService.actionUnlock([this.saleOrderId]).subscribe(() => {
        this.loadRecord();
      });
    }
  }

  getFormDataSave() {
    var val = this.formGroup.value;
    val.dateOrder = this.intlService.formatDate(val.dateOrderObj, 'yyyy-MM-ddTHH:mm:ss');
    val.partnerId = val.partner.id;
    val.pricelistId = val.pricelist.id;
    val.userId = val.user ? val.user.id : null;
    val.cardId = val.card ? val.card.id : null;
    val.orderLines.forEach(line => {
      if (line.employee) {
        line.employeeId = line.employee.id;
      }
      if (line.teeth) {
        line.toothIds = line.teeth.map(x => x.id);
      }
    });
    return val;
  }

  onSaveConfirm() {
    if (!this.formGroup.valid) {
      return false;
    }
    var val = this.getFormDataSave();
    this.saleOrderService.create(val)
      .pipe(
        mergeMap(r => {
          this.saleOrderId = r.id;
          return this.saleOrderService.actionConfirm([r.id]);
        })
      )
      .subscribe(r => {
        this.router.navigate(['/sale-orders/form'], { queryParams: { id: this.saleOrderId } });
      });
  }

  checkPromotion(id) {
    this.saleOrderService.checkPromotion(id).subscribe(result => {
      if (result) {
        this.showConfirmApplyPromotion(id);
      } else {
      }
    });
  }

  showConfirmApplyPromotion(id) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Quên cập nhật khuyến mãi?';
    modalRef.componentInstance.body = 'Hệ thống phát hiện có chương trình khuyến mãi có thể áp dụng cho đơn hàng này, bạn có muốn áp dụng trước khi xác nhận không?';
    modalRef.result.then(() => {
      this.saleOrderService.applyPromotion(id).subscribe(() => {
        this.saleOrderService.actionConfirm([id]).subscribe(() => {
          this.loadRecord();
        });
      });
    }, () => {
      this.saleOrderService.actionConfirm([id]).subscribe(() => {
        this.loadRecord();
      });
    });
  }

  confirmOrder(id) {
    return this.saleOrderService.actionConfirm(id);
  }

  isTurnOnSalePromotion() {
    var groups = [];
    if (localStorage.getItem('groups')) {
      groups = JSON.parse(localStorage.getItem('groups'));
    }

    var arr = ['sale.group_sale_coupon_promotion'];
    var intersect = _.intersection(groups, arr);
    var res = intersect.length == arr.length;
    return res;
  }

  onSave() {
    if (!this.formGroup.valid) {
      return false;
    }
    var val = this.formGroup.value;
    val.dateOrder = this.intlService.formatDate(val.dateOrderObj, 'yyyy-MM-ddTHH:mm:ss');
    val.partnerId = val.partner.id;
    val.pricelistId = val.pricelist.id;
    val.cardId = val.card ? val.card.id : null;
    val.orderLines.forEach(line => {
      if (line.employee) {
        line.employeeId = line.employee.id;
      }
      line.toothIds = line.teeth.map(x => x.id);
    });
    if (this.saleOrderId) {
      this.saleOrderService.update(this.saleOrderId, val).subscribe(() => {
        this.notificationService.show({
          content: 'Lưu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.loadRecord();
      }, () => {
        this.loadRecord();
      });
    } else {
      this.saleOrderService.create(val).subscribe(result => {
        this.loadSaleOrder();
      });
    }
  }

  onChangePartner(value) {
    if (value) {
      this.saleOrderService.onChangePartner({ partnerId: value.id }).subscribe(result => {
        this.formGroup.patchValue(result);
      });
    }
  }

  loadRecord() {
    if (this.saleOrderId) {
      this.saleOrderService.get(this.saleOrderId).subscribe(result => {
        this.saleOrder = result;
        this.formGroup.patchValue(result);
        let dateOrder = new Date(result.dateOrder);
        this.formGroup.get('dateOrderObj').patchValue(dateOrder);

        if (result.partner) {
          this.filteredPartners = _.unionBy(this.filteredPartners, [result.partner], 'id');
        }

        let control = this.formGroup.get('orderLines') as FormArray;
        control.clear();
        result.orderLines.forEach(line => {
          var g = this.fb.group(line);
          g.setControl('teeth', this.fb.array(line.teeth));
          control.push(g);
        });

        this.formGroup.markAsPristine();
      });
    }
  }

  actionCancel() {
    if (this.saleOrderId) {
      let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = 'Hủy phiếu điều trị';
      modalRef.componentInstance.body = 'Bạn có chắc chắn muốn hủy?';
      modalRef.result.then(() => {
        this.saleOrderService.actionCancel([this.saleOrderId]).subscribe(() => {
          this.loadRecord();
        });
      }, () => {
      });
    }
  }

  get orderLines() {
    return this.formGroup.get('orderLines') as FormArray;
  }

  //Mở popup thêm dịch vụ cho phiếu điều trị (Component: SaleOrderLineDialogComponent)
  showAddLineModal() {
    let modalRef = this.modalService.open(SaleOrderLineDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm dịch vụ điều trị';
    modalRef.componentInstance.partnerId = this.partnerId;
    var pricelist = this.formGroup.get('pricelist').value;
    modalRef.componentInstance.pricelistId = pricelist ? pricelist.id : null;
    if (this.formGroup.get('state').value == "draft") {
      modalRef.componentInstance.showSaveACreate = true;
    }

    modalRef.result.then(result => {
      debugger
      for (let i = 0; i < result.length; i++) {
        let line = result[i] as any;
        line.teeth = this.fb.array(line.teeth);
        this.orderLines.push(this.fb.group(line));
        this.orderLines.markAsDirty();
        this.computeAmountTotal();
      }

      /// nếu saleorder.state = "sale" thì update saleOrder và update công nợ
      if (this.formGroup.get('state').value == "sale") {
        var val = this.getFormDataSave();
        this.saleOrderService.update(this.saleOrderId, val).subscribe(() => {
          this.notificationService.show({
            content: 'Lưu thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
          this.loadRecord();
        }, () => {
          this.loadRecord();
        });
      }
    }, () => {
    });


  }

  addLine(val) {
    // this.saleOrderLine = event;
    var res = this.fb.group(val);
    // line.teeth = this.fb.array(line.teeth);
    if (!this.orderLines.controls.some(x => x.value.productId === res.value.productId)) {
      this.orderLines.push(res);
    } else {
      var line = this.orderLines.controls.find(x => x.value.productId === res.value.productId);
      if (line) {
        line.value.productUOMQty += 1;
        line.patchValue(line.value);
      }
    }
    this.getPriceSubTotal();
    this.orderLines.markAsDirty();
    this.computeAmountTotal();
    if (this.formGroup.get('state').value == "sale") {
      var val = this.getFormDataSave();
      this.saleOrderService.update(this.saleOrderId, val).subscribe(() => {
        this.notificationService.show({
          content: 'Lưu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.loadRecord();
      }, () => {
        this.loadRecord();
      });
    }
    this.saleOrderLine = null;
  }

  updateTeeth(line) {
    var val = this.getFormDataSave();
    this.saleOrderService.update(this.saleOrderId, val).subscribe(() => {
      this.notificationService.show({
        content: 'Lưu thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
      this.loadRecord();
    }, () => {
      this.loadRecord();
    });
  }

  getPriceSubTotal() {
    this.orderLines.controls.forEach(line => {
      var discountType = line.get('discountType') ? line.get('discountType').value : 'percentage';
      var discountFixedValue = line.get('discountFixed') ? line.get('discountFixed').value : 0;
      var discountNumber = line.get('discount') ? line.get('discount').value : 0;
      var getquanTity = line.get('productUOMQty') ? line.get('productUOMQty').value : 1;
      var getamountPaid = line.get('amountPaid') ? line.get('amountPaid').value : 0;
      var priceUnit = line.get('priceUnit') ? line.get('priceUnit').value : 0;
      var price = priceUnit * getquanTity;

      var subtotal = discountType == 'percentage' ? price * (1 - discountNumber / 100) :
        Math.max(0, price - discountFixedValue);
      line.get('priceSubTotal').setValue(subtotal);
      var getResidual = subtotal - getamountPaid;
      line.get('amountResidual').setValue(getResidual);
    });

  }

  //Mở popup Sửa dịch vụ cho phiếu điều trị (Component: SaleOrderLineDialogComponent)
  editLine(line: FormGroup) {
    var partner = this.formGroup.get('partner').value;
    if (!partner) {
      alert('Vui lòng chọn khách hàng');
      return false;
    }

    let modalRef = this.modalService.open(SaleOrderLineDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa dịch vụ điều trị';
    modalRef.componentInstance.line = line.value;
    modalRef.componentInstance.partnerId = partner.Id;
    var pricelist = this.formGroup.get('pricelist').value;
    modalRef.componentInstance.pricelistId = pricelist ? pricelist.id : null;

    modalRef.result.then(result => {
      var a = result[0] as any;
      line.patchValue(a);
      line.setControl('teeth', this.fb.array(a.teeth || []));
      this.computeAmountTotal();
      this.orderLines.markAsDirty();

      /// nếu saleorder.state = "sale" thì update saleOrder và update công nợ
      if (this.formGroup.get('state').value == "sale") {
        var val = this.getFormDataSave();
        this.saleOrderService.update(this.saleOrderId, val).subscribe(() => {
          this.notificationService.show({
            content: 'Sửa thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
          this.loadRecord();
        }, () => {
          this.loadRecord();
        });
      }
    }, () => {
    });
  }

  lineTeeth(line: FormGroup) {
    var teeth = line.get('teeth').value as any[];
    return teeth.map(x => x.name).join(',');
  }

  deleteLine(index: number) {
    if (this.formGroup.get('state').value == "draft" || this.formGroup.get('state').value == "cancel") {
      this.orderLines.removeAt(index);
      this.computeAmountTotal();
      this.orderLines.markAsDirty();
    } else {
      this.notificationService.show({
        content: 'Chỉ có thể xóa dịch vụ khi phiếu điều trị ở trạng thái nháp hoặc hủy bỏ',
        hideAfter: 5000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });
    }
  }

  get getAmountTotal() {
    return this.formGroup.get('amountTotal').value;
  }

  get getState() {
    return this.formGroup.get('state').value;
  }

  get getResidual() {
    return this.formGroup.get('residual').value;
  }

  get getPartner() {
    return this.formGroup.get('partner').value;
  }

  get getAmountPaidTotal(){
    let total = 0;
    this.orderLines.controls.forEach(line => {
      total += line.get('amountPaid').value;
    });

    return total;
  }

  computeAmountTotal() {
    let total = 0;
    this.orderLines.controls.forEach(line => {
      console.log(total);
      total += line.get('priceSubTotal').value;
    });
    // this.computeResidual(total);
    this.formGroup.get('amountTotal').patchValue(total);
  }

  //Tính nợ theo số tổng
  computeResidual(total) {
    let diff = this.getAmountTotal - this.getResidual;
    let residual = total - diff;
    this.formGroup.get('residual').patchValue(residual);
  }

  actionSaleOrderPayment() {
    if (this.saleOrderId) {
      this.paymentService.saleDefaultGet([this.saleOrderId]).subscribe(rs2 => {
        let modalRef = this.modalService.open(SaleOrderPaymentDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
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
          this.loadPayments();
          this.loadSaleOrder();
        }, () => {
        });
      })
    }
  }

  // hủy dịch vụ
  cancelSaleOrderLine(line: FormGroup) {
    var idControl = line.get('id');
    if (idControl) {
      let modalRef = this.modalService.open(ConfirmDialogComponent, { size: "sm", windowClass: "o_technical_modal", keyboard: false, backdrop: "static", });
      modalRef.componentInstance.title = "Hủy Dịch vụ";
      modalRef.componentInstance.body = `Bạn có chắc chắn muốn hủy dịch vụ ${line.get('name').value}?`;

      modalRef.result.then(() => {
        this.saleOrderLineService.cancelOrderLine([idControl.value]).subscribe(() => {
          this.notificationService.show({
            content: 'hủy dịch vụ thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
          this.loadRecord();
        }, () => {
          this.loadRecord();
        });
      }, (err) => {
        console.log(err);
      });
    }
  }


  loadPayments() {
    if (this.saleOrderId) {
      this.saleOrderService.getPayments(this.saleOrderId).subscribe(result => {
        this.paymentsInfo = result;
      });
    }
  }

  deletePayment(payment) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa thanh toán';
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.paymentService.unlink([payment.accountPaymentId]).subscribe(() => {
        this.notificationService.show({
          content: 'Xóa thanh toán thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });

        this.loadRecord();
        this.loadPayments();
      });
    });
  }

  printPayment(payment) {
    this.paymentService.getPrint(payment.accountPaymentId).subscribe(result => {
      this.accountPaymentPrintComponent.print(result);
    });
  }

  getAccountPaymentReconcicles() {
    if (this.saleOrderId) {
      this.saleOrderService.getAccountPaymentReconcicles(this.saleOrderId).subscribe(
        rs => {
          this.paymentsInfo = rs;
        }
      )
    }
  }

  showLaboList(id?: string) {
    const modalRef = this.modalService.open(
      SaleOrderLineLaboOrdersDialogComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' }
    );

    modalRef.componentInstance.title = 'Danh sách phiếu labo';
    modalRef.componentInstance.saleOrderLineId = id;
    modalRef.result.then((val) => {
    }, er => { });
  }

  onChangeQuantity(line: FormGroup) {
    var res = this.orderLines.controls.find(x => x.value.productId === line.value.productId);
    if (res) {
      res.patchValue(line.value);
    }
    this.getPriceSubTotal();
    this.computeAmountTotal();

  }

  onChangePriceUnit(line: FormGroup){
    debugger
    var res = this.orderLines.controls.find(x => x.value.productId === line.value.productId);
    if (res) {
      res.patchValue(line.value);
    }
    this.getPriceSubTotal();
    this.computeAmountTotal();
  }

  onChangeDiscountFixed(line: FormGroup) {
    var res = this.orderLines.controls.find(x => x.value.productId === line.value.productId);
    if (res) {
      res.patchValue(line.value);
    }
    this.getPriceSubTotal();
    this.computeAmountTotal();
  }

  updateSaleOrder() {
    if (this.formGroup.get('state').value == "sale") {
      var val = this.getFormDataSave();

      this.saleOrderService.update(this.saleOrderId, val).subscribe(() => {
        this.notificationService.show({
          content: 'Lưu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.loadRecord();
      }, () => {
        this.loadRecord();
      });
    }

  }

  onChangeDiscount(line: FormGroup) {
    var res = this.orderLines.controls.find(x => x.value.productId === line.value.productId);
    if (res) {
      res.patchValue(line.value);
    }
    this.getPriceSubTotal();
    this.computeAmountTotal();
  }

  onChangeDiscountType(line: FormGroup) {
    var res = this.orderLines.controls.find(x => x.value.productId === line.value.productId);
    if (res) {
      res.value.discount = 0;
      res.value.discountFixed = 0;
      res.patchValue(line.value);
    }
    this.getPriceSubTotal();
    this.computeAmountTotal();
  }

}