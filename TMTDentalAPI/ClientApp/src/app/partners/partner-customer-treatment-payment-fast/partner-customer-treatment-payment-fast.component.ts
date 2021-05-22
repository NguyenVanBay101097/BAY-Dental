import { EmployeePaged, EmployeeSimple } from './../../employees/employee';
import { Component, ElementRef, OnInit, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { FormGroup, FormBuilder, FormArray, FormControl, Validators } from '@angular/forms';
import { ActivatedRoute, Router, ParamMap } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import * as _ from 'lodash';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { PaymentInfoContent } from 'src/app/account-invoices/account-invoice.service';
import { AccountJournalFilter, AccountJournalService, AccountJournalSimple } from 'src/app/account-journals/account-journal.service';
import { AccountPaymentBasic } from 'src/app/account-payments/account-payment.service';
import { AuthService } from 'src/app/auth/auth.service';
import { SaleOrderService } from 'src/app/core/services/sale-order.service';
import { DotKhamBasic } from 'src/app/dot-khams/dot-khams';
import { LaboOrderBasic } from 'src/app/labo-orders/labo-order.service';
import { AccountPaymentPrintComponent } from 'src/app/shared/account-payment-print/account-payment-print.component';
import { AppointmentCreateUpdateComponent } from 'src/app/shared/appointment-create-update/appointment-create-update.component';
import { PartnerCustomerCuDialogComponent } from 'src/app/shared/partner-customer-cu-dialog/partner-customer-cu-dialog.component';
import { ToaThuocCuDialogSaveComponent } from 'src/app/shared/toa-thuoc-cu-dialog-save/toa-thuoc-cu-dialog-save.component';
import { ToaThuocPrintComponent } from 'src/app/shared/toa-thuoc-print/toa-thuoc-print.component';
import { ToaThuocService } from 'src/app/toa-thuocs/toa-thuoc.service';
import { PartnerSimple, PartnerPaged } from '../partner-simple';
import { PartnerService } from '../partner.service';
import { PrintSaleOrderComponent } from 'src/app/shared/print-sale-order/print-sale-order.component';
import { PrintService } from 'src/app/shared/services/print.service';
import { SaleOrderLineCuComponent } from 'src/app/sale-orders/sale-order-line-cu/sale-order-line-cu.component';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { ToothCategoryBasic, ToothCategoryService } from 'src/app/tooth-categories/tooth-category.service';
import { SaleOrderPromotionSave } from 'src/app/sale-orders/sale-order-promotion.service';
import { PartnerCustomerTreatmentFastPromotionComponent } from '../partner-customer-treatment-fast-promotion/partner-customer-treatment-fast-promotion.component';
import { PartnerCustomerTreatmentLineFastPromotionComponent } from '../partner-customer-treatment-line-fast-promotion/partner-customer-treatment-line-fast-promotion.component';
import { EmployeeService } from 'src/app/employees/employee.service';
import { ToothFilter, ToothService } from 'src/app/teeth/tooth.service';
import { formatDate } from '@angular/common';
import { SaleCouponProgramService } from 'src/app/sale-coupon-promotion/sale-coupon-program.service';

@Component({
  selector: 'app-partner-customer-treatment-payment-fast',
  templateUrl: './partner-customer-treatment-payment-fast.component.html',
  styleUrls: ['./partner-customer-treatment-payment-fast.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class PartnerCustomerTreatmentPaymentFastComponent implements OnInit {
  filteredPartners: PartnerSimple[];
  formGroup: FormGroup;
  saleOrder: any = {};
  lineSelected = null;
  saleOrderId: string;
  applyPromotionList = [];

  defaultToothCate: any;
  listTeeths: any[] = [];
  filteredToothCategories: any[] = [];
  initialListEmployees: any[] = [];
  @ViewChildren('lineTemplate') lineVCR: QueryList<SaleOrderLineCuComponent>;
  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;
  @ViewChild(PrintSaleOrderComponent, { static: true }) printSaleOrderComponent: PrintSaleOrderComponent;

  constructor(private fb: FormBuilder, private partnerService: PartnerService, private toaThuocService: ToaThuocService,
    private route: ActivatedRoute, private saleOrderService: SaleOrderService, private accountJournalService: AccountJournalService,
    private intlService: IntlService, private modalService: NgbModal,
    private router: Router, private notificationService: NotificationService,
    private authService: AuthService,
    private printService: PrintService,
    private notifyService: NotifyService,
    private toothCategoryService: ToothCategoryService,
    private employeeService: EmployeeService,
    private toothService: ToothService,
    private saleCouponProgramService: SaleCouponProgramService
  ) {
  }

  ngOnInit() {
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;

    this.formGroup = this.fb.group({
      partner: [null, Validators.required],
      dateOrder: [null, Validators.required]
    });

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

    this.routeActive();
    //load data for line component
    this.loadEmployees();
    this.loadToothCategories();
    this.loadTeethList();
    this.loadToothCateDefault();
    this.loadApplyPromotionList();
  }

  get orderLines() {
    return this.saleOrder ? this.saleOrder.orderLines : null;
  }

  get getPartner() {
    var res = this.saleOrder.partner ?  this.saleOrder.partner  : null;
    return res;
  }

  get f() {
    return this.formGroup.controls;
  }

  get state() {
    return this.saleOrder.state;
  }

  routeActive() {
    this.route.queryParamMap.pipe(
      switchMap((params: ParamMap) => {
        this.saleOrderId = params.get("id");
        if (this.saleOrderId) {
          return this.saleOrderService.get(this.saleOrderId);
        } else {
          return this.saleOrderService.defaultGet({ IsFast: true });
        }
      })).subscribe(result => {
        this.saleOrder = result;
        this.saleOrder.dateOrder = new Date(this.saleOrder.dateOrder);
        console.log(formatDate(this.saleOrder.dateOrder, 'yyyy-MM-ddTHH:mm:ss', 'en-US'));

        this.formGroup.patchValue(this.saleOrder);
      });
  }

  loadToothCateDefault() {
    this.toothCategoryService.getDefaultCategory().subscribe(
      res => {
        this.defaultToothCate = res;
      }
    );
  }

  loadApplyPromotionList() {
    this.saleCouponProgramService.getPromotionByFastSaleOrder().subscribe(res => {
      this.applyPromotionList = res;
    });
  }

  loadEmployees() {
    var val = new EmployeePaged();
    val.limit = 0;
    val.offset = 0;
    val.isDoctor = true;
    val.active = true;

    this.employeeService
      .getEmployeeSimpleList(val)
      .subscribe((result: any[]) => {
        this.initialListEmployees = result;
      });
  }

  loadToothCategories() {
    this.toothCategoryService.getAll().subscribe((result: any[]) => {
      this.filteredToothCategories = result;
    });
  }

  loadTeethList() {
    var val = new ToothFilter();
    this.toothService.getAllBasic(val).subscribe((result: any[]) => {
      this.listTeeths = result;
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
      this.saleOrder.partner = p;
      this.filteredPartners = _.unionBy(this.filteredPartners, [p], 'id');
      this.onChangePartner(p);
    }, () => {
    });
  }

  actionPayment() {
    //update line trước khi lưu
    if (this.lineSelected != null) {
      var viewChild = this.lineVCR.find(x => x.line == this.lineSelected);
      let rs = viewChild.updateLineInfo();
      if (!rs) return;
    }

    var val = this.getFormDataSave();
    val.dateOrder = this.intlService.formatDate(val.dateOrder, 'yyyy-MM-ddTHH:mm:ss');
    val.partnerId = val.partner.id;
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

  getFormDataSave() {
    var val = Object.assign({}, this.saleOrder);
    var formValue = this.formGroup.value;
    val = { ...val, formValue };
    console.log(formatDate(val.dateOrder, 'yyyy-MM-ddTHH:mm:ss', 'en-US'));

    val.dateOrder = this.intlService.formatDate(val.dateOrder, 'yyyy-MM-ddTHH:mm:ss');
    val.partnerId = val.partner ? val.partner.id : null;
    val.pricelistId = val.pricelist ? val.pricelist.id : null;
    val.userId = val.user ? val.user.id : null;
    val.cardId = val.card ? val.card.id : null;
    val.amountTotal = (val.orderLines as any[]).reduce((total, cur) => {
      return total + cur.productUOMQty * cur.priceUnit;
    }, 0);

    val.promotions.forEach(p => {
      delete p.saleCouponProgram;
    });

    val.orderLines.forEach(line => {
      if (line.employee) {
        line.employeeId = line.employee.id;
      }
      if (line.assistant) {
        line.assistantId = line.assistant.id;
      }
      if (line.counselor) {
        line.counselorId = line.counselor.id;
      }
      if (line.teeth) {
        line.toothIds = line.teeth.map(x => x.id);
      }

      line.priceSubTotal = Math.round(line.productUOMQty * (line.priceUnit - line.amountDiscountTotal))

      line.promotions.forEach(p => {
        delete p.saleCouponProgram;
      });
    });

    val.isFast = true;
    return val;
  }

  onChangePartner(value) {
    if (value) {
      this.partnerService.getPartner(value.id).subscribe(rs => {
        this.saleOrder.partner = rs;
        this.saleOrder.orderLines.forEach(line => {
          line.orderPartnerId = rs.id;
        });
      });
    }
  }

  loadRecord() {
    if (this.saleOrderId) {
      this.saleOrderService.get(this.saleOrderId).subscribe(result => {
        this.saleOrder = result;
        let dateOrder = new Date(result.dateOrder);
        this.saleOrder.dateOrder = dateOrder;

        if (result.partner) {
          this.filteredPartners = _.unionBy(this.filteredPartners, [result.partner], 'id');
        }

        this.getPriceSubTotal();
        this.computeAmountTotal();

      });
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
        name: val.name,
        categId: val.categId
      },
      productUOMQty: 1,
      state: 'draft',
      teeth: [],
      promotions: [],
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
    this.saleOrder.orderLines.push(value);
    this.onComputePromotion();
    this.computeAmountTotal();

    this.lineSelected = this.saleOrder.orderLines[this.saleOrder.orderLines.length - 1];
    // mặc định là trạng thái sửa
    setTimeout(() => {
      var viewChild = this.lineVCR.find(x => x.line == this.lineSelected);
      viewChild.onEditLine();
    }, 0);
  }

  getPriceSubTotal() {
    this.saleOrder.orderLines.forEach(line => {
      var discountType = line.discountType ? line.discountType : 'percentage';
      var discountFixedValue = line.discountFixed ? line.discountFixed : 0;
      var discountNumber = line.discount ? line.discount : 0;
      var getquanTity = line.productUOMQty ? line.productUOMQty : 1;
      var getamountPaid = line.amountPaid ? line.amountPaid : 0;
      var priceUnit = line.priceUnit ? line.priceUnit.value : 0;
      var price = priceUnit * getquanTity;

      var subtotal = discountType == 'percentage' ? price * (1 - discountNumber / 100) :
        Math.max(0, price - discountFixedValue);
      line.priceSubTotal = subtotal;
      var getResidual = subtotal - getamountPaid;
      line.amountResidual = getResidual;
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
    this.orderLines.forEach(line => {
      console.log(total);
      total += line.priceSubTotal;
    });
    // this.computeResidual(total);
    this.saleOrder.amountTotal = (total);
  }

  getAmount() {
    if (!this.orderLines) return 0;
    return (this.orderLines as any[]).reduce((total, cur) => {
      return total + cur.priceUnit * cur.productUOMQty;
    }, 0);
  }

  getTotalDiscount() {
    if (!this.orderLines) return 0;
    var res = (this.orderLines as any[]).reduce((total, cur) => {
      return total + (cur.amountDiscountTotal || 0) * cur.productUOMQty;
    }, 0);

    return Math.round(res / 1000) * 1000;
  }

  onDeleteLine(index) {
    this.orderLines.splice(index, 1);
    this.lineSelected = null;
    this.onComputePromotion();
    this.computeAmountTotal();

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

  onCancelEditLine(line) {
    this.lineSelected = null;
  }

  updateLineInfo(lineNew, line) {
    line.priceSubTotal = 323232;
    line = Object.assign(line, lineNew);
    this.lineSelected = null;
    this.onComputePromotion();
    this.computeAmountTotal();
  }

  getAmountPromotionOfSaleOrder(promotion: SaleOrderPromotionSave) {//get amount of saleorder from saleorderpromotion
    var amount = 0;
    var total = this.saleOrder.orderLines.reduce((total, cur) => {
      return total + cur.priceUnit * cur.productUOMQty;
    }, 0);
    switch (promotion.type) {
      case 'discount':
        amount = promotion.discountType == 'percentage' ? promotion.discountPercent * total / 100 : promotion.discountFixed;
        break;
      case 'code_usage_program':
        amount = promotion.discountType == 'percentage' ? this.getMaxAmountPromotion(promotion.discountPercent * total / 100, promotion) : promotion.discountFixed;
        break;
      case 'promotion_program':
        amount = promotion.discountType == 'percentage' ? this.getMaxAmountPromotion(promotion.discountPercent * total / 100, promotion) : promotion.discountFixed;
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
        var price_reduce = promotion.discountType == 'percentage' ? line.priceUnit * (1 - promotion.discountPercent / 100) : line.priceUnit - promotion.discountFixed;
        amount = (line.priceUnit - price_reduce) * line.productUOMQty;
        break;
      case 'code_usage_program':
        amount = promotion.discountType == 'percentage' ? this.getMaxAmountPromotion(promotion.discountPercent * total / 100, promotion) : promotion.discountFixed;
        break;
      case 'promotion_program':
        amount = promotion.discountType == 'percentage' ? this.getMaxAmountPromotion(promotion.discountPercent * total / 100, promotion) : promotion.discountFixed;
        break;
      default:
        break;
    }

    return amount;
  }

  onComputePromotion() {
    var promotions = this.saleOrder.promotions as any[];
    var total = this.getAmount();

    //compute saleorderpromotion
    promotions.forEach(pro => {
      pro.amount = this.getAmountPromotionOfSaleOrder(pro);
    });
    this.saleOrder.promotions = promotions;
    //compute saleorderline promotion
    this.orderLines.forEach((line) => {
      let amountToOrder = 0;
      promotions.forEach((pro: SaleOrderPromotionSave) => {
        amountToOrder += ((((line.productUOMQty * line.priceUnit) / total) * pro.amount) / line.productUOMQty);
      })

      let linePromotions = line.promotions;
      linePromotions.forEach(pro => {
        pro.amount = this.getAmountPromotionOfSaleOrderLine(line, pro);
      });
      line.promotions = linePromotions;

      let amountToLine = linePromotions.reduce((total, pro) => {
        return total + (pro.amount / line.productUOMQty);
      }, 0);

      line.amountPromotionToOrder = amountToOrder;
      line.amountPromotionToOrderLine = amountToLine;
      line.amountDiscountTotal = amountToOrder + amountToLine;
      line.priceSubTotal = Math.round((line.priceUnit - amountToOrder - amountToLine) * line.productUOMQty);
    })
  }

  applyDiscountPromotionSaleOrder(res) {
    var exist = this.saleOrder.promotions.find(x => x.type == 'discount');
    if (exist) {
      this.notifyService.notify('error', 'Ưu đãi đã được áp dụng cho đơn hàng này')
      return false;
    }
    //push cai uu dai vào mảng ưu đãi
    this.saleOrder.promotions.push({
      amount: res.discountType == 'percentage' ? res.discountPercent * this.getAmount() / 100 : res.discountFixed,
      type: 'discount',
      discountType: res.discountType,
      discountPercent: res.discountPercent,
      discountFixed: res.discountFixed,
      saleCouponProgramId: null,
      name: 'Giảm tiền',
      saleCouponProgram: null
    } as SaleOrderPromotionSave);
    //chạy hàm phân bổ
    this.onComputePromotion();
    this.computeAmountTotal();
    return true;
  }

  checkApplyPromotionSaleOrderLine(line, promotionApply) {
    var promotions = line.promotions;
    var res = this.checkApplyPromotionCommon(promotions, promotionApply);
    if (!res) return false;

    return true;
  }

  checkApplyPromotionCommon(promotions, promotionApply) {
    var exist = promotions.find(x => x.saleCouponProgramId == promotionApply.id);
    if (exist) {
      this.notifyService.notify('error', 'Mã đang trùng CTKM đang áp dụng')
      return false;
    }

    if (promotions.some(x => x.saleCouponProgram && x.saleCouponProgram.notIncremental)) {
      this.notifyService.notify('error', 'Đang áp dụng khuyến mãi không cộng dồn. Vui lòng xóa các CTKM đó.');
      return false;
    }
    if ((promotionApply.notIncremental && promotions.some(x => x.saleCouponProgram))) {
      this.notifyService.notify('error', 'Khuyến mãi này không dùng chung với CTKM khác. Vui lòng xóa các CTKM cũ');
      return false;
    }

    return true;
  }

  checkApplyPromotionSaleOrder(promotionApply) {
    var promotions = this.saleOrder.promotions;
    var res = this.checkApplyPromotionCommon(promotions, promotionApply);
    if (!res) return false;

    if (this.getAmount() < promotionApply.ruleMinimumAmount) {
      this.notifyService.notify('error', 'Đơn hàng không đạt giá trị tối thiểu ' + promotionApply.ruleMinimumAmount.toLocaleString('es'));
      return false;
    }
    return true;
  }

  getMaxAmountPromotion(amount, promotion) {
    return promotion.isApplyMaxDiscount ? (amount > promotion.discountMaxAmount ? promotion.discountMaxAmount : amount) : amount;
  }

  applyCouponPromotionSaleOrder(promotion) {

    var res = this.checkApplyPromotionSaleOrder(promotion);
    if (!res) return;
    //push cai uu dai vào mảng ưu đãi
    var type = promotion.promoCodeUsage == 'code_needed' ? 'code_usage_program' : 'promotion_program';
    this.saleOrder.promotions.push({
      amount: promotion.discountType == 'percentage' ? this.getMaxAmountPromotion(promotion.discountPercentage * this.getAmount() / 100, promotion) : promotion.discountFixedAmount,
      type: type,
      discountType: promotion.discountType,
      discountPercent: promotion.discountPercentage,
      discountFixed: promotion.discountFixedAmount,
      saleCouponProgramId: promotion.id,
      name: promotion.name,
      saleCouponProgram: promotion
    } as SaleOrderPromotionSave);
    //chạy hàm phân bổ
    this.onComputePromotion();
    this.computeAmountTotal();
    return true;
  }

  deletePromotionSaleOrder(item) {
    var promotions = this.saleOrder.promotions;
    var i = promotions.findIndex(x => x == item);
    promotions.splice(i, 1);
    //chạy hàm phân bổ
    this.onComputePromotion();
    this.computeAmountTotal();
  }

  openSaleOrderPromotionDialog() {
    var allPromotions = this.applyPromotionList.filter(x =>
      x.discountApplyOn == 'on_order' &&
      (x.applyPartnerOn == 'all' || x.discountSpecificPartners.map(x => x.id).some(z => z == this.saleOrder.partner.id)));
    let modalRef = this.modalService.open(PartnerCustomerTreatmentFastPromotionComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
    modalRef.componentInstance.saleOrder = this.saleOrder;
    modalRef.componentInstance.allPromotions = allPromotions;
    modalRef.componentInstance.getDiscountSJ().subscribe(res => {
      this.applyDiscountPromotionSaleOrder(res);
    });

    modalRef.componentInstance.getPromotionSJ().subscribe(res => {
      this.applyCouponPromotionSaleOrder(res);
    });

    modalRef.componentInstance.getDeleteSJ().subscribe(res => {
      this.deletePromotionSaleOrder(res);
    });
  }

  onOpenSaleOrderPromotion() {
    var partner = this.formGroup.get('partner').value;
    if (!partner) {
      this.notificationService.show({
        content: 'Vui lòng chọn khách hàng',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });

      return false;
    }

    //update line trước khi lưu
    if (this.lineSelected != null) {
      var viewChild = this.lineVCR.find(x => x.line == this.lineSelected);
      var rs = viewChild.updateLineInfo();
      if (!rs) return;
    }
    this.openSaleOrderPromotionDialog();

  }

  sumPromotionSaleOrder() {
    if (this.saleOrder && this.saleOrder.promotions)
      return (this.saleOrder.promotions as any[]).reduce((total, cur) => {
        return total + cur.amount;
      }, 0);
    return 0;
  }

  applyDiscountPromotionSaleOrderLine(res, line) {
    var exist = line.promotions.find(x => x.type == 'discount');
    if (exist) {
      this.notifyService.notify('error', 'Ưu đãi đã được áp dụng cho đơn hàng này')
      return false;
    }
    //push cai uu dai vào mảng ưu đãi
    line.promotions.push({
      amount: res.discountType == 'percentage' ? res.discountPercent * (line.priceUnit * line.productUOMQty) / 100 : res.discountFixed,
      type: 'discount',
      discountType: res.discountType,
      discountPercent: res.discountPercent,
      discountFixed: res.discountFixed,
      saleCouponProgramId: null,
      name: 'Giảm tiền',
      saleCouponProgram: null
    } as SaleOrderPromotionSave);
    //chạy hàm phân bổ
    this.onComputePromotion();
    this.computeAmountTotal();
    return true;
  }

  applyCouponPromotionSaleOrderLine(promotion, line) {
    var res = this.checkApplyPromotionSaleOrderLine(line, promotion);
    if (!res) return;
    //push cai uu dai vào mảng ưu đãi

    line.promotions.push({
      amount: promotion.discountType == 'percentage' ? this.getMaxAmountPromotion(promotion.discountPercentage * (line.priceUnit * line.productUOMQty) / 100, promotion) : promotion.discountFixedAmount,
      type: promotion.promoCodeUsage == 'code_needed' ? 'code_usage_program' : 'promotion_program',
      discountType: promotion.discountType,
      discountPercent: promotion.discountPercentage,
      discountFixed: promotion.discountFixedAmount,
      saleCouponProgramId: promotion.id,
      name: promotion.name,
      saleCouponProgram: promotion
    } as SaleOrderPromotionSave);
    //chạy hàm phân bổ
    this.onComputePromotion();
    this.computeAmountTotal();
    return true;
  }

  deletePromotionSaleOrderLine(item, line) {
    var promotions = line.promotions;
    var i = promotions.findIndex(x => x == item);
    promotions.splice(i, 1);
    //chạy hàm phân bổ
    this.onComputePromotion();
    this.computeAmountTotal();
  }


  onOpenLinePromotionDialog(i) {
    var line = this.orderLines[i];
    var allPromotions = this.applyPromotionList.filter(x =>
      ((x.discountApplyOn == 'specific_products' && x.discountSpecificProducts.map(z => z.id).some(i => i == line.productId)) ||
        (x.discountApplyOn == 'specific_product_categories' && x.discountSpecificProductCategories.map(z => z.id).some(i => i == line.product.categId)))
       &&
      (x.applyPartnerOn == 'all' || x.discountSpecificPartners.map(x => x.id).some(z => z == this.saleOrder.partner.id)));
    let modalRef = this.modalService.open(PartnerCustomerTreatmentLineFastPromotionComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
    modalRef.componentInstance.saleOrderLine = line;
    modalRef.componentInstance.allPromotions = allPromotions;
    modalRef.componentInstance.getDiscountSJ().subscribe(res => {
      this.applyDiscountPromotionSaleOrderLine(res, line);
    });
    modalRef.componentInstance.getPromotionSJ().subscribe(res => {
      this.applyCouponPromotionSaleOrderLine(res, line);
    });
    modalRef.componentInstance.getDeleteSJ().subscribe(res => {
      this.deletePromotionSaleOrderLine(res, line);
    });
  }

  onUpdateOpenLinePromotion(line, lineControl, i) {
    var partner = this.formGroup.get('partner').value;
    if (!partner) {
      this.notificationService.show({
        content: 'Vui lòng chọn khách hàng',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });

      return false;
    }

    //update line trước khi mở popup promotion
    if (this.lineSelected != null) {
      var viewChild = this.lineVCR.find(x => x.line == this.lineSelected);
      var rs = viewChild.updateLineInfo();
      if (!rs) return;
    }
    this.onOpenLinePromotionDialog(i);
  }

}
