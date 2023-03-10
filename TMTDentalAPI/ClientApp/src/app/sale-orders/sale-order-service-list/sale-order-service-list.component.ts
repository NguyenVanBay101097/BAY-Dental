import { DecimalPipe } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, OnInit, Output, QueryList, SimpleChanges, ViewChild, ViewChildren } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbModal, NgbPopover } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import * as moment from 'moment';
import { forkJoin } from 'rxjs';
import { catchError, mergeMap } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/auth.service';
import { AmountCustomerDebtFilter, CustomerDebtReportService } from 'src/app/core/services/customer-debt-report.service';
import { SaleOrderLineService } from 'src/app/core/services/sale-order-line.service';
import { SaleOrderService } from 'src/app/core/services/sale-order.service';
import { EmployeePaged } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { PartnerService } from 'src/app/partners/partner.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { ToothFilter, ToothService } from 'src/app/teeth/tooth.service';
import { ToothCategoryService } from 'src/app/tooth-categories/tooth-category.service';
import { SaleOrderInsurancePaymentDialogComponent } from '../sale-order-insurance-payment-dialog/sale-order-insurance-payment-dialog.component';
import { SaleOrderLineCuComponent } from '../sale-order-line-cu/sale-order-line-cu.component';
import { SaleOrderLinePromotionDialogComponent } from '../sale-order-line-promotion-dialog/sale-order-line-promotion-dialog.component';
import { SaleOrderPromotionDialogComponent } from '../sale-order-promotion-dialog/sale-order-promotion-dialog.component';
import { SaleOrderPromotionService } from '../sale-order-promotion.service';

@Component({
  selector: 'app-sale-order-service-list',
  templateUrl: './sale-order-service-list.component.html',
  styleUrls: ['./sale-order-service-list.component.css'],
  providers: [DecimalPipe]
})
export class SaleOrderServiceListComponent implements OnInit, OnChanges {
  @Input() saleOrder: any;
  @Output() updateOrderEvent = new EventEmitter<any>();
  @Output() insurancePayment = new EventEmitter<any>();
  @ViewChildren('lineTemplate') lineVCR: QueryList<SaleOrderLineCuComponent>;
  orderLines: any[] = [];
  promotions: any[] = []; //danh s??ch promotion c???a phi???u ??i???u tr???
  initialListEmployees: any[] = [];
  filteredToothCategories: any[] = [];
  listTeeths: any[] = [];
  partner: any;
  lineSelected = null;
  linesDirty = false;
  formGroup: FormGroup;
  submitted = false;
  partnerDebt = null;
  doctors = [];

  @ViewChild('popover', { static: false }) public popover: NgbPopover;

  constructor(
    private saleOrderService: SaleOrderService,
    private notificationService: NotificationService,
    private intlService: IntlService,
    private modalService: NgbModal,
    private saleOrderPromotionService: SaleOrderPromotionService,
    private saleOrderLineService: SaleOrderLineService,
    private toothCategoryService: ToothCategoryService,
    private employeeService: EmployeeService,
    private toothService: ToothService,
    private _decimalPipe: DecimalPipe,
    private partnerService: PartnerService,
    private customerDebtReportService: CustomerDebtReportService,
    private authService: AuthService,
    private fb: FormBuilder) { }

  ngOnChanges(changes: SimpleChanges): void {
    if (!changes.saleOrder.firstChange) {
      this.saleOrderService.get(this.saleOrder.id).subscribe((result: any) => {
        this.orderLines = result.orderLines;
        this.promotions = result.promotions;

        var dateOrder = new Date(result.dateOrder);
        this.formGroup.get('dateOrder').setValue(dateOrder);
        this.formGroup.get('doctor').setValue(result.doctor);
        this.lineSelected = null;
      });
    }
  }

