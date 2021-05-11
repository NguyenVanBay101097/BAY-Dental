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
import { ToothDiagnosisSave } from 'src/app/tooth-diagnosis/tooth-diagnosis.service';
import { SaleOrderLinePromotionDialogComponent } from '../sale-order-line-promotion-dialog/sale-order-line-promotion-dialog.component';

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
  loading = false;
  saleOrderId: string;
  partnerId: string;
  filteredPartners: PartnerSimple[];
  filteredUsers: UserSimple[];
  filteredPricelists: ProductPriceListBasic[];
  discountDefault: DiscountDefault;
  // isEditting = true;
  isChanged = false;

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
            this.patchValueSaleOrder(result);
            setTimeout(() => {
              this.formGroup.valueChanges.subscribe(res => {
                {
                  this.isChanged = true;
                }
              });
            }, 0);
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


  updateLineInfo(line, lineControl: FormGroup) {
    line.toothCategoryId = line.toothCategory.id;
    line.assistantId = line.assistant ? line.assistant.id : null;
    line.employeeId = line.employee ? line.employee.id : null;
    // line.productUOMQty = (line.teeth && line.teeth.length > 0) ? line.teeth.length : 1;
    line.counselorId = line.counselor ? line.counselor.id : null;
    lineControl.patchValue(line);

    lineControl.setControl('teeth', this.fb.array(line.teeth));
    lineControl.setControl('promotions', this.fb.array(line.promotions));

    lineControl.updateValueAndValidity();
    // this.onChangeQuantity(lineControl);
    this.computeAmountTotal();

    this.lineSelected = null;
  }

  getAmountAdvanceBalance() {
    this.partnerService.getAmountAdvanceBalance(this.partner.id).subscribe(result => {
      this.amountAdvanceBalance = result;
    })
  }

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

  actionCancel() {
    if (this.saleOrderId) {
      this.saleOrderService.actionCancel([this.saleOrderId]).subscribe(() => {
        this.loadRecord();
        document.getElementById('home-tab').click()
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
        this.saleOrderId = result.id;
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

    if (result.User) {
      this.filteredUsers = _.unionBy(this.filteredUsers, [result.user], 'id');
    }
    if (result.Partner) {
      this.filteredPartners = _.unionBy(this.filteredPartners, [result.partner], 'id');
      if (!this.saleOrderId) {
        this.onChangePartner(result.partner);
      }
    }

    this.formGroup.setControl('promotions', this.fb.array(result.promotions));

    let control = this.formGroup.get('orderLines') as FormArray;
    control.clear();
    result.orderLines.forEach(line => {
      var g = this.fb.group(line);
      g.setControl('teeth', this.fb.array(line.teeth));
      g.setControl('promotions', this.fb.array(line.promotions));
      control.push(g);
    });

    this.formGroup.markAsPristine();
    this.getAmountAdvanceBalance();
  }


  async loadRecord() {
    if (this.saleOrderId) {
      //  this.saleOrderService.get(this.saleOrderId).subscribe((result: any) => {
      //     this.patchValueSaleOrder(result);
      //     this.saleOrder = result;
      //     return result;
      //   });
      var result = await this.saleOrderService.get(this.saleOrderId).toPromise();
      this.patchValueSaleOrder(result);
      this.saleOrder = result;
      this.isChanged = false;
      return result;
    }
  }

  actionEdit() {
    // this.isEditting = true;
    // this.lineVCR.forEach(vc => {
    //   vc.canEdit = true;
    // });
  }

  get orderLines() {
    return this.getFormControl('orderLines') as FormArray;
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
      amountInvoiced: 0,//thanh toán
      productId: val.id,
      product: {
        id: val.id,
        name: val.name
      },
      productUOMQty: 1,
      state: 'draft',
      teeth: this.fb.array([]),
      promotions: this.fb.array([]),
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

  getFormControl(value) {
    return this.formGroup.get(value);
  }

  get amountTotal() {
    return this.getFormControl('amountTotal').value;
  }

  get getAmountPaidTotal() {
    return this.amountTotal - this.getResidual;
  }

  get getResidual() {
    return this.getFormControl('residual').value;
  }

  computeAmountTotal() {
    let total = 0;
    this.orderLines.controls.forEach(line => {
      total += line.get('priceSubTotal').value;
    });
    // this.computeResidual(total);
    this.formGroup.get('amountTotal').patchValue(total);
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


  isEditSate() {
    return ['draft', 'sale'].indexOf(this.getFormControl('state').value) !== -1
  }

  getAmount() {
    return (this.orderLines.value as any[]).reduce((total, cur) => {
      return total + cur.priceUnit * cur.productUOMQty;
    }, 0);
  }

  getAmountTotal() {
    return this.getAmount() - this.getTotalDiscount();
  }

  getTotalDiscount() {
    var res = (this.orderLines.value as any[]).reduce((total, cur) => {
      return total + (cur.amountDiscountTotal || 0) * cur.productUOMQty;
    }, 0);

    return res;
  }

  onDeleteLine(index) {
    if (this.orderLines.controls[index].value == this.lineSelected) {
      this.lineSelected = null;
    }
    this.orderLines.removeAt(index);
    this.computeAmountTotal();
  }

  async openSaleOrderPromotionDialog() {
    let modalRef = this.modalService.open(SaleOrderPromotionDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
    modalRef.componentInstance.saleOrder = this.saleOrder;
    modalRef.componentInstance.getUpdateSJ().subscribe(async res => {
      // this.saleOrderService.get(this.saleOrderId).subscribe((result: any) => {
      //   this.patchValueSaleOrder(result);
      //   modalRef.componentInstance.saleOrder = this.saleOrder;
      // });
      var r = await this.loadRecord();
      modalRef.componentInstance.saleOrder = r;
    });
  }

  onOpenSaleOrderPromotion() {

    //update line trước khi lưu
    if (this.lineSelected != null) {
      var viewChild = this.lineVCR.find(x => x.line == this.lineSelected);
      viewChild.updateLineInfo();
    }

    //nếu data không change thì mở dialog luôn
    if (!this.isChanged) {
      this.openSaleOrderPromotionDialog();
      return;
    }

    const val = this.getFormDataSave();

    if (!this.saleOrderId) {
      this.submitted = true;
      if (!this.formGroup.valid) {
        return false;
      }
      this.saleOrderService.create(val).subscribe((result: any) => {
        this.saleOrderId = result.id;
        this.saleOrder = result;
        this.saleOrder.promotions = [];

        this.router.navigate(["/sale-orders/form"], {
          queryParams: { id: result.id },
        });



        this.openSaleOrderPromotionDialog();
      });
    } else {

      this.saleOrderService.update(this.saleOrderId, val).subscribe((result: any) => {
        this.openSaleOrderPromotionDialog();
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
    if (this.saleOrderId && this.saleOrder.promotions) {
      return (this.saleOrder.promotions as any[]).reduce((total, cur) => {
        return total + cur.amount;
      }, 0);
    }
    return 0;
  }

  async onOpenLinePromotionDialog(i) {
    let modalRef = this.modalService.open(SaleOrderLinePromotionDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
    modalRef.componentInstance.saleOrderLine = this.orderLines.controls[i].value;
    modalRef.componentInstance.getUpdateSJ().subscribe(async res => {
      // this.saleOrderService.get(this.saleOrderId).subscribe((result: any) => {
      //   this.patchValueSaleOrder(result);
      //   modalRef.componentInstance.saleOrderLine = this.orderLines.controls[i].value;

      // });
      var r = await this.loadRecord();
      modalRef.componentInstance.saleOrder = r;
    });
  }

  onUpdateOpenLinePromotion(line, lineControl, i) {
    //update line trước khi lưu
    if (this.lineSelected != null) {
      var viewChild = this.lineVCR.find(x => x.line == this.lineSelected);
      viewChild.updateLineInfo();
    }
    //nếu data không change thì mở dialog luôn
    if (!this.isChanged) {
      this.onOpenLinePromotionDialog(i);
      return;
    }
    // this.updateLineInfo(line, lineControl);// lưu ở client
    const val = this.getFormDataSave();
    if (!this.saleOrderId) {
      this.submitted = true;
      if (!this.formGroup.valid) {
        return false;
      }

      this.saleOrderService.create(val).subscribe((result: any) => {
        this.saleOrderId = result.id;
        this.router.navigate(["/sale-orders/form"], {
          queryParams: { id: result.id },
        });
        this.saleOrderService.get(this.saleOrderId).subscribe((result: any) => {
          this.patchValueSaleOrder(result);
          this.onOpenLinePromotionDialog(i);

        });
      })
    } else {

      this.saleOrderService.update(this.saleOrderId, val).subscribe((result: any) => {
        this.saleOrderService.get(this.saleOrderId).subscribe((result: any) => {
          this.patchValueSaleOrder(result);
          this.onOpenLinePromotionDialog(i);

        });
      });
    }
  }

}

