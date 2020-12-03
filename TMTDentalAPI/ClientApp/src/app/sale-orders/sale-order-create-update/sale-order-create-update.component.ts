import { DiscountDefault } from '../../core/services/sale-order.service';
import { Component, OnInit, ViewChild } from '@angular/core';
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
import { of } from 'rxjs';
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

declare var $: any;

@Component({
  selector: 'app-sale-order-create-update',
  templateUrl: './sale-order-create-update.component.html',
  styleUrls: ['./sale-order-create-update.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class SaleOrderCreateUpdateComponent implements OnInit {
  formGroup: FormGroup;
  saleOrderId: string;
  partnerId: string;
  filteredPartners: PartnerSimple[];
  filteredUsers: UserSimple[];
  filteredPricelists: ProductPriceListBasic[];
  discountDefault: DiscountDefault;
  filteredToothCategories: any[];
  initialListTeeths: any[];

  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;
  @ViewChild('userCbx', { static: true }) userCbx: ComboBoxComponent;
  @ViewChild('pricelistCbx', { static: true }) pricelistCbx: ComboBoxComponent;
  @ViewChild('employeeCbx', { static: false }) employeeCbx: ComboBoxComponent;
  @ViewChild('toathuocComp', { static: false }) toathuocComp: PartnerCustomerToathuocListComponent;
  @ViewChild('paymentComp', { static: false }) paymentComp: SaleOrderPaymentListComponent;

  saleOrder: any = new SaleOrderDisplay();
  saleOrderPrint: any;
  laboOrders: LaboOrderBasic[] = [];
  saleOrderLine: any;
  payments: AccountPaymentBasic[] = [];
  paymentsInfo: PaymentInfoContent[] = [];
  filteredEmployees: any[] = [];
  initialListEmployees: any = [];

  searchCardBarcode: string;
  type: string;

  constructor(private fb: FormBuilder, private partnerService: PartnerService,
    private userService: UserService, private route: ActivatedRoute, private saleOrderService: SaleOrderService,
    private saleOrderLineService: SaleOrderLineService, private intlService: IntlService, private modalService: NgbModal,
    private router: Router, private notificationService: NotificationService, private cardCardService: CardCardService,
    private pricelistService: PriceListService, private errorService: AppSharedShowErrorService,
    private paymentService: AccountPaymentService,
    private laboOrderService: LaboOrderService, private dotKhamService: DotKhamService, private employeeService: EmployeeService,
    private saleOrderOdataService: SaleOrdersOdataService,
    private employeeOdataService: EmployeesOdataService, private toothCategoryOdataService: ToothCategoryOdataService,
    private teethOdataService: TeethOdataService,
    private toaThuocService: ToaThuocService,
    private printService: PrintService,
    private accountPaymentOdataService: AccountPaymentsOdataService
  ) {
  }

  ngOnInit() {
    this.formGroup = this.fb.group({
      Partner: [null, Validators.required],
      dateOrderObj: [null, Validators.required],
      OrderLines: this.fb.array([]),
      CompanyId: null,
      AmountTotal: 0,
      State: null,
      Residual: null,
      Card: null,
      Pricelist: [null],
    });
    this.routeActive();
    this.loadEmployees();
    this.getAccountPaymentReconcicles();
    this.loadToothCategories();
    this.loadTeethList();
  }

  loadEmployees() {
    const options = {
      select: 'Id,Name'
    };
    this.employeeOdataService.getFetch({}, options).subscribe(
      (result: any) => {
        this.initialListEmployees = result.data;
        this.filteredEmployees = this.initialListEmployees.slice(0, 20);
      }
    );
  }

  routeActive() {
    this.route.queryParamMap.pipe(
      switchMap((params: ParamMap) => {
        this.saleOrderId = params.get("id");
        this.partnerId = params.get("partner_id");
        if (this.saleOrderId) {
          return this.saleOrderOdataService.getDisplay(this.saleOrderId);
        } else {
          return this.saleOrderOdataService.defaultGet({ partnerId: this.partnerId || '' });
        }
      })).subscribe((result: any) => {
        this.saleOrder = result;
        this.formGroup.patchValue(result);
        let dateOrder = new Date(result.DateOrder);
        this.formGroup.get('dateOrderObj').patchValue(dateOrder);

        if (result.User) {
          this.filteredUsers = _.unionBy(this.filteredUsers, [result.User], 'Id');
        }

        if (result.Partner) {
          this.filteredPartners = _.unionBy(this.filteredPartners, [result.Partner], 'Id');
          if (!this.saleOrderId) {
            this.onChangePartner(result.Partner);
          }
        }

        // if (result.pricelist) {
        //   this.filteredPricelists = _.unionBy(this.filteredPricelists, [result.pricelist], 'id');
        // }

        const control = this.formGroup.get('OrderLines') as FormArray;
        control.clear();
        result.OrderLines.forEach(line => {
          var g = this.fb.group(line);
          g.setControl('Teeth', this.fb.array(line.Teeth));
          control.push(g);
        });

        this.formGroup.markAsPristine();
      });
  }

  get stateControl() {
    return this.formGroup.get('State');
  }

  getStateDisplay() {
    var state = this.formGroup.get('State').value;
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
      return this.saleOrder.PartnerId;
    }

    return undefined;
  }

  get partner() {
    var control = this.formGroup.get('Partner');
    return control ? control.value : null;
  }

  // loadLaboOrderList() {
  //   if (this.saleOrderId) {
  //     var val = new LaboOrderPaged();
  //     val.saleOrderId = this.saleOrderId;
  //     return this.laboOrderService.GetFromSaleOrder_OrderLine(val).subscribe(result => {
  //       this.laboOrders = result.items;
  //     });
  //   }
  // }

  get cardValue() {
    return this.formGroup.get('card').value;
  }

  // loadPartners() {
  //   this.searchPartners().subscribe(result => {
  //     this.filteredPartners = _.unionBy(this.filteredPartners, result, 'id');
  //   });
  // }

  // loadPricelists() {
  //   this.searchPricelists().subscribe(result => {
  //     this.filteredPricelists = _.unionBy(this.filteredPricelists, result.items, 'id');
  //   });
  // }

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
    var partner = this.formGroup.get('Partner').value;
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
          modalRef.componentInstance.amountTotal = this.formGroup.get('AmountTotal').value;
          modalRef.result.then(() => {
            this.loadRecord();
          }, () => {
          });
        })
      } else {
        let modalRef = this.modalService.open(SaleOrderApplyServiceCardsDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
        modalRef.componentInstance.orderId = this.saleOrderId;
        modalRef.componentInstance.amountTotal = this.formGroup.get('AmountTotal').value;
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
            id: result.Id
          },
        });

        let modalRef = this.modalService.open(SaleOrderApplyServiceCardsDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
        modalRef.componentInstance.orderId = result.Id;
        modalRef.componentInstance.amountTotal = this.formGroup.get('AmountTotal').value;
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
            id: result.Id
          },
        });

        let modalRef = this.modalService.open(SaleOrderApplyCouponDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
        modalRef.componentInstance.orderId = result.Id;
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
      this.createRecord().subscribe((result: any) => {
        this.router.navigate(['/sale-orders/form'], {
          queryParams: {
            id: result.Id
          },
        });

        this.saleOrderService.applyPromotion(result.Id).subscribe(() => {
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
            id: result.Id
          },
        });

        val.saleOrderId = result.Id;
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
    return this.saleOrderOdataService.create(val);
  }

  getDiscountNumber(line: FormGroup) {
    var discountType = line.get('DiscountType') ? line.get('DiscountType').value : 'percentage';
    if (discountType == 'fixed') {
      return line.get('DiscountFixed').value;
    } else {
      return line.get('Discount').value;
    }
  }

  getDiscountTypeDisplay(line: FormGroup) {
    var discountType = line.get('DiscountType') ? line.get('DiscountType').value : 'percentage';
    if (discountType == 'fixed') {
      return "";
    } else {
      return '%';
    }
  }

  saveRecord() {
    var val = this.getFormDataSave();
    return this.saleOrderOdataService.update(this.saleOrderId, val);
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
            return this.saleOrderOdataService.update(this.saleOrderId, val);
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
      this.saleOrderService.getPrint(this.saleOrderId).subscribe((result: any) => {
        this.saleOrderPrint = result;
        setTimeout(() => {
          var printContents = document.getElementById('printSaleOrderDiv').innerHTML;
          this.printService.print(printContents);
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
    const val = Object.assign({}, this.formGroup.value);
    val.DateOrder = this.intlService.formatDate(val.dateOrderObj, 'yyyy-MM-ddTHH:mm:ss');
    val.PartnerId = val.Partner.Id;
    val.pricelistId = val.Pricelist ? val.Pricelist.Id : null;
    val.UserId = val.User ? val.User.Id : null;
    val.CardId = val.card ? val.card.id : null;
    val.OrderLines.forEach(line => {
      if (line.Employee) {
        line.EmployeeId = line.Employee.Id;
      }
      if (line.Teeth) {
        line.ToothIds = line.Teeth.map(x => x.Id);
      }
    });
    return val;
  }

  onSaveConfirm() {
    if (!this.formGroup.valid) {
      return false;
    }
    var val = this.getFormDataSave();
    if (!this.saleOrderId) {
      this.saleOrderOdataService.create(val)
        .pipe(
          mergeMap((r: any) => {
            this.saleOrderId = r.Id;
            return this.saleOrderService.actionConfirm([r.Id]);
          })
        )
        .subscribe(r => {
          this.router.navigate(['/sale-orders/form'], { queryParams: { id: this.saleOrderId } });
        });
    } else {
      this.saleOrderOdataService.update(this.saleOrderId, val)
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
    if (!this.formGroup.valid) {
      return false;
    }
    const val = this.getFormDataSave();

    if (this.saleOrderId) {
      this.saleOrderOdataService.update(this.saleOrderId, val).subscribe(() => {
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
      this.saleOrderOdataService.create(val).subscribe((result: any) => {
        this.router.navigate(['/sale-orders/form'], { queryParams: { id: result.Id } });
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
      this.saleOrderOdataService.getDisplay(this.saleOrderId).subscribe((result: any) => {
        this.saleOrder = result;
        this.formGroup.patchValue(result);
        let dateOrder = new Date(result.DateOrder);
        this.formGroup.get('dateOrderObj').patchValue(dateOrder);

        if (result.Partner) {
          this.filteredPartners = _.unionBy(this.filteredPartners, [result.Partner], 'Id');
        }

        let control = this.formGroup.get('OrderLines') as FormArray;
        control.clear();
        result.OrderLines.forEach(line => {
          var g = this.fb.group(line);
          g.setControl('Teeth', this.fb.array(line.Teeth));
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
    return this.formGroup.get('OrderLines') as FormArray;
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
        this.saleOrderOdataService.update(this.saleOrderId, val).subscribe(() => {
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
    // this.saleOrderLine = event;
    var res = this.fb.group(val);
    // line.teeth = this.fb.array(line.teeth);
    if (!this.orderLines.controls.some(x => x.value.ProductId === res.value.ProductId)) {
      this.orderLines.push(res);
    } else {
      var line = this.orderLines.controls.find(x => x.value.ProductId === res.value.ProductId);
      if (line) {
        line.value.ProductUOMQty += 1;
        line.patchValue(line.value);
      }
    }
    this.getPriceSubTotal();
    this.orderLines.markAsDirty();
    this.computeAmountTotal();
    if (this.formGroup.get('State').value == "sale") {
      var val = this.getFormDataSave();
      this.saleOrderOdataService.update(this.saleOrderId, val).subscribe(() => {
        this.notify('success', 'Lưu thành công');
        this.loadRecord();
      }, () => {
        this.loadRecord();
      });
    }
    this.saleOrderLine = null;
  }

  updateTeeth(line, lineControl) {
    line.ProductUOMQty = (line.Teeth && line.Teeth.length > 0) ? line.Teeth.length : 1;
    lineControl.patchValue(line);
    lineControl.get('Teeth').clear();
    line.Teeth.forEach(teeth => {
      let g = this.fb.group(teeth);
      lineControl.get('Teeth').push(g);
    });
    this.onChangeQuantity(lineControl);
  }

  getPriceSubTotal() {
    this.orderLines.controls.forEach(line => {
      var discountType = line.get('DiscountType') ? line.get('DiscountType').value : 'percentage';
      var discountFixedValue = line.get('DiscountFixed') ? line.get('DiscountFixed').value : 0;
      var discountNumber = line.get('Discount') ? line.get('Discount').value : 0;
      var getquanTity = line.get('ProductUOMQty') ? line.get('ProductUOMQty').value : 1;
      var getamountPaid = line.get('AmountPaid') ? line.get('AmountPaid').value : 0;
      var priceUnit = line.get('PriceUnit') ? line.get('PriceUnit').value : 0;
      var price = priceUnit * getquanTity;

      var subtotal = discountType == 'percentage' ? price * (1 - discountNumber / 100) :
        Math.max(0, price - discountFixedValue);
      line.get('PriceSubTotal').setValue(subtotal);
      var getResidual = subtotal - getamountPaid;
      line.get('AmountResidual').setValue(getResidual);
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
        this.saleOrderOdataService.update(this.saleOrderId, val).subscribe(() => {
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
    if (this.formGroup.get('State').value == "draft" || this.formGroup.get('State').value == "cancel") {
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
    return this.formGroup.get('AmountTotal').value;
  }

  get getAmountTotalDiscount() {
    if (this.saleOrder.OrderLines) {
      const val = this.saleOrder.OrderLines.find(x => x.IsRewardLine === true);
      return val ? val.PriceTotal : 0;
    }
    return 0;
  }

  get getAmountPaidTotal() {
    return this.saleOrder.PaidTotal;
  }

  get getState() {
    return this.formGroup.get('State').value;
  }

  get getResidual() {
    return this.formGroup.get('Residual').value;
  }

  get getPartner() {
    return this.formGroup.get('Partner').value;
  }

  computeAmountTotal() {
    let total = 0;
    this.orderLines.controls.forEach(line => {
      total += line.get('PriceSubTotal').value;
    });
    // this.computeResidual(total);
    this.formGroup.get('AmountTotal').patchValue(total);
  }

  //Tính nợ theo số tổng
  computeResidual(total) {
    let diff = this.getAmountTotal - this.getResidual;
    let residual = total - diff;
    this.formGroup.get('Residual').patchValue(residual);
  }

  actionSaleOrderPayment() {
    if (this.saleOrderId) {
      this.paymentService.saleDefaultGet([this.saleOrderId]).subscribe(rs2 => {
        let modalRef = this.modalService.open(SaleOrderPaymentDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
        modalRef.componentInstance.title = 'Thanh toán';
        modalRef.componentInstance.defaultVal = rs2;
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
    var res = this.orderLines.controls.find(x => x.value.ProductId === line.value.ProductId);
    if (res) {
      res.patchValue(line.value);
    }
    this.getPriceSubTotal();
    this.computeAmountTotal();

  }

  onChangeDiscountFixed(line: FormGroup) {
    var res = this.orderLines.controls.find(x => x.value.ProductId === line.value.ProductId);
    if (res) {
      res.patchValue(line.value);
    }
    this.getPriceSubTotal();
    this.computeAmountTotal();
  }

  onChangeDiscount(event, line: FormGroup) {
    var res = this.orderLines.controls.find(x => x.value.ProductId === line.value.ProductId);
    if (res) {
      line.value.DiscountType = event.DiscountType;
      if (event.DiscountType == "fixed") {
        line.value.DiscountFixed = event.DiscountFixed;
      } else {
        line.value.Discount = event.DiscountPercent;
      }
      res.patchValue(line.value);
    }
    this.getPriceSubTotal();
    this.computeAmountTotal();
  }

  onChangeDiscountType(line: FormGroup) {
    var res = this.orderLines.controls.find(x => x.value.ProductId === line.value.ProductId);
    if (res) {
      res.value.discount = 0;
      res.value.discountFixed = 0;
      res.patchValue(line.value);
    }
    this.getPriceSubTotal();
    this.computeAmountTotal();
  }

  loadToothCategories() {
    const options = {
      select: 'Id,Name,Sequence'
    };
    this.toothCategoryOdataService.getFetch({}, options).subscribe(
      (result: any) => {
        this.filteredToothCategories = result.data;
      }
    );
  }
  loadTeethList() {
    const options = {
      select: 'Id,Name,CategoryId,ViTriHam,Position'
    };
    this.teethOdataService.getFetch({}, options).subscribe(
      (result: any) => {
        this.initialListTeeths = result.data;
      }
    );
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
    let modalRef = this.modalService.open(ToaThuocCuDialogSaveComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm: Đơn Thuốc';
    modalRef.componentInstance.defaultVal = { partnerId: (this.partnerId || this.partner.Id), saleOrderId: this.saleOrderId };
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

    modalRef.componentInstance.defaultVal = { partnerId: (this.partnerId || this.partner.Id), saleOrderId: this.saleOrderId };
    modalRef.result.then(() => {
      this.notify('success', 'Tạo lịch hẹn thành công');
    }, () => {
    });
  }

  paymentOutput(e) {
    this.loadRecord();
  }

  isLaboLine(line: FormGroup) {
    return line.get('ProductIsLabo') && line.get('ProductIsLabo').value;
  }
}
