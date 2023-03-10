import { Component, OnInit, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { BehaviorSubject, forkJoin } from 'rxjs';
import { catchError, mergeMap } from 'rxjs/operators';
import { PaymentInfoContent } from 'src/app/account-invoices/account-invoice.service';
import { AccountPaymentBasic } from 'src/app/account-payments/account-payment.service';
import { AuthService } from 'src/app/auth/auth.service';
import { AmountCustomerDebtFilter, CustomerDebtReportService } from 'src/app/core/services/customer-debt-report.service';
import { SaleOrderPaymentService } from 'src/app/core/services/sale-order-payment.service';
import { LaboOrderBasic } from 'src/app/labo-orders/labo-order.service';
import { PartnerSimple } from 'src/app/partners/partner-simple';
import { PartnerService } from 'src/app/partners/partner.service';
import { ProductPriceListBasic } from 'src/app/price-list/price-list';
import { AppointmentCreateUpdateComponent } from 'src/app/shared/appointment-create-update/appointment-create-update.component';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PrintService } from 'src/app/shared/services/print.service';
import { SmsMessageService } from 'src/app/sms/sms-message.service';
import { ToaThuocCuDialogComponent } from 'src/app/toa-thuocs/toa-thuoc-cu-dialog/toa-thuoc-cu-dialog.component';
import { ToaThuocService } from 'src/app/toa-thuocs/toa-thuoc.service';
import { ToothCategoryBasic, ToothCategoryService } from 'src/app/tooth-categories/tooth-category.service';
import { UserSimple } from 'src/app/users/user-simple';
import { SaleOrderLineService } from '../../core/services/sale-order-line.service';
import { DiscountDefault, SaleOrderPrint, SaleOrderService } from '../../core/services/sale-order.service';
import { PartnerCustomerToathuocListComponent } from '../partner-customer-toathuoc-list/partner-customer-toathuoc-list.component';
import { SaleOrderLineCuComponent } from '../sale-order-line-cu/sale-order-line-cu.component';
import { SaleOrderLinePromotionDialogComponent } from '../sale-order-line-promotion-dialog/sale-order-line-promotion-dialog.component';
import { SaleOrderPaymentDialogComponent } from '../sale-order-payment-dialog/sale-order-payment-dialog.component';
import { SaleOrderPaymentListComponent } from '../sale-order-payment-list/sale-order-payment-list.component';
import { SaleOrderPrintPopupComponent } from '../sale-order-print-popup/sale-order-print-popup.component';
import { SaleOrderPromotionDialogComponent } from '../sale-order-promotion-dialog/sale-order-promotion-dialog.component';
import { SaleOrderPromotionService } from '../sale-order-promotion.service';
import { SaleOrderServiceListComponent } from '../sale-order-service-list/sale-order-service-list.component';
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
  linesDirty = false;
  isCollapsed = false;

  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;
  @ViewChild('userCbx', { static: true }) userCbx: ComboBoxComponent;
  @ViewChild('pricelistCbx', { static: true }) pricelistCbx: ComboBoxComponent;
  @ViewChild('employeeCbx', { static: false }) employeeCbx: ComboBoxComponent;
  @ViewChild('assistantCbx', { static: false }) assistantCbx: ComboBoxComponent;
  @ViewChild('counselorCbx', { static: false }) counselorCbx: ComboBoxComponent;
  @ViewChild('toathuocComp', { static: false }) toathuocComp: PartnerCustomerToathuocListComponent;
  @ViewChild('paymentComp', { static: false }) paymentComp: SaleOrderPaymentListComponent;
  @ViewChild('serviceListComp', { static: false }) serviceListComp: SaleOrderServiceListComponent;

  partner: any;
  partnerDisplay: any;
  saleOrder: any;
  saleOrderPrint: SaleOrderPrint;
  laboOrders: LaboOrderBasic[] = [];
  saleOrderLine: any;
  payments: AccountPaymentBasic[] = [];
  paymentsInfo: PaymentInfoContent[] = [];

  searchCardBarcode: string;
  listTeeths: any[] = [];
  filteredToothCategories: any[] = [];
  initialListEmployees: any[] = [];
  type: string;
  submitted = false;
  amountAdvanceBalance: number = 0;
  defaultToothCate: ToothCategoryBasic;
  tags: any[] = [];
  checkedAccordion: boolean = true;
  childEmiter = new BehaviorSubject<any>(null);
  @ViewChildren('lineTemplate') lineVCR: QueryList<SaleOrderLineCuComponent>;
  lineSelected = null;
  toothTypeDict = [
    { name: "H??m tr??n", value: "upper_jaw" },
    { name: "Nguy??n h??m", value: "whole_jaw" },
    { name: "H??m d?????i", value: "lower_jaw" },
    { name: "Ch???n r??ng", value: "manual" },
  ];
  // active = 'home-tab'; 
  // saleOrderSimple: any;
  constructor(
    private fb: FormBuilder,
    private partnerService: PartnerService,
    public route: ActivatedRoute,
    private saleOrderService: SaleOrderService,
    private saleOrderLineService: SaleOrderLineService,
    private intlService: IntlService,
    private modalService: NgbModal,
    private router: Router,
    private notificationService: NotificationService,
    private toaThuocService: ToaThuocService,
    private printService: PrintService,
    private toothCategoryService: ToothCategoryService,
    private saleOrderPromotionService: SaleOrderPromotionService,
    private smsMessageService: SmsMessageService,
    private saleOrderPaymentService: SaleOrderPaymentService,
    private authService: AuthService,
    private customerDebtReportService: CustomerDebtReportService
  ) {
  }

  ngOnInit() {
    this.saleOrderId = this.route.snapshot.paramMap.get('id');
    this.route.data.subscribe((response: any) => {
      this.saleOrder = response.saleOrder
    });

    this.formGroup = this.fb.group({
      dateOrderObj: [null, Validators.required],
    });
  }

  onCancelPayment() {
    this.loadCustomerInfo();
    this.loadSaleOrder();
  }

  loadCustomerInfo() {
    if (this.saleOrder.partnerId) {
      this.partnerService.getCustomerInfo(this.saleOrder.partnerId).subscribe((result) => {
        this.partner = result;
        this.tags = [];
        this.partner.categories.forEach(item => {
          var category = {
            Id: item.id,
            Name: item.name,
            CompleteName: item.completeName,
            Color: item.color
          };
          this.tags.push(category);
        });
      });
    }
  }




  updateFormGroup(result) {
    // this.saleOrder = result;
    // this.formGroup.patchValue(result);
    let dateOrder = new Date(result.dateOrder);
    this.formGroup.get('dateOrderObj').patchValue(dateOrder);
    // if (result.User) {
    //   this.filteredUsers = _.unionBy(this.filteredUsers, [result.user], 'id');
    // }
    // if (result.Partner) {
    //   this.filteredPartners = _.unionBy(this.filteredPartners, [result.partner], 'id');
    //   if (!this.saleOrderId) {
    //     this.onChangePartner(result.partner);
    //   }
    // }

    // this.formGroup.setControl('promotions', this.fb.array(result.promotions));

    // let control = this.formGroup.get('orderLines') as FormArray;
    // control.clear();
    // result.orderLines.forEach(line => {
    //   var g = this.fb.group(line);
    //   g.setControl('teeth', this.fb.array(line.teeth));
    //   g.setControl('promotions', this.fb.array(line.promotions));
    //   control.push(g);
    // });

    // this.formGroup.markAsPristine();
    // this.getAmountAdvanceBalance();
  }

  get f() {
    return this.formGroup.controls;
  }

  // routeActive() {
  //   this.route.params.subscribe(
  //     () => {
  //       this.route.queryParamMap.pipe(
  //         switchMap((params: ParamMap) => {
  //           this.saleOrderId = params.get("id");
  //           this.partnerId = params.get("partner_id");
  //           if (this.saleOrderId) {
  //             return this.saleOrderService.get(this.saleOrderId);
  //           } else {
  //             return this.saleOrderService.defaultGet({ partnerId: this.partnerId || '' });
  //           }
  //         })).subscribe((result: any) => {
  //           this.patchValueSaleOrder(result);
  //           this.isChanged = false;
  //         });
  //     }
  //   )
  // }

  get stateControl() {
    return this.saleOrder.state;
  }


  get customerId() {
    // var parterIdParam = this.route.snapshot.queryParamMap.get('partner_id');
    if (this.saleOrder?.partnerId) {
      return this.saleOrder?.partnerId;
    }

    if (this.saleOrder && this.saleOrder.partnerId) {
      return this.saleOrder?.partnerId;
    }

    return undefined;
  }

  loadToothCateDefault() {
    this.toothCategoryService.getDefaultCategory().subscribe(
      res => {
        this.defaultToothCate = res;
      }
    );
  }

  getAmountAdvanceBalance() {
    this.partnerService.getAmountAdvanceBalance(this.partner.id).subscribe(result => {
      this.amountAdvanceBalance = result;
    })
  }

  onUpdateOrder(data) {
    this.saleOrder.dateOrder = data.dateOrder;
    this.saleOrder.doctorName = data.doctor ? data.doctor.name : null;
  }

  printSaleOrder() {
    let modalRef = this.modalService.open(SaleOrderPrintPopupComponent, { size: 'md' });
    modalRef.componentInstance.id = this.saleOrder.id;
    modalRef.result.then(result => {
      if (this.saleOrderId && result) {
        var attachIds = result.map(x => x.id);
        var val = new SaleOrderPrint();
        val.id = this.saleOrder.id;
        val.attachmentIds = attachIds;
        this.saleOrderService.printSaleOrder(val).subscribe((result: any) => {
          this.printService.printHtml(result.html);
        });
      }
    })
  }

  actionDone() {
    if (this.saleOrder.id) {
      let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
      modalRef.componentInstance.title = 'Ho??n th??nh phi???u ??i???u tr???';
      modalRef.componentInstance.body = 'B???n c?? ch???c ch???n mu???n ho??n th??nh ??i???u tr????';
      modalRef.result.then(() => {
        this.saleOrderService.actionDone([this.saleOrderId]).subscribe(() => {
          this.loadSaleOrder();
          this.notify('success', 'Ho??n th??nh phi???u ??i???u tr???');
          this.smsMessageService.SetupSendSmsOrderAutomatic(this.saleOrderId).subscribe(
            () => { }
          )
        });
      });
    }
  }

  actionCancel() {
    if (this.saleOrderId) {
      this.saleOrderService.actionCancel([this.saleOrderId]).subscribe(() => {
        this.router.navigate([], { fragment: 'services', relativeTo: this.route })
        this.loadSaleOrder();
      });
    }
  }

  actionUnlock() {
    if (this.saleOrderId) {
      this.saleOrderService.actionUnlock([this.saleOrderId]).subscribe(() => {
        this.loadSaleOrder();
      });
    }
  }

  getFormDataSave() {
    var val = {
      dateOrder: this.saleOrder.dateOrder,
      partnerId: this.saleOrder.partnerId,
      companyId: this.saleOrder.companyId,
      // orderLines: this.saleOrder.orderLines.map(x => {
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

  onSaveConfirm() {
    if (this.serviceListComp.lineSelected != null) { //N???u d??? li???u c???n l??u l???i
      var viewChild = this.serviceListComp.lineVCR.find(x => x.line == this.serviceListComp.lineSelected);
      var rs = viewChild.updateLineInfo();
      if (rs) {
        viewChild.onUpdateSignSubject.subscribe(value => {
          if (value) {
            this.saleOrderService.actionConfirm([this.saleOrder.id]).subscribe(() => {
              this.notify('success', 'X??c nh???n th??nh c??ng');
              this.loadSaleOrder();
            })
          }
        })
      }
    } else {
      this.saleOrderService.actionConfirm([this.saleOrder.id]).subscribe(() => {
        this.notify('success', 'X??c nh???n th??nh c??ng');
        this.loadSaleOrder();
      })
    }
  }

  updateFormGroupDataToSaleOrder() {
    var value = this.formGroup.value;
    this.saleOrder.dateOrder = this.intlService.formatDate(value.dateOrderObj, 'yyyy-MM-ddTHH:mm:ss');
  }

  onSave() {
    // this.submitted = true;
    // if (!this.formGroup.valid) {
    //   return false;
    // }

    this.updateFormGroupDataToSaleOrder();
    const val = this.getFormDataSave();

    if (this.saleOrderId) {
      this.saleOrderService.update(this.saleOrderId, val).subscribe((res) => {
        if (this.lineSelected != null) {
          var viewChild = this.lineVCR.find(x => x.line == this.lineSelected);
          var rs = viewChild.updateLineInfo();
          if (!rs) return;
        } else {
          this.notify('success', 'L??u th??nh c??ng');
          this.loadSaleOrder();
        }
      }, (error) => {
        this.loadSaleOrder();

      });
    } else {
      this.saleOrderService.create(val).subscribe((result: any) => {
        this.saleOrderId = result.id;
        //update line tr?????c khi l??u
        if (this.lineSelected != null) {
          this.router.navigate(['/sale-orders/form'], { queryParams: { id: result.id } });
          var viewChild = this.lineVCR.find(x => x.line == this.lineSelected);
          var rs = viewChild.updateLineInfo();
          if (!rs) return;
        } else {
          this.notify('success', 'L??u th??nh c??ng');
          this.router.navigate(['/sale-orders/form'], { queryParams: { id: result.id } });
          this.loadSaleOrder();
        }
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

  // patchValueSaleOrder(result) {
  //   this.saleOrder = result;
  //   this.formGroup.patchValue(result);
  //   let dateOrder = new Date(result.dateOrder);
  //   this.formGroup.get('dateOrderObj').patchValue(dateOrder);

  //   if (result.User) {
  //     this.filteredUsers = _.unionBy(this.filteredUsers, [result.user], 'id');
  //   }
  //   if (result.Partner) {
  //     this.filteredPartners = _.unionBy(this.filteredPartners, [result.partner], 'id');
  //     if (!this.saleOrderId) {
  //       this.onChangePartner(result.partner);
  //     }
  //   }

  //   this.formGroup.setControl('promotions', this.fb.array(result.promotions));

  //   let control = this.formGroup.get('orderLines') as FormArray;
  //   control.clear();
  //   result.orderLines.forEach(line => {
  //     var g = this.fb.group(line);
  //     g.setControl('teeth', this.fb.array(line.teeth));
  //     g.setControl('promotions', this.fb.array(line.promotions));
  //     control.push(g);
  //   });

  //   this.formGroup.markAsPristine();
  //   this.getAmountAdvanceBalance();
  // }


  // async loadRecord() {
  //   if (this.saleOrderId) {
  //     //  this.saleOrderService.get(this.saleOrderId).subscribe((result: any) => {
  //     //     this.patchValueSaleOrder(result);
  //     //     this.saleOrder = result;
  //     //     return result;
  //     //   });
  //     var result = await this.saleOrderService.get(this.saleOrderId).toPromise();
  //     this.patchValueSaleOrder(result);
  //     this.saleOrder = result;
  //     this.isChanged = false;
  //     return result;
  //   }
  // }

  loadSaleOrder() {
    // this.saleOrderService.get(this.saleOrderId).subscribe(res => {
    //   this.resetData(res);
    // });
    this.saleOrderService.getBasic(this.saleOrder.id).subscribe(res => {
      this.saleOrder = res;
    });
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

  actionSaleOrderPayment() {
    if (this.saleOrderId) {
      var val = new AmountCustomerDebtFilter();
      val.partnerId = this.saleOrder.partnerId;
      val.companyId = this.authService.userInfo.companyId;
      var loadDebt$ = this.customerDebtReportService.getAmountDebtTotal(val);
      var loadSaleOrder$ = this.saleOrderService.getSaleOrderPaymentBySaleOrderId(this.saleOrderId);
      forkJoin({ partnerDebt: loadDebt$, payment: loadSaleOrder$ }).subscribe(rs => {
        let modalRef = this.modalService.open(SaleOrderPaymentDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
        modalRef.componentInstance.title = 'Thanh to??n';
        modalRef.componentInstance.defaultVal = rs.payment;
        modalRef.componentInstance.advanceAmount = this.amountAdvanceBalance;
        modalRef.componentInstance.partnerId = this.saleOrder.partnerId;
        modalRef.componentInstance.partnerName = this.saleOrder.partnerName;
        modalRef.componentInstance.partnerDebt = (rs.partnerDebt as any).balanceTotal;

        modalRef.result.then(result => {
          this.notificationService.show({
            content: 'Thanh to??n th??nh c??ng',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });

          this.loadSaleOrder();
          if (this.serviceListComp) // load l???i c??ng n???
          {
            this.serviceListComp.loadPartnerDebt();
          }
          if (this.paymentComp) {
            this.paymentComp.loadPayments();
          }

          if (result.print) {
            this.printPayment(result.paymentId)
          }
        }, () => {
        });
      })
    }
  }

  printPayment(paymentId) {
    this.saleOrderPaymentService.getPrint(paymentId).subscribe((result: any) => {
      this.printService.printHtml(result.html);
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
    if (this.saleOrder) {
      let modalRef = this.modalService.open(ToaThuocCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = 'Th??m: ????n Thu???c';
      modalRef.componentInstance.defaultVal = { partnerId: this.saleOrder.partnerId, saleOrderId: this.saleOrder.id };
      modalRef.result.then((result: any) => {
        this.notify('success', 'T???o toa thu???c th??nh c??ng');
        if (this.toathuocComp) {
          this.toathuocComp.loadData();
        }
        if (result.print) {
          this.printToaThuoc(result.item);
        }
      }, () => {
      });
    }
  }

  dialogAppointment() {
    const modalRef = this.modalService.open(AppointmentCreateUpdateComponent, { size: 'xl', windowClass: 'o_technical_modal modal-appointment', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.defaultVal = { partnerId: (this.saleOrder.partnerId), saleOrderId: this.saleOrderId };
    modalRef.result.then(() => {
      this.notify('success', 'T???o l???ch h???n th??nh c??ng');
    }, () => {
    });
  }

  onDeleteLine(index) {
    var line = this.saleOrder.orderLines[index];
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
          this.notify('success', 'X??a d???ch v??? th??nh c??ng');
          this.loadSaleOrder();
        })
      });
    } else {
      if (line == this.lineSelected) {
        this.lineSelected = null;
      }
      this.saleOrder.orderLines.splice(index, 1);
      this.linesDirty = true;
    }
  }

  resetFormPristine() {
    this.linesDirty = false;
    this.formGroup.markAsPristine();
  }

  resetData(data) {
    this.saleOrder = data;
    this.resetFormPristine();
  }

  openSaleOrderPromotionDialog() {
    let modalRef = this.modalService.open(SaleOrderPromotionDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
    modalRef.componentInstance.saleOrder = this.saleOrder;

    modalRef.componentInstance.getBtnDiscountObs().subscribe(data => {
      var val = {
        id: this.saleOrder.id,
        discountType: data.discountType,
        discountPercent: data.discountPercent,
        discountFixed: data.discountFixed,
      };

      this.saleOrderService.applyDiscountOnOrder(val).pipe(
        mergeMap(() => this.saleOrderService.get(this.saleOrderId))
      )
        .subscribe(res => {
          this.resetData(res);
          modalRef.componentInstance.saleOrder = this.saleOrder;
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
          return this.saleOrderService.get(this.saleOrderId);
        })
      )
        .subscribe(res => {
          this.resetData(res);
          modalRef.componentInstance.saleOrder = this.saleOrder;
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
          return this.saleOrderService.get(this.saleOrderId);
        })
      )
        .subscribe(res => {
          this.resetData(res);
          modalRef.componentInstance.saleOrder = this.saleOrder;
        }, err => {
          console.log(err);
          this.notify('error', err.error.error);
        });
    });

    modalRef.componentInstance.getBtnDeletePromoObs().subscribe(data => {
      this.saleOrderPromotionService.removePromotion([data.id]).pipe(
        catchError((err) => { throw err; }),
        mergeMap((result: any) => {
          return this.saleOrderService.get(this.saleOrderId);
        })
      )
        .subscribe(res => {
          this.resetData(res);
          modalRef.componentInstance.saleOrder = this.saleOrder;
          this.notify('success', "X??a khuy???n m??i th??nh c??ng");
        }, err => {
          console.log(err);
          this.notify('error', err.error.error);
        });
    });
  }

  isDataChanged() {
    return this.formGroup.dirty || this.linesDirty;
  }

  onOpenSaleOrderPromotion() {
    var getData_OpenDialog = () => {
      //N???u c?? chi ti???t ??ang ch???nh s???a th?? c???p nh???t
      if (this.lineSelected != null) {
        var viewChild = this.lineVCR.find(x => x.line == this.lineSelected);
        var rs = viewChild.updateLineInfo();
        if (rs) {
          viewChild.onUpdateSignSubject.subscribe(value => {
            this.saleOrderService.get(this.saleOrderId).subscribe((rs: any) => {
              this.resetData(rs);
              this.openSaleOrderPromotionDialog();
            });
          })
        }
      } else {
        this.saleOrderService.get(this.saleOrderId).subscribe((rs: any) => {
          this.resetData(rs);
          this.openSaleOrderPromotionDialog();
        });
      }
    }
    //N???u phi???u ??i???u tr??? ch??a l??u
    if (!this.saleOrderId) {
      if (!this.formGroup.valid) {
        return false;
      }

      this.updateFormGroupDataToSaleOrder();
      const val = this.getFormDataSave();

      this.saleOrderService.create(val).subscribe((result: any) => {
        this.saleOrderId = result.id;
        this.router.navigate(['/sale-orders/form'], { queryParams: { id: result.id } });
        getData_OpenDialog();

      });
    } else if (this.lineSelected != null) { //N???u d??? li???u c???n l??u l???i
      if (!this.formGroup.valid) {
        return false;
      }

      this.updateFormGroupDataToSaleOrder();
      const val = this.getFormDataSave();

      this.saleOrderService.update(this.saleOrderId, val).subscribe((result: any) => {
        getData_OpenDialog();
      });
    } else {
      this.openSaleOrderPromotionDialog();
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

  onCancelEditLine(line, index) {
    if (!line.id)
      this.saleOrder.orderLines.splice(index, 1);
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

  onOpenLinePromotionDialog(i) {
    var line = this.saleOrder.orderLines[i];
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
        mergeMap(() => this.saleOrderService.get(this.saleOrderId))
      )
        .subscribe(res => {
          this.resetData(res);
          var newLine = this.saleOrder.orderLines[i];
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
          return this.saleOrderService.get(this.saleOrderId);
        })
      )
        .subscribe(res => {
          this.resetData(res);
          var newLine = this.saleOrder.orderLines[i];
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
          return this.saleOrderService.get(this.saleOrderId);
        })
      )
        .subscribe(res => {
          this.resetData(res);
          var newLine = this.saleOrder.orderLines[i];
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
          return this.saleOrderService.get(this.saleOrderId);
        })
      )
        .subscribe(res => {
          this.resetData(res);
          var newLine = this.saleOrder.orderLines[i];
          modalRef.componentInstance.saleOrderLine = newLine;
          this.notify('success', "X??a khuy???n m??i th??nh c??ng");
        }, err => {
          console.log(err);
          this.notify('error', err.error.error);
        });
    });
  }

  onUpdateOpenLinePromotion(i) {
    var getData_OpenDialog = () => {
      //N???u c?? chi ti???t ??ang ch???nh s???a th?? c???p nh???t

      if (this.lineSelected != null) {
        var viewChild = this.lineVCR.find(x => x.line == this.lineSelected);
        var rs = viewChild.updateLineInfo();
        if (rs) {
          viewChild.onUpdateSignSubject.subscribe(value => {
            this.saleOrderService.get(this.saleOrderId).subscribe(result => {
              this.resetData(result);
              this.onOpenLinePromotionDialog(i);
            });
          })
        }
      } else {
        this.saleOrderService.get(this.saleOrderId).subscribe(result => {
          this.resetData(result);
          this.onOpenLinePromotionDialog(i);
        });
      }
    }
    //N???u phi???u ??i???u tr??? ch??a l??u
    if (!this.saleOrderId) {
      if (!this.formGroup.valid) {
        return false;
      }

      this.updateFormGroupDataToSaleOrder();
      const val = this.getFormDataSave();

      this.saleOrderService.create(val).subscribe((r: any) => {
        this.saleOrderId = r.id;
        this.router.navigate(['/sale-orders/form'], { queryParams: { id: r.id } });
        getData_OpenDialog();
      });
    } else if (this.lineSelected != null) { //N???u d??? li???u c???n l??u l???i
      if (!this.formGroup.valid) {
        return false;
      }

      this.updateFormGroupDataToSaleOrder();
      const val = this.getFormDataSave();

      this.saleOrderService.update(this.saleOrderId, val).subscribe((result: any) => {
        getData_OpenDialog();
      });
    } else {
      this.onOpenLinePromotionDialog(i);
    }
    // //update line tr?????c khi l??u
    // if (this.lineSelected != null) {
    //   var viewChild = this.lineVCR.find(x => x.line == this.lineSelected);
    //   var rs = viewChild.updateLineInfo();
    //   if(!rs) return;
    // }
    // //n???u data kh??ng change th?? m??? dialog lu??n
    // if (!this.isChanged) {
    //   this.onOpenLinePromotionDialog(i);
    //   return;
    // }
    // // this.updateLineInfo(line, lineControl);// l??u ??? client
    // const val = this.getFormDataSave();
    // if (!this.saleOrderId) {
    //   this.submitted = true;
    //   if (!this.formGroup.valid) {
    //     return false;
    //   }

    //   this.saleOrderService.create(val).subscribe(async (result: any) => {
    //     this.saleOrderId = result.id;
    //     const url = this.router.createUrlTree([], { queryParams: {id: result.id}}).toString()
    //     this.location.go(url);
    //     // this.saleOrderService.get(this.saleOrderId).subscribe((result: any) => {
    //     //   this.patchValueSaleOrder(result);
    //     //   this.onOpenLinePromotionDialog(i);

    //     // });
    //     await this.loadRecord();
    //     this.onOpenLinePromotionDialog(i);
    //   })
    // } else {

    //   this.saleOrderService.update(this.saleOrderId, val).subscribe(async (result: any) => {
    //     // this.saleOrderService.get(this.saleOrderId).subscribe((result: any) => {
    //     //   this.patchValueSaleOrder(result);
    //     //   this.onOpenLinePromotionDialog(i);

    //     // });
    //     await this.loadRecord();
    //     this.onOpenLinePromotionDialog(i);
    //   });
    // }
  }

  insurancePayment() {
    this.loadSaleOrder();
  }
}

