import { EmployeeSimple } from './../../employees/employee';
import { Component, ElementRef, HostListener, OnInit, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray, FormControl } from '@angular/forms';
import { ActivatedRoute, Router, ParamMap } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import * as _ from 'lodash';
import { of } from 'rxjs';
import { debounceTime, tap, switchMap, mergeMap } from 'rxjs/operators';
import { PaymentInfoContent } from 'src/app/account-invoices/account-invoice.service';
import { AccountJournalFilter, AccountJournalService, AccountJournalSimple } from 'src/app/account-journals/account-journal.service';
import { AccountPaymentBasic, AccountPaymentService } from 'src/app/account-payments/account-payment.service';
import { AccountRegisterPaymentService } from 'src/app/account-payments/account-register-payment.service';
import { AuthService } from 'src/app/auth/auth.service';
import { CardCardService, CardCardPaged } from 'src/app/card-cards/card-card.service';
import { SaleOrderLineService } from 'src/app/core/services/sale-order-line.service';
import { DiscountDefault, SaleOrderService } from 'src/app/core/services/sale-order.service';
import { DotKhamService } from 'src/app/dot-khams/dot-kham.service';
import { DotKhamBasic } from 'src/app/dot-khams/dot-khams';
import { EmployeeBasic, EmployeePaged } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { LaboOrderBasic, LaboOrderService, LaboOrderPaged } from 'src/app/labo-orders/labo-order.service';
import { ProductPriceListBasic, ProductPricelistPaged } from 'src/app/price-list/price-list';
import { PriceListService } from 'src/app/price-list/price-list.service';
import { LaboOrderCuDialogComponent } from 'src/app/sale-orders/labo-order-cu-dialog/labo-order-cu-dialog.component';
import { SaleOrderApplyCouponDialogComponent } from 'src/app/sale-orders/sale-order-apply-coupon-dialog/sale-order-apply-coupon-dialog.component';
import { SaleOrderApplyServiceCardsDialogComponent } from 'src/app/sale-orders/sale-order-apply-service-cards-dialog/sale-order-apply-service-cards-dialog.component';
import { SaleOrderCreateDotKhamDialogComponent } from 'src/app/sale-orders/sale-order-create-dot-kham-dialog/sale-order-create-dot-kham-dialog.component';
import { SaleOrderDisplay } from 'src/app/sale-orders/sale-order-display';
import { SaleOrderLineLaboOrdersDialogComponent } from 'src/app/sale-orders/sale-order-line-labo-orders-dialog/sale-order-line-labo-orders-dialog.component';
import { SaleOrderPaymentDialogComponent } from 'src/app/sale-orders/sale-order-payment-dialog/sale-order-payment-dialog.component';
import { AccountPaymentPrintComponent } from 'src/app/shared/account-payment-print/account-payment-print.component';
import { AppointmentCreateUpdateComponent } from 'src/app/shared/appointment-create-update/appointment-create-update.component';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { DotKhamCreateUpdateDialogComponent } from 'src/app/shared/dot-kham-create-update-dialog/dot-kham-create-update-dialog.component';
import { PartnerCustomerCuDialogComponent } from 'src/app/shared/partner-customer-cu-dialog/partner-customer-cu-dialog.component';
import { SaleOrderLineDialogComponent } from 'src/app/shared/sale-order-line-dialog/sale-order-line-dialog.component';
import { PartnersService } from 'src/app/shared/services/partners.service';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';
import { ToaThuocCuDialogSaveComponent } from 'src/app/shared/toa-thuoc-cu-dialog-save/toa-thuoc-cu-dialog-save.component';
import { ToaThuocPrintComponent } from 'src/app/shared/toa-thuoc-print/toa-thuoc-print.component';
import { ToaThuocService } from 'src/app/toa-thuocs/toa-thuoc.service';
import { UserSimple } from 'src/app/users/user-simple';
import { UserService, UserPaged } from 'src/app/users/user.service';
import { PartnerCustomerTreatmentHistoryFormAddServiceDialogComponent } from '../partner-customer-treatment-history-form-add-service-dialog/partner-customer-treatment-history-form-add-service-dialog.component';
import { PartnerSearchDialogComponent } from '../partner-search-dialog/partner-search-dialog.component';
import { PartnerSimple, PartnerPaged } from '../partner-simple';
import { PartnerService } from '../partner.service';
import { PrintSaleOrderComponent } from 'src/app/shared/print-sale-order/print-sale-order.component';
import { PrintService } from 'src/app/shared/services/print.service';
import { SaleOrderLineCuComponent } from 'src/app/sale-orders/sale-order-line-cu/sale-order-line-cu.component';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { ToothCategoryBasic, ToothCategoryService } from 'src/app/tooth-categories/tooth-category.service';
import { SaleOrderPromotionDialogComponent } from 'src/app/sale-orders/sale-order-promotion-dialog/sale-order-promotion-dialog.component';
import { PromotionProgramSave } from 'src/app/promotion-programs/promotion-program.service';
import { SaleOrderPromotionSave } from 'src/app/sale-orders/sale-order-promotion.service';
import { PartnerCustomerTreatmentFastPromotionComponent } from '../partner-customer-treatment-fast-promotion/partner-customer-treatment-fast-promotion.component';
import { PartnerCustomerTreatmentLineFastPromotionComponent } from '../partner-customer-treatment-line-fast-promotion/partner-customer-treatment-line-fast-promotion.component';