  ngOnInit() {
    this.formGroup = this.fb.group({
      dateOrder: [null, Validators.required],
      doctor: [null],
      doctorId: [null]
    });

    this.saleOrderService.get(this.saleOrder.id).subscribe((result: any) => {
      this.orderLines = result.orderLines;
      this.promotions = result.promotions;

      var dateOrder = new Date(result.dateOrder);
      this.formGroup.get('dateOrder').setValue(dateOrder);
      this.formGroup.get('doctor').setValue(result.doctor);
    });

    this.loadToothCategories();
    this.loadEmployees();
    this.loadTeethList();
    this.loadPartnerInfo();
  }

  get f() {
    return this.formGroup.controls;
  }

  public hasEditingLine() {
    return this.lineSelected != null || this.lineSelected != undefined;
  }

  loadTeethList() {
    var val = new ToothFilter();
    this.toothService.getAllBasic(val).subscribe((result: any[]) => {
      this.listTeeths = result;
    });
  }

  saveSaleOrderInfo() {
    this.submitted = true;
    if (!this.formGroup.valid) {
      return false;
    }

    var value = this.formGroup.value;
    var val = {
      dateOrder: this.intlService.formatDate(value.dateOrder, 'yyyy-MM-ddTHH:mm:ss'),
      doctorId: value.doctor?.id ?? '',
      doctor: value.doctor
    };

    this.saleOrderService.update(this.saleOrder.id, val)
      .subscribe(() => {
        this.notify('success', 'L??u th??nh c??ng');
        this.updateOrderEvent.emit(val);
        this.popover.close();
      });
  }

  openUpdateOrderPopover(popover) {
    popover.open();
  }


  loadEmployees() {
    var val = new EmployeePaged();
    val.limit = 0;
    val.offset = 0;
    val.active = true;
    val.companyId = this.authService.userInfo.companyId;

    this.employeeService
      .getEmployeePaged(val)
      .subscribe((result: any) => {
        this.initialListEmployees = result.items;
        this.doctors = this.initialListEmployees.filter(x => x.isDoctor == true);
      });
  }


  loadToothCategories() {
    this.toothCategoryService.getAll().subscribe((result: any[]) => {
      this.filteredToothCategories = result;
    });
  }

  isEditSate() {
    return ['draft', 'sale'].indexOf(this.saleOrder.state) !== -1;
  }

  addLine(val) {
    if (this.lineSelected) {
      this.notify('error', 'Vui l??ng ho??n th??nh d???ch v??? hi???n t???i ????? th??m d???ch v??? kh??c');
      return;
    }
    // this.saleOrderLine = event;
    var toothCategory = this.filteredToothCategories[0];
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
      amountInvoiced: 0,//thanh to??n
      productId: val.id,
      product: {
        id: val.id,
        name: val.name,
      },
      productUOMQty: 1,
      productUOM: {
        name: val.uom.name,
        id: val.uom.id
      },
      state: this.saleOrder.state,
      teeth: [],
      promotions: [],
      toothCategory: toothCategory,
      toothCategoryId: toothCategory.id,
      counselor: null,
      counselorId: null,
      toothType: '',
      isActive: true,
      amountPromotionToOrder: 0,
      amountPromotionToOrderLine: 0,
      amountDiscountTotal: 0,
      amountInsurancePaidTotal: 0,
      orderPartnerId: this.saleOrder.partnerId,
      date: new Date(),
    };

    this.orderLines.unshift(value);
    // this.orderLines.push(value);
    // this.orderLines.markAsDirty();
    // this.computeAmountTotal();

    // this.saleOrderLine = null;
    this.lineSelected = value;

    // m???c ?????nh l?? tr???ng th??i s???a
    setTimeout(() => {
      var viewChild = this.lineVCR.find(x => x.line == this.lineSelected);
      viewChild.onEditLine();
    }, 0);
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

  onOpenSaleOrderPromotion() {
    //N???u phi???u ??i???u tr??? ch??a l??u
    if (this.lineSelected != null) { //N???u d??? li???u c???n l??u l???i
      var viewChild = this.lineVCR.find(x => x.line == this.lineSelected);
      var rs = viewChild.updateLineInfo();
      if (rs) {
        viewChild.onUpdateSignSubject.subscribe(value => {
          this.openSaleOrderPromotionDialog();
        })
      }
    } else {
      this.openSaleOrderPromotionDialog();
    }
  }

