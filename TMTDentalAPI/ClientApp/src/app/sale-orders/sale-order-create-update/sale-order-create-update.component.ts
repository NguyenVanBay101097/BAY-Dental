import { DiscountDefault } from '../../core/services/sale-order.service';
import { Component, EventEmitter, OnInit, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { FormControl, FormGroup, FormBuilder, FormArray, Validators } from '@angular/forms';
import { switchMap, mergeMap } from 'rxjs/operators';
import { PartnerSimple, PartnerPaged } from 'src/app/partners/partner-simple';
import { PartnerService } from 'src/app/partners/partner.service';
import { UserService, UserPaged } from 'src/app/users/user.service';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { SaleOrderService } from '../../core/services/sale-order.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { SaleOrderDisplay } from '../sale-order-display';
import * as _ from 'lodash';
import { UserSimple } from 'src/app/users/user-simple';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { AccountPaymentBasic, AccountPaymentService } from 'src/app/account-payments/account-payment.service';
import { PaymentInfoContent } from 'src/app/account-invoices/account-invoice.service';
import { CardCardService, CardCardPaged } from 'src/app/card-cards/card-card.service';
import { ProductPriceListBasic, ProductPricelistPaged } from 'src/app/price-list/price-list';
import { PriceListService } from 'src/app/price-list/price-list.service';
import { SaleOrderApplyCouponDialogComponent } from '../sale-order-apply-coupon-dialog/sale-order-apply-coupon-dialog.component';
import { PartnerSearchDialogComponent } from 'src/app/partners/partner-search-dialog/partner-search-dialog.component';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';
import { BehaviorSubject, of, Subject } from 'rxjs';
import { LaboOrderBasic, LaboOrderService, LaboOrderPaged } from 'src/app/labo-orders/labo-order.service';
import { DotKhamService } from 'src/app/dot-khams/dot-kham.service';
import { SaleOrderApplyServiceCardsDialogComponent } from '../sale-order-apply-service-cards-dialog/sale-order-apply-service-cards-dialog.component';
import { SaleOrderLineService } from '../../core/services/sale-order-line.service';
import { SaleOrderLineLaboOrdersDialogComponent } from '../sale-order-line-labo-orders-dialog/sale-order-line-labo-orders-dialog.component';
import { SaleOrderLineDialogComponent } from 'src/app/shared/sale-order-line-dialog/sale-order-line-dialog.component';
import { LaboOrderCuDialogComponent } from '../labo-order-cu-dialog/labo-order-cu-dialog.component';
import { PartnerCustomerCuDialogComponent } from 'src/app/shared/partner-customer-cu-dialog/partner-customer-cu-dialog.component';
import { SaleOrderPaymentDialogComponent } from '../sale-order-payment-dialog/sale-order-payment-dialog.component';
import { EmployeeService } from 'src/app/employees/employee.service';
import { SaleOrdersOdataService } from 'src/app/shared/services/sale-ordersOdata.service';
import { EmployeesOdataService } from 'src/app/shared/services/employeeOdata.service';
import { ToothCategoryOdataService } from 'src/app/shared/services/tooth-categoryOdata.service';
import { TeethOdataService } from 'src/app/shared/services/toothOdata.service';
import { ToaThuocCuDialogSaveComponent } from 'src/app/shared/toa-thuoc-cu-dialog-save/toa-thuoc-cu-dialog-save.component';
import { ToaThuocService } from 'src/app/toa-thuocs/toa-thuoc.service';
import { PrintService } from 'src/app/shared/services/print.service';
import { PartnerCustomerToathuocListComponent } from '../partner-customer-toathuoc-list/partner-customer-toathuoc-list.component';
import { AppointmentCreateUpdateComponent } from 'src/app/shared/appointment-create-update/appointment-create-update.component';
import { SaleOrderPaymentListComponent } from '../sale-order-payment-list/sale-order-payment-list.component';
import { AccountPaymentsOdataService } from 'src/app/shared/services/account-payments-odata.service';
import { ToaThuocCuDialogComponent } from 'src/app/toa-thuocs/toa-thuoc-cu-dialog/toa-thuoc-cu-dialog.component';
import { ToaThuocLinesSaveCuFormComponent } from 'src/app/toa-thuocs/toa-thuoc-lines-save-cu-form/toa-thuoc-lines-save-cu-form.component';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { ToothDisplay, ToothFilter, ToothService } from 'src/app/teeth/tooth.service';
import { EmployeePaged } from 'src/app/employees/employee';
import { ToothCategoryBasic, ToothCategoryService } from 'src/app/tooth-categories/tooth-category.service';
import { SaleOrderPromotionDialogComponent } from '../sale-order-promotion-dialog/sale-order-promotion-dialog.component';
import { SaleOrderLineCuComponent } from '../sale-order-line-cu/sale-order-line-cu.component';

declare var $: any;

@Component({
  selector: 'app-sale-order-create-update',
  templateUrl: './sale-order-create-update.component.html',
  styleUrls: ['./sale-order-create-update.component.css'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({ height: '0px', minHeight: '0' })),
      state('expanded', style({ height: '*' })),
      transition('expanded <=> collapsed', animate('300ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ],
  host: {
    class: 'o_action o_view_controller'
  }
})



export class SaleOrderCreateUpdateComponent implements OnInit {
  formGroup: FormGroup;
  loading = false;
  saleOrderId: string;
  partnerId: string;
  filteredPartners: PartnerSimple[];
  filteredUsers: UserSimple[];
  filteredPricelists: ProductPriceListBasic[];
  discountDefault: DiscountDefault;

  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;
  @ViewChild('userCbx', { static: true }) userCbx: ComboBoxComponent;
  @ViewChild('pricelistCbx', { static: true }) pricelistCbx: ComboBoxComponent;
  @ViewChild('employeeCbx', { static: false }) employeeCbx: ComboBoxComponent;
  @ViewChild('assistantCbx', { static: false }) assistantCbx: ComboBoxComponent;
  @ViewChild('counselorCbx', { static: false }) counselorCbx: ComboBoxComponent;
  @ViewChild('toathuocComp', { static: false }) toathuocComp: PartnerCustomerToathuocListComponent;
  @ViewChild('paymentComp', { static: false }) paymentComp: SaleOrderPaymentListComponent;

  saleOrder: any = new SaleOrderDisplay();
  saleOrderPrint: any;
  laboOrders: LaboOrderBasic[] = [];
  saleOrderLine: any;
  payments: AccountPaymentBasic[] = [];
  paymentsInfo: PaymentInfoContent[] = [];

  searchCardBarcode: string;
  listTeeths: any[] = [];
  type: string;
  submitted = false;
  amountAdvanceBalance: number = 0;
  defaultToothCate: ToothCategoryBasic;

  childEmiter = new BehaviorSubject<any>(null);
  @ViewChildren('lineTemplate') lineVCR: QueryList<SaleOrderLineCuComponent>;
  lineSelected = null;

  constructor(
    private fb: FormBuilder,
    private partnerService: PartnerService,
    private userService: UserService,
    private route: ActivatedRoute,
    private saleOrderService: SaleOrderService,
    private saleOrderLineService: SaleOrderLineService,
    private intlService: IntlService,
    private modalService: NgbModal,
    private router: Router,
    private notificationService: NotificationService,
    private cardCardService: CardCardService,
    private pricelistService: PriceListService,
    private errorService: AppSharedShowErrorService,
    private toaThuocService: ToaThuocService,
    private printService: PrintService,
    private accountPaymentOdataService: AccountPaymentsOdataService,
    private toothCategoryService: ToothCategoryService,

  ) {
  }

  ngOnInit() {
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
    this.formGroup = this.fb.group({
      partner: [null, Validators.required],
      dateOrderObj: [null, Validators.required],
      orderLines: this.fb.array([]),
      companyId: null,
      amountTotal: 0,
      state: null,
      residual: null,
      card: null,
      pricelist: [null],
    });
    this.routeActive();
    this.getAccountPaymentReconcicles();
    this.loadToothCateDefault();
  }

  get f() {
    return this.formGroup.controls;
  }

  routeActive() {
    this.route.params.subscribe(
      () => {
        this.route.queryParamMap.pipe(
          switchMap((params: ParamMap) => {
            this.saleOrderId = params.get("id");
            this.partnerId = params.get("partner_id");
            if (this.saleOrderId) {
              return this.saleOrderService.get(this.saleOrderId);
            } else {
              return this.saleOrderService.defaultGet({ partnerId: this.partnerId || '' });
            }
          })).subscribe((result: any) => {
            this.saleOrder = result;
            this.formGroup.patchValue(result);
            let dateOrder = new Date(result.dateOrder);
            this.formGroup.get('dateOrderObj').patchValue(dateOrder);

            if (result.User) {
              this.filteredUsers = _.unionBy(this.filteredUsers, [result.user], 'id');
            }

            if (result.Partner) {
              this.filteredPartners = _.unionBy(this.filteredPartners, [result.partner], 'id');
              if (!this.saleOrderId) {
                this.onChangePartner(result.partner);
              }
              this.getAmountAdvanceBalance();
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
    )
  }

  get stateControl() {
    return this.formGroup.get('state');
  }

  getStateDisplay() {
    var state = this.formGroup.get('state').value;
    switch (state) {
      case 'sale':
        return 'Đang điều trị';
      case 'done':
        return 'Hoàn thành';
      default:
        return 'Nháp';
    }
  }

  get customerId() {
    var parterIdParam = this.route.snapshot.queryParamMap.get('partner_id');
    if (parterIdParam) {
      return parterIdParam;
    }

    if (this.saleOrderId && this.saleOrder) {
      return this.saleOrder.partnerId;
    }

    return undefined;
  }

  get partner() {
    var control = this.formGroup.get('partner');
    return control ? control.value : null;
  }

  loadToothCateDefault() {
    this.toothCategoryService.getDefaultCategory().subscribe(
      res => {
        this.defaultToothCate = res;
      }
    );
  }


  updateLineInfo(line, lineControl) {
    line.toothCategoryId = line.toothCategory.id;
    line.assistantId = line.assistant ? line.assistant.id : null;
    line.employeeId = line.employee ? line.employee.id : null;
    // line.productUOMQty = (line.teeth && line.teeth.length > 0) ? line.teeth.length : 1;
    line.counselorId = line.counselor ? line.counselor.id : null;
    lineControl.patchValue(line);

    lineControl.get('teeth').clear();
    line.teeth.forEach(teeth => {
      let g = this.fb.group(teeth);
      lineControl.get('teeth').push(g);
    });

    lineControl.updateValueAndValidity();
    // this.onChangeQuantity(lineControl);
    this.computeAmountTotal();

    this.lineSelected = null;
  }

  get cardValue() {
    return this.formGroup.get('card').value;
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

      this.createRecord().subscribe((result: any) => {
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

  // lineEditable(line: FormControl) {
  //   if (line.get('isRewardLine')) {
  //     return !line.get('isRewardLine').value;
  //   }

  //   return true;
  // }

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

      this.createRecord().subscribe((result: any) => {
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

  getAmountAdvanceBalance() {
    this.partnerService.getAmountAdvanceBalance(this.partner.id).subscribe(result => {
      this.amountAdvanceBalance = result;
    })
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
      this.createRecord().subscribe((result: any) => {
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
      this.saveRecord().subscribe(() => {
        this.discountDefault = val;
        this.discountDefault.saleOrderId = this.saleOrderId;
        this.saleOrderService.applyDiscountDefault(this.discountDefault).subscribe(() => {
          this.loadRecord();
        }, (error) => {
          this.errorService.show(error);
        });
      });
    }
    else {
      if (!this.formGroup.valid) {
        return false;
      }

      this.createRecord().subscribe((result: any) => {
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
    const val = this.getFormDataSave();
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
    var val = this.getFormDataSave();
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

  createNew() {
    if (this.customerId) {
      this.router.navigate(['/sale-orders/form'], { queryParams: { partner_id: this.customerId } });
    }
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

  // actionLabo(item?) {
  //   if (this.saleOrderId) {
  //     let modalRef = this.modalService.open(LaboOrderCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
  //     if (item && item.id) {
  //       modalRef.componentInstance.title = 'Cập nhật phiếu labo';
  //       modalRef.componentInstance.id = item.id;
  //     }
  //     else {
  //       modalRef.componentInstance.title = 'Tạo phiếu labo';
  //     }

  //     modalRef.componentInstance.saleOrderId = this.saleOrderId;

  //     modalRef.result.then(res => {
  //       if (res) {
  //         this.loadLaboOrderList();
  //       }
  //     }, () => {
  //     });
  //   }
  // }


  printSaleOrder() {
    if (this.saleOrderId) {
      this.saleOrderService.printSaleOrder(this.saleOrderId).subscribe((result: any) => {
        this.printService.printHtml(result.html);
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
    const val = Object.assign({}, this.formGroup.value);
    val.dateOrder = this.intlService.formatDate(val.dateOrderObj, 'yyyy-MM-ddTHH:mm:ss');
    val.partnerId = val.partner.id;
    val.pricelistId = val.pricelist ? val.pricelist.id : null;
    val.userId = val.user ? val.user.id : null;
    val.cardId = val.card ? val.card.id : null;

    val.orderLines.forEach(line => {
      if (line.employee) {
        line.employeeId = line.employee.id;
      }

      if (line.assistant) {
        line.assistantId = line.assistant.id;
      }

      if (line.teeth) {
        line.toothIds = line.teeth.map(x => x.id);
      }

      if (line.toothCategory) {
        line.toothCategoryId = line.toothCategory.id;
      }

      if (line.counselor) {
        line.counselorId = line.counselor.id;
      }

    });
    return val;
  }

  onSaveConfirm() {
    //update line trước khi lưu
    if (this.lineSelected != null) {
      var viewChild = this.lineVCR.find(x => x.line == this.lineSelected);
      viewChild.updateLineInfo();
    }

    this.submitted = true;
    if (!this.formGroup.valid) {
      return false;
    }
    var val = this.getFormDataSave();
    if (!this.saleOrderId) {
      this.saleOrderService.create(val)
        .pipe(
          mergeMap((r: any) => {
            this.saleOrderId = r.id;
            return this.saleOrderService.actionConfirm([r.id]);
          })
        )
        .subscribe(r => {
          this.router.navigate(['/sale-orders/form'], { queryParams: { id: this.saleOrderId } });
        });
    } else {
      this.saleOrderService.update(this.saleOrderId, val)
        .pipe(
          mergeMap(r => {
            return this.saleOrderService.actionConfirm([this.saleOrderId]);
          })
        )
        .subscribe(() => {
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
    //update line trước khi lưu
    if (this.lineSelected != null) {
      var viewChild = this.lineVCR.find(x => x.line == this.lineSelected);
      viewChild.updateLineInfo();
    }

    this.submitted = true;
    if (!this.formGroup.valid) {
      return false;
    }
    const val = this.getFormDataSave();

    if (this.saleOrderId) {
      this.saleOrderService.update(this.saleOrderId, val).subscribe((res) => {
        this.notificationService.show({
          content: 'Lưu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.loadRecord();
      }, (error) => {
        this.loadRecord();
      });
    } else {
      this.saleOrderService.create(val).subscribe((result: any) => {
        this.router.navigate(['/sale-orders/form'], { queryParams: { id: result.id } });
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

  patchValueSaleOrder(result) {
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
    this.getAmountAdvanceBalance();
  }

  loadRecord() {
    if (this.saleOrderId) {
      this.saleOrderService.get(this.saleOrderId).subscribe((result: any) => {
        this.patchValueSaleOrder(result);
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
          document.getElementById('home-tab').click()
        });
      }, () => {
      });
    }
  }

  actionEdit() {

  }

  get orderLines() {
    return this.getFormControl('orderLines') as FormArray;
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
          this.notify('success', 'lưu thành công');
          this.loadRecord();
        }, () => {
          this.loadRecord();
        });
      }
    }, () => {
    });


  }

  addLine(val) {
    if (this.lineSelected) {
      this.notify('error', 'Vui lòng hoàn thành dịch vụ hiện tại để thêm dịch vụ khác');
      return;
    }
    // this.saleOrderLine = event;
    var value = {
      amountPaid: 0,
      amountResidual: 0,
      diagnostic: '',
      discount: 0,
      discountFixed: 0,
      discountType: 'percentage',
      employee: null,
      employeeId: '',
      assistant: null,
      assistantId: '',
      name: val.name,
      priceSubTotal: val.listPrice * 1,
      priceUnit: val.listPrice,
      productId: val.id,
      product: {
        id: val.id,
        name: val.name
      },
      productUOMQty: 1,
      state: 'draft',
      teeth: this.fb.array([]),
      toothCategory: this.defaultToothCate,
      toothCategoryId: this.defaultToothCate.id,
      counselor: null,
      counselorId: null,
      toothType: 'manual',
      isActive: true,
      amountPromotionToOrder: 0,
      amountPromotionToOrderLine: 0
    };
    var res = this.fb.group(value);

    this.orderLines.push(res);
    this.orderLines.markAsDirty();
    this.computeAmountTotal();

    this.saleOrderLine = null;
    this.lineSelected = res.value;

    // mặc định là trạng thái sửa
    setTimeout(() => {
      var viewChild = this.lineVCR.find(x => x.line == this.lineSelected);
      viewChild.onEditLine();
    }, 0);
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
    modalRef.componentInstance.partnerId = partner.id;
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
          this.notify('success', 'Sửa thành công');
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
    if (this.isEditSate() || this.formGroup.get('state').value == "cancel") {
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

  getFormControl(value) {
    return this.formGroup.get(value);
  }

  get getAmountTotal() {
    return this.getFormControl('amountTotal').value;
  }

  // get getAmountTotalDiscount() {
  //   if (this.saleOrder.orderLines) {
  //     const val = this.saleOrder.orderLines.find(x => x.isRewardLine === true);
  //     return val ? val.priceTotal : 0;
  //   }
  //   return 0;
  // }

  get getAmountPaidTotal() {
    return this.getFormControl('paidTotal').value;
  }

  // get getState() {
  //   return this.formGroup.get('state').value;
  // }

  get getResidual() {
    return this.getFormControl('residual').value;
  }

  // get getPartner() {
  //   return this.formGroup.get('partner').value;
  // }

  computeAmountTotal() {
    let total = 0;
    this.orderLines.controls.forEach(line => {
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
      this.saleOrderService.getSaleOrderPaymentBySaleOrderId(this.saleOrderId).subscribe(rs2 => {
        let modalRef = this.modalService.open(SaleOrderPaymentDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
        modalRef.componentInstance.title = 'Thanh toán';
        modalRef.componentInstance.defaultVal = rs2;
        modalRef.componentInstance.advanceAmount = this.amountAdvanceBalance;

        modalRef.result.then(result => {
          this.notificationService.show({
            content: 'Thanh toán thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });

          this.loadRecord();
          this.paymentComp.loadPayments();
          if (result.print) {
            this.printPayment(result.paymentId)
          }
        }, () => {
        });
      })
    }
  }

  printPayment(paymentId) {
    this.accountPaymentOdataService.getPrint(paymentId).subscribe(result => {
      if (result) {
        var html = result['html']
        this.printService.printHtml(html);
      }
    });
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
            content: 'Hủy dịch vụ thành công',
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

  getAccountPaymentReconcicles() {
    if (this.saleOrderId) {
      this.saleOrderService.getAccountPaymentReconcicles(this.saleOrderId).subscribe(
        rs => {
          this.paymentsInfo = rs;
        }
      )
    }
  }

  onChangeDiscountFixed(line: FormGroup) {

    // this.getPriceSubTotal();
    this.computeAmountTotal();
  }

  onChangeDiscount(event, line: FormGroup) {
    line.value.discountType = event.discountType;
    if (event.discountType == "fixed") {
      line.value.discountFixed = event.discountFixed;
    } else {
      line.value.discount = event.discountPercent;
    }

    line.patchValue(line.value);

    // this.getPriceSubTotal();
    this.computeAmountTotal();
  }

  onChangeDiscountType(line: FormGroup) {
    var res = this.orderLines.controls.find(x => x.value.productId === line.value.productId);
    if (res) {
      res.value.discount = 0;
      res.value.discountFixed = 0;
      res.patchValue(line.value);
    }
    // this.getPriceSubTotal();
    this.computeAmountTotal();
  }

  notify(type, content) {
    this.notificationService.show({
      content: content,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: type, icon: true }
    });
  }
  printToaThuoc(item) {
    this.toaThuocService.getPrint(item.id).subscribe((result: any) => {
      this.printService.printHtml(result.html);
    });
  }
  createProductToaThuoc() {
    let modalRef = this.modalService.open(ToaThuocCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm: Đơn Thuốc';
    modalRef.componentInstance.defaultVal = { partnerId: (this.partnerId || this.partner.id), saleOrderId: this.saleOrderId };
    modalRef.result.then((result: any) => {
      this.notify('success', 'Tạo toa thuốc thành công');
      this.toathuocComp.loadData();
      if (result.print) {
        this.printToaThuoc(result.item);
      }
    }, () => {
    });
  }

  dialogAppointment() {
    const modalRef = this.modalService.open(AppointmentCreateUpdateComponent, { size: 'lg', windowClass: 'o_technical_modal modal-appointment', keyboard: false, backdrop: 'static' });

    modalRef.componentInstance.defaultVal = { partnerId: (this.partnerId || this.partner.id), saleOrderId: this.saleOrderId };
    modalRef.result.then(() => {
      this.notify('success', 'Tạo lịch hẹn thành công');
    }, () => {
    });
  }

  paymentOutput(e) {
    this.loadRecord();
  }

  isLaboLine(line: FormGroup) {
    return line.get('productIsLabo') && line.get('productIsLabo').value;
  }

  isEditSate() {
    return ['draft', 'sale'].indexOf(this.getFormControl('state').value) !== -1
  }

  getAmount() {
    return (this.orderLines.value as any[]).reduce((total, cur) => {
      return total + cur.priceUnit * cur.productUOMQty;
    }, 0);
  }

  getTotalDiscount() {
    return (this.orderLines.value as any[]).reduce((total, cur) => {
      return total + cur.amountPromotionToOrder + cur.amountPromotionToOrderLine;
    }, 0);
  }

  onDeleteLine(index) {
    this.orderLines.removeAt(index);
    this.computeAmountTotal();
    this.lineSelected = null;
  }

  onOpenSaleOrderPromotion() {

    if (!this.saleOrderId) {
      this.submitted = true;
      if (!this.formGroup.valid) {
        return false;
      }
      const val = this.getFormDataSave();
      this.saleOrderService.create(val).subscribe((result: any) => {
        this.saleOrderId = result.id;
        let modalRef = this.modalService.open(SaleOrderPromotionDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
        modalRef.componentInstance.salerOrderId = result.id;
        modalRef.result.then(() => {
          this.router.navigate(["/sale-orders/form"], {
            queryParams: { id: result.id },
          });
        }, () => {
        });
      });
    } else {

      let modalRef = this.modalService.open(SaleOrderPromotionDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
      modalRef.componentInstance.salerOrderId = this.saleOrderId;
      modalRef.result.then(() => {
        this.loadRecord();
      }, () => {
      });
    }
  }

  onEditLine(line) {
    if (this.lineSelected != null) {
      this.notify('error', 'Vui lòng hoàn thành dịch vụ hiện tại để chỉnh sửa dịch vụ khác');
    } else {
      this.lineSelected = line;
      var viewChild = this.lineVCR.find(x => x.line == line);
      viewChild.onEditLine();
    }
  }

  onCancelEditLine(line) {
    this.lineSelected = null;
  }

  sumPromotionSaleOrder() {
    if(this.saleOrderId) {
      return (this.saleOrder.promotions as any[]).reduce((total, cur) => {
        return total + cur.amount;
      },0);
    }
    return 0;
  }

}