@Component({
  selector: 'app-partner-customer-treatment-payment-fast',
  templateUrl: './partner-customer-treatment-payment-fast.component.html',
  styleUrls: ['./partner-customer-treatment-payment-fast.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class PartnerCustomerTreatmentPaymentFastComponent implements OnInit {
  formGroup: FormGroup;
  saleOrderId: string;
  partnerId: string;
  filteredPartners: PartnerSimple[];
  filteredUsers: UserSimple[];
  filteredPricelists: ProductPriceListBasic[];
  filteredJournals: AccountJournalSimple[];
  journalFixed: AccountJournalSimple;

  discountDefault: DiscountDefault;
  valueSearch: string;
  submitted = false;

  lineSelected = null;
  @ViewChildren('lineTemplate') lineVCR: QueryList<SaleOrderLineCuComponent>;

  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;
  @ViewChild('userCbx', { static: true }) userCbx: ComboBoxComponent;
  @ViewChild('pricelistCbx', { static: true }) pricelistCbx: ComboBoxComponent;
  @ViewChild(AccountPaymentPrintComponent, { static: true }) accountPaymentPrintComponent: AccountPaymentPrintComponent;
  @ViewChild(PrintSaleOrderComponent, { static: true }) printSaleOrderComponent: PrintSaleOrderComponent;
  @ViewChild('employeeCbx', { static: true }) employeeCbx: ComboBoxComponent;
  // @ViewChild('journalCbx', { static: true }) journalCbx: ComboBoxComponent;
  @ViewChild('search', { static: true }) searchElement: ElementRef;

  saleOrder: SaleOrderDisplay = new SaleOrderDisplay();
  saleOrderPrint: any;
  saleOrderPrintId: string;
  dotKhams: DotKhamBasic[] = [];
  laboOrders: LaboOrderBasic[] = [];
  saleOrderLine: any;
  payments: AccountPaymentBasic[] = [];
  paymentsInfo: PaymentInfoContent[] = [];
  filteredEmployees: EmployeeSimple[] = [];
  @ViewChild(ToaThuocPrintComponent, { static: true }) toaThuocPrintComponent: ToaThuocPrintComponent;
  defaultToothCate: ToothCategoryBasic;

  searchCardBarcode: string;
  partnerSend: any;
  type: string;

  get f() { return this.formGroup.controls; }

  constructor(private fb: FormBuilder, private partnerService: PartnerService, private toaThuocService: ToaThuocService,
    private route: ActivatedRoute, private saleOrderService: SaleOrderService, private accountJournalService: AccountJournalService,
    private intlService: IntlService, private modalService: NgbModal,
    private router: Router, private notificationService: NotificationService,
    private authService: AuthService,
    private odataPartnerService: PartnersService,
    private printService: PrintService,
    private notifyService: NotifyService,
    private toothCategoryService: ToothCategoryService

  ) {
  }

  ngOnInit() {
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
    this.formGroup = this.fb.group({
      partner: [null, Validators.required],
      partnerAge: null,
      partnerPhone: null,
      partnerAddress: null,
      dateOrderObj: [null, Validators.required],
      orderLines: this.fb.array([]),
      companyId: null,
      amountTotal: 0,
      state: null,
      residual: null,
      card: null,
      pricelist: [null],
      journal: [null, Validators.required],
      payments: null,
      promotions: this.fb.array([])
    });

    this.loadFilteredJournals();
    this.routeActive();


    this.loadPartners();

    this.partnerCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => this.partnerCbx.loading = true),
      switchMap(val => this.searchPartners(val))
    ).subscribe(
      rs => {
        this.filteredPartners = rs;
        this.partnerCbx.loading = false;
      }
    )

    //load default loại răng
    this.loadToothCateDefault();
  }

  loadToothCateDefault() {
    this.toothCategoryService.getDefaultCategory().subscribe(
      res => {
        this.defaultToothCate = res;
      }
    );
  }

  routeActive() {
    this.loadFilteredJournals();
    this.route.queryParamMap.pipe(
      switchMap((params: ParamMap) => {
        this.saleOrderId = params.get("id");
        this.partnerId = params.get("partner_id");
        if (this.saleOrderId) {
          return this.saleOrderService.get(this.saleOrderId);
        } else {
          return this.saleOrderService.defaultGet({ IsFast: true });
        }
      })).subscribe(result => {
        this.patchValueSaleOrder(result, false);

      });
  }

  getControl(value) {
    return this.formGroup.get(value);
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

  get partner() {
    var control = this.formGroup.get('partner');
    return control ? control.value : null;
  }

  get partnerAge() {
    return this.formGroup.get('partnerAge').value;
  }

  get partnerPhone() {
    return this.formGroup.get('partnerPhone').value;
  }

  get partnerAddress() {
    return this.formGroup.get('partnerAddress').value;
  }

  get orderLines() {
    return this.formGroup.get('orderLines') as FormArray;
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

  ///load phương thức thanh toán
  loadFilteredJournals() {
    this.searchJournals().subscribe(result => {
      this.filteredJournals = _.unionBy(this.filteredJournals, result, 'id');
    });
  }

  getJournalDefault() {
    var jounrnalDedault = this.filteredJournals.find(x => x.name == "Tiền mặt");
    return jounrnalDedault;
  }


  searchJournals(search?: string) {
    var val = new AccountJournalFilter();
    val.type = 'bank,cash';
    val.search = search || '';
    val.companyId = this.authService.userInfo.companyId;
    return this.accountJournalService.autocomplete(val);
  }

  actionPayment() {
    this.submitted = true;

    if (this.formGroup.invalid) {
      return;
    }

    var val = this.formGroup.value;
    val.dateOrder = this.intlService.formatDate(val.dateOrderObj, 'yyyy-MM-ddTHH:mm:ss');
    val.partnerId = this.getPartner.id;
    val.orderLines.forEach(line => {
      line.employeeId = line.employee ? line.employee.id : null;
      line.toothIds = line.teeth.map(x => x.id);
    });

    val.journalId = val.journal.id;

    this.saleOrderService.createFastSaleOrder(val).subscribe((rs: any) => {
      this.router.navigate(['/partners/treatment-paymentfast/from'], { queryParams: { id: rs.id } });
      this.printFastSaleOrder(rs.id);
      this.notificationService.show({
        content: "Thanh toán thành công",
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });

      this.routeActive();
    }, err => {
      console.log(err);
      this.submitted = false;
    });
  }

  loadPartners() {
    this.searchPartners().subscribe(result => {
      this.filteredPartners = _.unionBy(this.filteredPartners, result, 'id');
    });
  }

  searchPartners(filter?: string) {
    var val = new PartnerPaged();
    val.customer = true;
    val.search = filter;
    return this.partnerService.getAutocompleteSimple(val);
  }


  printFastSaleOrder(saleOrderId) {
    this.saleOrderService.printSaleOrder(saleOrderId).subscribe((result: any) => {
      this.printService.printHtml(result.html);
    });
  }

  getFormDataToReload() {
    var res = this.getFormDataSave();
    delete res.toothIds;
    return res;
  }

  getFormDataSave() {
    var val = this.formGroup.value;
    val.dateOrder = this.intlService.formatDate(val.dateOrderObj, 'yyyy-MM-ddTHH:mm:ss');
    val.partnerId = val.partner ? val.partner.id : null;
    val.pricelistId = val.pricelist ? val.pricelist.id : null;
    val.userId = val.user ? val.user.id : null;
    val.cardId = val.card ? val.card.id : null;
    val.amountTotal = (val.orderLines as any[]).reduce((total, cur) => {
      return total + cur.productUOMQty * cur.priceUnit;
    }, 0);
    val.orderLines.forEach(line => {
      if (line.employee) {
        line.employeeId = line.employee.id;
      }
      if (line.teeth) {
        line.toothIds = line.teeth.map(x => x.id);
      }
     
      line.priceSubTotal = Math.round(line.productUOMQty * (line.priceUnit - line.amountDiscountTotal))
    });
    return val;
  }

  onChangePartner(value) {
    if (this.partner) {
      this.odataPartnerService.get(this.partner.id, null).subscribe(rs => {
        this.formGroup.get('partnerAge').patchValue(rs.Age);
        this.formGroup.get('partnerPhone').patchValue(rs.Phone);
        this.formGroup.get('partnerAddress').patchValue(rs.Address);

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

        if (this.journalFixed) {
          this.filteredJournals = _.unionBy(this.filteredJournals, [this.journalFixed], 'id');
        } else {
          this.filteredJournals = _.unionBy(this.filteredJournals, [this.getJournalDefault()], 'id');
        }

        let control = this.formGroup.get('orderLines') as FormArray;
        control.clear();
        result.orderLines.forEach(line => {
          var g = this.fb.group(line);
          g.setControl('teeth', this.fb.array(line.teeth));
          control.push(g);
        });

        this.formGroup.markAsPristine();
        this.getPriceSubTotal();
        this.computeAmountTotal();

      });
    } else {
      this.filteredJournals = _.unionBy(this.filteredJournals, [this.getJournalDefault()], 'id');
    }
  }

  addLine(val) {
    if (this.lineSelected) {
      this.notifyService.notify('error', 'Vui lòng hoàn thành dịch vụ hiện tại để thêm dịch vụ khác');
      return;
    }

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
      amountPromotionToOrderLine: 0,
      amountDiscountTotal: 0
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
  ///Toa thuốc
  createToaThuoc() {
    let modalRef = this.modalService.open(ToaThuocCuDialogSaveComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm: Đơn Thuốc';
    modalRef.componentInstance.defaultVal = { partnerId: this.saleOrder.partner ? this.saleOrder.partner.id : null, saleOrderId: this.saleOrderId };
    modalRef.result.then((result: any) => {
      this.loadRecord();
      if (result.print) {
        this.printToaThuoc(result.item);
      }
    }, () => {
    });
  }

  printToaThuoc(item) {
    this.toaThuocService.getPrint(item.id).subscribe((result: any) => {
      this.printService.printHtml(result.html);
    });
  }

  ///lịch hẹn
  createAppoinment() {
    const modalRef = this.modalService.open(AppointmentCreateUpdateComponent, { size: 'lg', windowClass: 'o_technical_modal modal-appointment', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm: lịch hẹn';
    modalRef.componentInstance.defaultVal = { partnerId: this.getPartner ? this.getPartner.id : null };
    modalRef.result.then((result: any) => {
      this.notificationService.show({
        content: ' Tạo lịch hẹn thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
      this.routeActive();
    }, () => {
    });
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



  onCancelEditLine(line) {
    this.lineSelected = null;
  }

  getAmount() {
    return (this.orderLines.value as any[]).reduce((total, cur) => {
      return total + cur.priceUnit * cur.productUOMQty;
    }, 0);
  }

  getTotalDiscount() {
    var res = (this.orderLines.value as any[]).reduce((total, cur) => {
      return total + (cur.amountDiscountTotal || 0) * cur.productUOMQty;
    }, 0);

    return Math.round(res / 1000) * 1000;
  }

  onDeleteLine(index) {
    this.orderLines.removeAt(index);
    this.computeAmountTotal();
    this.lineSelected = null;
    this.saleOrder = this.getFormDataToReload();
    this.patchValueSaleOrder(this.saleOrder);
  }

  onEditLine(line) {
    if (this.lineSelected != null) {
      this.notifyService.notify('error', 'Vui lòng hoàn thành dịch vụ hiện tại để chỉnh sửa dịch vụ khác');
    } else {
      this.lineSelected = line;
      var viewChild = this.lineVCR.find(x => x.line == line);
      viewChild.onEditLine();
    }
  }


  updateLineInfo(line, lineControl) {
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
    this.onComputePromotion();
  }

  patchValueSaleOrder(result, compute = true) {
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
    if(compute) {
    this.onComputePromotion();
    }
  }

  getAmountPromotionOfSaleOrder(promotion: SaleOrderPromotionSave) {//get amount of saleorder from saleorderpromotion
    var amount = 0;
    this.saleOrder = this.getFormDataToReload();
    var total = this.saleOrder.orderLines.reduce((total, cur) => {
      return total + cur.priceUnit * cur.productUOMQty;
    }, 0);
    switch (promotion.type) {
      case 'discount':
        amount = promotion.discountType == 'percentage' ? promotion.discountPercent * total / 100 : promotion.discountFixed;
        break;
      case 'code_usage_program':
        amount = promotion.discountType == 'percentage' ? promotion.discountPercent * total / 100 : promotion.discountFixed;
        break;
      case 'promotion_program':
        amount = promotion.discountType == 'percentage' ? promotion.discountPercent * total / 100 : promotion.discountFixed;
        break;
      default:
        break;
    }

    return amount;
  }

  getAmountPromotionOfSaleOrderLine(line, promotion: SaleOrderPromotionSave) {//get amount of saleorderline from saleorderpromotion
    var amount = 0;
    var total = line.priceUnit * line.productUOMQty;
    switch (promotion.type) {
      case 'discount':
        var price_reduce = promotion.discountType == 'percentage'  ? line.priceUnit * (1 -promotion.discountPercent / 100) : line.priceUnit -  promotion.discountFixed;
        amount = (line.priceUnit - price_reduce) *line.productUOMQty;
        break;
      case 'code_usage_program':
          amount = promotion.discountType == 'percentage' ? promotion.discountPercent * total / 100 : promotion.discountFixed;
        break;
      case 'promotion_program':
          amount =promotion.discountType == 'percentage' ? promotion.discountPercent * total/ 100 :  promotion.discountFixed;
        break;
      default:
        break;
    }

    return amount;
  }

  onComputePromotion() {
    var promotions = this.getControl('promotions').value as any[];
    var total = this.getAmount();

    //compute saleorderpromotion
    promotions.forEach(pro => {
      pro.amount = this.getAmountPromotionOfSaleOrder(pro);
    });
    this.formGroup.setControl('promotions', this.fb.array(promotions));
    //compute saleorderline promotion
    var lineFA = this.getControl('orderLines') as FormArray;
    lineFA.controls.forEach((lineFG: FormGroup) => {
      var line = lineFG.value;
      let amountToOrder = 0;
      promotions.forEach((pro: SaleOrderPromotionSave) => {
        amountToOrder += ((((line.productUOMQty * line.priceUnit) / total) * pro.amount) / line.productUOMQty);
      })

      let linePromotions = line.promotions;
      linePromotions.forEach(pro => {
        pro.amount = this.getAmountPromotionOfSaleOrderLine(line, pro);
      });
      lineFG.setControl('promotions', this.fb.array(linePromotions));

      let amountToLine = linePromotions.reduce((total, pro) => {
        return total + (pro.amount / line.productUOMQty);
      }, 0);

      lineFG.get('amountPromotionToOrder').setValue(amountToOrder);
      lineFG.get('amountPromotionToOrderLine').setValue(amountToLine);
      lineFG.get('amountDiscountTotal').setValue(amountToOrder + amountToLine);
      lineFG.get('priceSubTotal').setValue(Math.round((line.priceUnit - amountToOrder - amountToLine) * line.productUOMQty));
    })
  }

  openSaleOrderPromotionDialog() {
    let modalRef = this.modalService.open(PartnerCustomerTreatmentFastPromotionComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
    modalRef.componentInstance.saleOrder = this.saleOrder;
    modalRef.componentInstance.getUpdateSJ().subscribe(res => {
      this.patchValueSaleOrder(res);
      //phân bổ 
      this.saleOrder = this.getFormDataToReload();//phân bổ xong pass lại saleOrder
      modalRef.componentInstance.saleOrder = this.saleOrder;
    });
  }

  onOpenSaleOrderPromotion() {
    //update line trước khi lưu
    if (this.lineSelected != null) {
      var viewChild = this.lineVCR.find(x => x.line == this.lineSelected);
      viewChild.updateLineInfo();
    }
    this.saleOrder = this.getFormDataToReload();
    this.openSaleOrderPromotionDialog();

  }

  sumPromotionSaleOrder() {
    if(this.saleOrder && this.saleOrder.promotions)
      return (this.saleOrder.promotions as any[]).reduce((total, cur) => {
        return total + cur.amount;
      }, 0);
      return 0;
  }


  onOpenLinePromotionDialog(i) {
    let modalRef = this.modalService.open(PartnerCustomerTreatmentLineFastPromotionComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
    modalRef.componentInstance.saleOrderLine = this.orderLines.controls[i].value;
    modalRef.componentInstance.getUpdateSJ().subscribe(res => {//res is saleorderline value
      this.updateLineInfo(res, this.orderLines.controls[i]);
      this.saleOrder = this.getFormDataToReload();
      this.patchValueSaleOrder(this.saleOrder);
      //phân bổ, tính lại promotionline
      //phân bổ xong pass lại saleorderline
      modalRef.componentInstance.saleOrderLine = this.orderLines.controls[i].value;
    });
  }

  onUpdateOpenLinePromotion(line, lineControl, i) {
    //update line trước khi mở popup promotion
    if (this.lineSelected != null) {
      var viewChild = this.lineVCR.find(x => x.line == this.lineSelected);
      viewChild.updateLineInfo();
    }

    this.saleOrder = this.getFormDataToReload();
    this.onOpenLinePromotionDialog(i);
  }

}