  actionInsurancePayment() {
    if (this.lineSelected != null) { //N???u d??? li???u c???n l??u l???i
      var viewChild = this.lineVCR.find(x => x.line == this.lineSelected);
      var rs = viewChild.updateLineInfo();
      if (rs) {
        viewChild.onUpdateSignSubject.subscribe(value => {
          this.onOpenInsurancePayment();
        })
      }
    } else {
      this.onOpenInsurancePayment();
    }
  }

  onOpenInsurancePayment() {
    if (this.saleOrder) {
      this.saleOrderService.getDefaultInsuranceBySaleOrderId(this.saleOrder.id).subscribe((res: any) => {
        const modalRef = this.modalService.open(SaleOrderInsurancePaymentDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
        modalRef.componentInstance.title = 'B???o hi???m b???o l??nh';
        modalRef.componentInstance.defaultValue = res;
        modalRef.componentInstance.saleOrderId = this.saleOrder.id;
        modalRef.result.then(result => {
          this.insurancePayment.emit(null);
          this.notify('success', "B???o l??nh th??nh c??ng");
        }, () => { });
      })
    }
  }

  getFormDataSave() {
    var val = {
      dateOrder: this.saleOrder.dateOrder,
      partnerId: this.saleOrder.partner.id,
      companyId: this.saleOrder.companyId,
      // orderLines: this.orderLines.map(x => {
      //   return {
      //     id: x.id,
      //     name: x.name,
      //     productId: x.product.id,
      //     priceUnit: x.priceUnit,
      //     productUOMQty: x.productUOMQty,
      //     employeeId: x.employee != null ? x.employee.id : null,
      //     assistantId: x.assistant != null ? x.assistant.id : null,
      //     counselorId: x.counselor != null ? x.counselor.id : null,
      //     toothIds: x.teeth.map(s => s.id),
      //     toothCategoryId: x.toothCategory != null ? x.toothCategory.id : null,
      //     diagnostic: x.diagnostic,
      //     toothType: x.toothType,
      //     isActive: x.isActive
      //   }
      // })
    };

    return val;

    // const val = Object.assign({}, this.formGroup.value);
    // val.dateOrder = this.intlService.formatDate(val.dateOrderObj, 'yyyy-MM-ddTHH:mm:ss');
    // val.partnerId = val.partner.id;
    // val.pricelistId = val.pricelist ? val.pricelist.id : null;
    // val.userId = val.user ? val.user.id : null;
    // val.cardId = val.card ? val.card.id : null;

    // val.orderLines.forEach(line => {
    //   if (line.employee) {
    //     line.employeeId = line.employee.id;
    //   }

    //   if (line.assistant) {
    //     line.assistantId = line.assistant.id;
    //   }

    //   if (line.teeth) {
    //     line.toothIds = line.teeth.map(x => x.id);
    //   }

    //   if (line.toothCategory) {
    //     line.toothCategoryId = line.toothCategory.id;
    //   }

    //   if (line.counselor) {
    //     line.counselorId = line.counselor.id;
    //   }

    // });
    // return val;
  }

  updateFormGroupDataToSaleOrder() {
    var value = this.formGroup.value;
    this.saleOrder.dateOrder = this.intlService.formatDate(value.dateOrderObj, 'yyyy-MM-ddTHH:mm:ss');
  }


  resetData(data) {
    this.orderLines = data.orderLines;
    this.promotions = data.promotions;
    this.resetFormPristine();
  }

  resetFormPristine() {
    this.linesDirty = false;
    // this.formGroup.markAsPristine();
  }

  openSaleOrderPromotionDialog() {
    let modalRef = this.modalService.open(SaleOrderPromotionDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
    modalRef.componentInstance.saleOrder = this.saleOrder;
    modalRef.componentInstance.promotions = this.promotions;

    modalRef.componentInstance.getBtnDiscountObs().subscribe(data => {
      var val = {
        id: this.saleOrder.id,
        discountType: data.discountType,
        discountPercent: data.discountPercent,
        discountFixed: data.discountFixed,
      };

      this.saleOrderService.applyDiscountOnOrder(val).pipe(
        mergeMap(() => this.saleOrderService.get(this.saleOrder.id))
      )
        .subscribe(res => {
          this.resetData(res);
          modalRef.componentInstance.saleOrder = this.saleOrder;
          modalRef.componentInstance.promotions = this.promotions;
        });
    });

    modalRef.componentInstance.getBtnPromoCodeObs().subscribe(data => {
      var val = {
        id: this.saleOrder.id,
        couponCode: data.couponCode
      };

      this.saleOrderService.applyCouponOnOrder(val).pipe(
        mergeMap((result: any) => {
          if (!result.success) {
            throw result;
          }
          return this.saleOrderService.get(this.saleOrder.id);
        })
      )
        .subscribe(res => {
          this.resetData(res);
          modalRef.componentInstance.saleOrder = this.saleOrder;
          modalRef.componentInstance.promotions = this.promotions;
        }, err => {
          console.log(err);
          this.notify('error', err.error);
        });
    });

    modalRef.componentInstance.getBtnPromoNoCodeObs().subscribe(data => {
      var val = {
        id: this.saleOrder.id,
        saleProgramId: data.id
      };

      this.saleOrderService.applyPromotion(val).pipe(
        catchError((err) => { throw err; }),
        mergeMap((result: any) => {
          return this.saleOrderService.get(this.saleOrder.id);
        })
      )
        .subscribe(res => {
          this.resetData(res);
          modalRef.componentInstance.saleOrder = this.saleOrder;
          modalRef.componentInstance.promotions = this.promotions;
        }, err => {
          console.log(err);
          this.notify('error', err.error.error);
        });
    });

    modalRef.componentInstance.getBtnDeletePromoObs().subscribe(data => {
      this.saleOrderPromotionService.removePromotion([data.id]).pipe(
        catchError((err) => { throw err; }),
        mergeMap((result: any) => {
          return this.saleOrderService.get(this.saleOrder.id);
        })
      )
        .subscribe(res => {
          this.resetData(res);
          modalRef.componentInstance.saleOrder = this.saleOrder;
          modalRef.componentInstance.promotions = this.promotions;
          this.notify('success', "X??a khuy???n m??i th??nh c??ng");
        }, err => {
          console.log(err);
          this.notify('error', err.error.error);
        });
    });
  }

  onUpdateOpenLinePromotion(i) {
    if (this.lineSelected != null) {
      var viewChild = this.lineVCR.find(x => x.line == this.lineSelected);
      var rs = viewChild.updateLineInfo();
      if (rs) {
        viewChild.onUpdateSignSubject.subscribe(value => {
          this.saleOrderService.get(this.saleOrder.id).subscribe(result => {
            this.resetData(result);
            this.onOpenLinePromotionDialog(i);
          });
        })
      }
    } else {
      this.onOpenLinePromotionDialog(i);
    }
  }

  onOpenLinePromotionDialog(i) {
    var line = this.orderLines[i];
    let modalRef = this.modalService.open(SaleOrderLinePromotionDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
    modalRef.componentInstance.saleOrderLine = line;

    modalRef.componentInstance.getBtnDiscountObs().subscribe(data => {
      var val = {
        id: line.id,
        discountType: data.discountType,
        discountPercent: data.discountPercent,
        discountFixed: data.discountFixed,
      };

      this.saleOrderLineService.applyDiscountOnOrderLine(val).pipe(
        mergeMap(() => this.saleOrderService.get(this.saleOrder.id))
      )
        .subscribe(res => {
          console.log(res);
          this.resetData(res);
          var newLine = this.orderLines[i];
          modalRef.componentInstance.saleOrderLine = newLine;
        });
    });

    modalRef.componentInstance.getBtnPromoCodeObs().subscribe(data => {
      var val = {
        id: line.id,
        couponCode: data.couponCode
      };

      this.saleOrderLineService.applyPromotionUsageCode(val).pipe(
        mergeMap((result: any) => {
          if (!result.success) {
            throw result;
          }
          return this.saleOrderService.get(this.saleOrder.id);
        })
      )
        .subscribe(res => {
          this.resetData(res);
          var newLine = this.orderLines[i];
          modalRef.componentInstance.saleOrderLine = newLine;
        }, err => {
          console.log(err);
          this.notify('error', err.error);
        });
    });

    modalRef.componentInstance.getBtnPromoNoCodeObs().subscribe(data => {
      var val = {
        id: line.id,
        saleProgramId: data.id
      };

      this.saleOrderLineService.applyPromotion(val).pipe(
        catchError((err) => { throw err; }),
        mergeMap((result: any) => {
          return this.saleOrderService.get(this.saleOrder.id);
        })
      )
        .subscribe(res => {
          this.resetData(res);
          var newLine = this.orderLines[i];
          modalRef.componentInstance.saleOrderLine = newLine;
        }, err => {
          console.log(err);
          this.notify('error', err.error.error);
        });
    });

    modalRef.componentInstance.getBtnPromoServiceCardObs().subscribe(data => {
      var val = {
        id: line.id,
        serviceCardId: data.id
      };

      this.saleOrderLineService.applyServiceCardCard(val).pipe(
        catchError((err) => { throw err; }),
        mergeMap((result: any) => {
          return this.saleOrderService.get(this.saleOrder.id);
        })
      )
        .subscribe(res => {
          this.resetData(res);
          var newLine = this.orderLines[i];
          modalRef.componentInstance.saleOrderLine = newLine;
        }, err => {
          console.log(err);
          this.notify('error', err.error.error);
        });
    });

    modalRef.componentInstance.getBtnPromoCardCardObs().subscribe(data => {
      var val = {
        id: line.id,
        cardId: data.id
      };

      this.saleOrderLineService.applyCardCard(val).pipe(
        catchError((err) => { throw err; }),
        mergeMap((result: any) => {
          return this.saleOrderService.get(this.saleOrder.id);
        })
      )
        .subscribe(res => {
          this.resetData(res);
          var newLine = this.orderLines[i];
          modalRef.componentInstance.saleOrderLine = newLine;
        }, err => {
          console.log(err);
          this.notify('error', err.error.error);
        });
    });

    modalRef.componentInstance.getBtnDeletePromoObs().subscribe(data => {
      this.saleOrderPromotionService.removePromotion([data.id]).pipe(
        catchError((err) => { throw err; }),
        mergeMap((result: any) => {
          return this.saleOrderService.get(this.saleOrder.id);
        })
      )
        .subscribe(res => {
          this.resetData(res);
          var newLine = this.orderLines[i];
          modalRef.componentInstance.saleOrderLine = newLine;
          this.notify('success', "X??a khuy???n m??i th??nh c??ng");
        }, err => {
          console.log(err);
          this.notify('error', err.error.error);
        });
    });
  }

  onCancelEditLine(line, index) {
    if (!line.id)
      this.orderLines.splice(index, 1);
    this.lineSelected = null;
  }

  onDeleteLine(index) {
    var line = this.orderLines[index];
    if (this.lineSelected != null && this.lineSelected != line) {
      this.notify('error', 'Vui l??ng ho??n th??nh d???ch v??? hi???n t???i ????? ch???nh s???a d???ch v??? kh??c');
      return;
    }
    if (line.id) {
      let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
      modalRef.componentInstance.title = 'X??a d???ch v???';
      modalRef.componentInstance.body = 'B???n c?? mu???n x??a d???ch v??? kh??ng ?';
      modalRef.result.then(() => {
        this.saleOrderLineService.remove(line.id).subscribe(res => {
          if (line == this.lineSelected) {
            this.lineSelected = null;
          }
          this.orderLines.splice(index, 1);
          this.notify('success', 'X??a d???ch v??? th??nh c??ng');
          //load l???i list promotions c???a phi???u ??i???u tr???
        })
      });
    } else {
      if (line == this.lineSelected) {
        this.lineSelected = null;
      }
      this.orderLines.splice(index, 1);
      this.linesDirty = true;
    }
  }

  onEditLine(line) {
    if (this.lineSelected != null) {
      this.notify('error', 'Vui l??ng ho??n th??nh d???ch v??? hi???n t???i ????? ch???nh s???a d???ch v??? kh??c');
    } else {
      this.lineSelected = line;
      var viewChild = this.lineVCR.find(x => x.line == line);
      viewChild.onEditLine();
    }
  }

  updateLineInfo(value, index) {
    var line = this.orderLines[index];
    Object.assign(line, value);

    this.computeAmountLine([line]);
    this.computeAmountTotal();
    this.linesDirty = true;

    var valLine = {
      id: line.id,
      name: line.name,
      productId: line.product.id,
      priceUnit: line.priceUnit,
      productUOMQty: line.productUOMQty,
      employeeId: line.employee != null ? line.employee.id : null,
      assistantId: line.assistant != null ? line.assistant.id : null,
      counselorId: line.counselor != null ? line.counselor.id : null,
      toothIds: line.teeth.map(s => s.id),
      toothCategoryId: line.toothCategory != null ? line.toothCategory.id : null,
      diagnostic: line.diagnostic,
      toothType: line.toothType,
      isActive: line.isActive,
      orderId: this.saleOrder.id,
      date: moment(line.date).format('YYYY-MM-DD HH:mm'),
      state: line.state,
      productUOMId: line.productUOM ? line.productUOM.id : null
    };

    if (!line.id) {
      this.saleOrderLineService.create(valLine).subscribe(res => {
        line.id = res.id;
        this.notify('success', 'L??u th??nh c??ng');
        var viewChild = this.lineVCR.find(x => x.line == this.lineSelected);
        viewChild.onUpdateSignSubject.next(true);
        this.lineSelected = null;
      })
    }
    else {
      this.saleOrderLineService.update(line.id, valLine).subscribe(res => {
        this.notify('success', 'L??u th??nh c??ng');
        var viewChild = this.lineVCR.find(x => x.line == this.lineSelected);
        viewChild.onUpdateSignSubject.next(true);
        this.lineSelected = null;
      })
    }
  }

  computeAmountLine(lines) {
    lines.forEach(line => {
      line.priceSubTotal = (line.priceUnit - line.amountDiscountTotal) * line.productUOMQty;
    });
  }

  computeAmountTotal() {
    let total = 0;
    this.orderLines.forEach(line => {
      total += line.priceSubTotal;
    });
    // this.computeResidual(total);
    this.saleOrder.amountTotal = total;
  }


  onActiveLine(active, line) {
    if (active) {
      this.saleOrderLineService.patchIsActive(line.id, active).subscribe(() => {
        line.isActive = active;
      });
    } else {
      let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
      modalRef.componentInstance.title = 'Ng???ng d???ch v???';
      modalRef.componentInstance.body = 'B???n c?? mu???n ng???ng d???ch v??? kh??ng?';
      modalRef.componentInstance.body2 = '(L??u ??: Sau khi ng???ng kh??ng th??? ch???nh s???a d???ch v???)';
      modalRef.result.then(() => {
        this.saleOrderLineService.patchIsActive(line.id, active).subscribe(() => {
          line.isActive = active;
          this.notify('success', 'Ng???ng d???ch v??? th??nh c??ng');
        });
      });
    }
  }

  onUpdateStateLine(lineIndex, state) {
    if (this.lineSelected != null && this.lineSelected != line) {
      this.notify('error', 'Vui l??ng ho??n th??nh d???ch v??? hi???n t???i ????? ch???nh s???a d???ch v??? kh??c');
      return;
    }

    var line = this.orderLines[lineIndex];
    if (state == line.state) {
      return;
    }

    var stateTitle = {
      "cancel": "Ng???ng",
      "done": "Ho??n th??nh",
      "sale": "Ti???p t???c"
    };

    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = `${stateTitle[state]} d???ch v??? ${line.name}`;
    modalRef.componentInstance.body = `B???n c?? x??c nh???n ${(stateTitle[state] as string).toLocaleLowerCase()} d???ch v??? kh??ng?`;
    modalRef.componentInstance.body2 = state == 'cancel' ? "(L??u ??: Sau khi ng???ng kh??ng th??? ch???nh s???a d???ch v???)" : (state == "sale" ? "" : "(L??u ??: Sau khi ho??n th??nh kh??ng th??? ch???nh s???a, x??a d???ch v???)");
    modalRef.result.then(() => {
      this.saleOrderLineService.updateState(line.id, state).subscribe(() => {
        this.notify('success', 'L??u th??nh c??ng');
        line.state = state;

        if (this.orderLines.every(x => x.state == 'done' || x.state == 'cancel') &&
          this.orderLines.some(x => x.state == 'done')
        ) {
          this.saleOrder.state = 'done';
        } else if (this.orderLines.some(x => x.state == 'sale')) {
          this.saleOrder.state = 'sale';
        }

        if (state == "done" && line.priceSubTotal - line.amountInvoiced > 0) {
          let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
          modalRef.componentInstance.title = `Ghi c??ng n??? s??? ti???n ${this._decimalPipe.transform((line.priceSubTotal - line.amountInvoiced))}??`;
          modalRef.componentInstance.body = `D???ch v??? ${line.name} c??n ${this._decimalPipe.transform((line.priceSubTotal - line.amountInvoiced))}?? ch??a ???????c thanh to??n. B???n c?? mu???n ghi c??ng n??? s??? ti???n n??y?`;
          modalRef.componentInstance.confirmText = "?????ng ??";
          modalRef.componentInstance.closeText = "Kh??ng ?????ng ??";
          modalRef.componentInstance.closeClass = "btn-danger";
          modalRef.result.then(() => {
            this.saleOrderLineService.debtPayment(line.id)
              .subscribe(r => {
                this.notify('success', 'Ghi n??? th??nh c??ng');
                this.saleOrder.totalPaid = this.saleOrder.totalPaid + (line.priceSubTotal - line.amountInvoiced);
                this.loadPartnerDebt();
                line.amountInvoiced = line.priceSubTotal;
              });
          })

        }
      })
    }).catch(() => { });
  }

  computeOrderState() {
    if (this.orderLines.every(x => x.state == 'done' || x.state == 'cancel') &&
      this.orderLines.some(x => x.state == 'done')
    ) {
      this.saleOrder.state = 'done';
    } else if (this.orderLines.some(x => x.state == 'sale')) {
      this.saleOrder.state = 'sale';
    }
  }

  onLineActionDone(lineIndex) {
    if (this.lineSelected != null && this.lineSelected != line) {
      this.notify('error', 'Vui l??ng ho??n th??nh d???ch v??? hi???n t???i ????? ch???nh s???a d???ch v??? kh??c');
      return;
    }

    var line = this.orderLines[lineIndex];

    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = `Ho??n th??nh d???ch v??? ${line.name}`;
    modalRef.componentInstance.body = `B???n c?? x??c nh???n ho??n th??nh d???ch v??? kh??ng?`;
    modalRef.componentInstance.body2 = "(L??u ??: Sau khi ho??n th??nh kh??ng th??? ch???nh s???a, x??a d???ch v???)";
    modalRef.result.then(() => {
      this.saleOrderLineService.actionDone([line.id]).subscribe(() => {
        this.notify('success', 'L??u th??nh c??ng');
        line.state = 'done';

        this.computeOrderState();

        if (line.priceSubTotal - line.amountInvoiced > 0) {
          let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
          modalRef.componentInstance.title = `Ghi c??ng n??? s??? ti???n ${this._decimalPipe.transform((line.priceSubTotal - line.amountInvoiced))}??`;
          modalRef.componentInstance.body = `D???ch v??? ${line.name} c??n ${this._decimalPipe.transform((line.priceSubTotal - line.amountInvoiced))}?? ch??a ???????c thanh to??n. B???n c?? mu???n ghi c??ng n??? s??? ti???n n??y?`;
          modalRef.componentInstance.confirmText = "?????ng ??";
          modalRef.componentInstance.closeText = "Kh??ng ?????ng ??";
          modalRef.componentInstance.closeClass = "btn-danger";
          modalRef.result.then(() => {
            this.saleOrderLineService.debtPayment(line.id)
              .subscribe(r => {
                this.notify('success', 'Ghi n??? th??nh c??ng');
                this.saleOrder.totalPaid = this.saleOrder.totalPaid + (line.priceSubTotal - line.amountInvoiced);
                this.loadPartnerDebt();
                line.amountInvoiced = line.priceSubTotal;
              });
          })

        }
      })
    }).catch(() => { });
  }

  onLineActionCancel(lineIndex) {
    if (this.lineSelected != null && this.lineSelected != line) {
      this.notify('error', 'Vui l??ng ho??n th??nh d???ch v??? hi???n t???i ????? ch???nh s???a d???ch v??? kh??c');
      return;
    }

    var line = this.orderLines[lineIndex];

    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = `Ng???ng d???ch v??? ${line.name}`;
    modalRef.componentInstance.body = `B???n c?? x??c nh???n ng???ng d???ch v??? kh??ng?`;
    modalRef.componentInstance.body2 = "(L??u ??: Sau khi ng???ng kh??ng th??? ch???nh s???a, x??a d???ch v???)";
    modalRef.result.then(() => {
      this.saleOrderLineService.actionCancel([line.id]).subscribe(() => {
        this.notify('success', 'L??u th??nh c??ng');
        line.state = 'cancel';

        this.computeOrderState();
      })
    }).catch(() => { });
  }

  onLineActionUnlock(lineIndex) {
    if (this.lineSelected != null && this.lineSelected != line) {
      this.notify('error', 'Vui l??ng ho??n th??nh d???ch v??? hi???n t???i ????? ch???nh s???a d???ch v??? kh??c');
      return;
    }

    var line = this.orderLines[lineIndex];

    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = `Ti???p t???c d???ch v??? ${line.name}`;
    modalRef.componentInstance.body = `B???n c?? x??c nh???n ti???p t???c d???ch v??? kh??ng?`;
    modalRef.result.then(() => {
      this.saleOrderLineService.actionUnlock([line.id]).subscribe(() => {
        this.notify('success', 'L??u th??nh c??ng');
        line.state = 'sale';

        this.computeOrderState();
      })
    }).catch(() => { });
  }

  getAmountSubTotal() {
    //H??m tr??? v??? th??nh ti???n
    return this.orderLines.reduce((total, cur) => {
      return total + cur.priceUnit * cur.productUOMQty;
    }, 0);
  }

  getAmountDiscountTotal() {
    return this.orderLines.reduce((total, cur) => {
      return total + cur.amountDiscountTotal * cur.productUOMQty;
    }, 0);
  }

  getAmountInsurancePaidTotal() {
    return this.orderLines.reduce((total, cur) => {
      return total + (cur.amountInsurancePaidTotal || 0);
    }, 0);
  }

  getAmountTotal() {
    return this.getAmountSubTotal() - this.getAmountDiscountTotal();
  }

  getStateDisplay() {
    var state = this.saleOrder.state;
    switch (state) {
      case 'sale':
        return '??ang ??i???u tr???';
      case 'done':
        return 'Ho??n th??nh';
      default:
        return 'Nh??p';
    }
  }

  loadPartnerDebt() {
    this.loadPartnerDebt$().subscribe(res => {
      this.partnerDebt = res;
    });
  }

  loadPartnerDebt$() {
    var val = new AmountCustomerDebtFilter();
    val.partnerId = this.saleOrder.partnerId;
    val.companyId = this.authService.userInfo.companyId;
    return this.customerDebtReportService.getAmountDebtTotal(val);
  }

  loadPartnerInfo() {
    var loadPartner$ = this.partnerService.getCustomerInfo(this.saleOrder.partnerId);
    var loadDebt$ = this.loadPartnerDebt$();
    forkJoin({ partner: loadPartner$, debt: loadDebt$ }).subscribe(res => {
      this.partner = res.partner;
      this.partnerDebt = res.debt;
    });
  }

}
