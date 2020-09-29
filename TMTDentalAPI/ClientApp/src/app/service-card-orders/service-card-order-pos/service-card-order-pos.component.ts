
import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import * as _ from 'lodash';
import { debounceTime, mergeMap, switchMap, tap } from 'rxjs/operators';
import { AccountJournalFilter, AccountJournalService, AccountJournalSimple } from 'src/app/account-journals/account-journal.service';
import { AccountPaymentService } from 'src/app/account-payments/account-payment.service';
import { AuthService } from 'src/app/auth/auth.service';
import { PartnerPaged, PartnerSimple } from 'src/app/partners/partner-simple';
import { PartnerService } from 'src/app/partners/partner.service';
import { AccountInvoiceRegisterPaymentDialogV2Component } from 'src/app/shared/account-invoice-register-payment-dialog-v2/account-invoice-register-payment-dialog-v2.component';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PartnerCustomerCuDialogComponent } from 'src/app/shared/partner-customer-cu-dialog/partner-customer-cu-dialog.component';
import { UserSimple } from 'src/app/users/user-simple';
import { UserPaged, UserService } from 'src/app/users/user.service';
import { ServiceCardOrderLineDialogComponent } from '../service-card-order-line-dialog/service-card-order-line-dialog.component';
import { ServiceCardOrderLineService } from '../service-card-order-line.service';
import { ServiceCardOrderPaymentsDialogComponent } from '../service-card-order-payments-dialog/service-card-order-payments-dialog.component';
import { ServiceCardOrderService } from '../service-card-order.service';

@Component({
  selector: 'app-service-card-order-pos',
  templateUrl: './service-card-order-pos.component.html',
  styleUrls: ['./service-card-order-pos.component.css']
})
export class ServiceCardOrderPosComponent implements OnInit {
  cardOrder: any;
  formGroup: FormGroup;
  filteredPartners: PartnerSimple[];
  filteredUsers: UserSimple[];
  filteredJournals: AccountJournalSimple[];
  cusPayment: string;
  item: any;
  id: string;
  title = 'Đơn bán thẻ';
  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;
  @ViewChild('userCbx', { static: true }) userCbx: ComboBoxComponent;

  public keypressed;



  constructor(private fb: FormBuilder, private partnerService: PartnerService, private userService: UserService,
    private cardOrderService: ServiceCardOrderService, private route: ActivatedRoute,
    private intlService: IntlService, private router: Router,
    private notificationService: NotificationService, private modalService: NgbModal,
    private paymentService: AccountPaymentService,
    private accountJournalService: AccountJournalService,
    private authService: AuthService,
    private saleLineService: ServiceCardOrderLineService) { }

  ngOnInit() {
    this.cardOrder = {
      state: 'draft'
    };

    this.formGroup = this.fb.group({
      partner: [null, Validators.required],
      dateOrderObj: [null, Validators.required],
      user: null,
      orderLines: this.fb.array([]),
      cusPayments: this.fb.array([]),
      companyId: null,
      amountTotal: 0
    });


    this.route.queryParamMap.subscribe((param: ParamMap) => {
      this.id = param.get('id');
      if (this.id) {
        this.loadRecord();
      } else {
        this.cardOrder = {
          state: 'draft'
        };

        this.formGroup = this.fb.group({
          partner: [null, Validators.required],
          dateOrderObj: [null, Validators.required],
          user: null,
          orderLines: this.fb.array([]),
          cusPayments: this.fb.array([]),
          companyId: null,
          amountTotal: 0,
          custommerAmountPayment: 0,
          amountRefund: 0,
        });

        this.cardOrderService.defaultGet().subscribe((result: any) => {
          this.formGroup.patchValue(result);

          let dateOrder = new Date(result.dateOrder);
          this.formGroup.get('dateOrderObj').patchValue(dateOrder);
        });
      }
    });

    this.loadFilteredPartners();
    this.loadFilteredUsers();
    this.loadJournals();

    this.partnerCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.partnerCbx.loading = true)),
      switchMap(value => this.searchPartners(value))
    ).subscribe(result => {
      this.filteredPartners = result;
      this.partnerCbx.loading = false;
    });

    this.userCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.userCbx.loading = true)),
      switchMap(value => this.searchUsers(value))
    ).subscribe(result => {
      this.filteredUsers = result;
      this.userCbx.loading = false;
    });
  }

  @HostListener('document:keydown.f9', ['$event'])
  onKeydownHandler(event: KeyboardEvent) {
    if (event.keyCode === 120) {
      this.onKeyPressPayment();
    }

  }



  get orderLines() {
    return this.formGroup.get('orderLines') as FormArray;
  }

  get cusPayments() {
    return this.formGroup.get('cusPayments') as FormArray;
  }

  computeAmountTotal() {
    let total = 0;
    var lines = this.formGroup.get('orderLines') as FormArray;
    lines.controls.forEach(line => {
      total += line.get('priceSubTotal').value;
    });
    this.formGroup.get('amountTotal').patchValue(total);
  }

  computeAmountCustomerPaymentTotal() {
    let total = 0;
    if (this.cusPayments.controls.length > 0) {
      this.cusPayments.controls.forEach(line => {
        total += line.get('amount').value;
      });
      this.formGroup.get('custommerAmountPayment').patchValue(total);
    } else {
      this.formGroup.get('custommerAmountPayment').patchValue(this.amountTotalValue);
    }

  }

  get amountPaymentTotal() {
    let total = 0;
    if (this.cusPayments.controls.length > 0) {
      this.cusPayments.controls.forEach(line => {
        total += line.get('amount').value;
      });
    }
    return total;
  }

  computeAmountRefund() {
    var total = this.amountCustomerPaymentTotalValue - this.amountTotalValue;
    this.formGroup.get('amountRefund').patchValue(total);
  }

  loadRecord() {
    this.cardOrderService.get(this.id).subscribe((result: any) => {
      this.cardOrder = result;
      this.formGroup.patchValue(result);

      let dateOrder = new Date(result.dateOrder);
      this.formGroup.get('dateOrderObj').patchValue(dateOrder);

      this.filteredPartners = _.unionBy(this.filteredPartners, result.partner, 'id');
      if (result.user) {
        this.filteredUsers = _.unionBy(this.filteredUsers, result.user, 'id');
      }

      let control = this.formGroup.get('orderLines') as FormArray;
      control.clear();

      this.formGroup.markAsPristine();
    });
  }

  loadFilteredPartners() {
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

  loadFilteredUsers() {
    this.searchUsers().subscribe((result: any) => {
      this.filteredUsers = _.unionBy(this.filteredUsers, result, 'id');
    });
  }

  searchUsers(filter?: string) {
    var val = new UserPaged();
    val.search = filter || '';
    return this.userService.autocompleteSimple(val);
  }

  onSave() {
    if (!this.formGroup.valid) {
      return false;
    }

    var value = this.formGroup.value;
    value.partnerId = value.partner.id;
    value.userId = value.user ? value.user.id : null;
      value.dateOrder = this.intlService.formatDate(value.dateOrderObj, 'yyyy-MM-ddTHH:mm:ss');

    if (!this.id) {
      this.cardOrderService.create(value).subscribe((result: any) => {
        this.cardOrderService.actionConfirm([result.id]).subscribe(() => {
          this.loadRecord();
        });
      });
    }
  }

  onSaveConfirm() {
    if (!this.formGroup.valid) {
      return false;
    }

    if (this.amountTotalValue > this.amountCustomerPaymentTotalValue) {
      this.notificationService.show({
        content: 'Số tiền khách thanh toán nhỏ hơn tổng tiền',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });
      return false;
    }



    var value = this.formGroup.value;
    value.partnerId = value.partner.id;
    value.userId = value.user ? value.user.id : null;
    value.amountRefund = value.amountRefund;
    value.dateOrder = this.intlService.formatDate(value.dateOrderObj, 'yyyy-MM-ddTHH:mm:ss');

    if (!this.orderLines.length) {
      this.notificationService.show({
        content: 'Không tìm thấy loại thẻ nào',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });
      return false;
    }

    if (!this.cusPayments.length) {
      this.notificationService.show({
        content: 'Chọn phương thức thanh toán',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });
      return false;
    }


    this.cardOrderService.create(value)
      .subscribe((result: any) => {
        this.cardOrderService.actionConfirm([result.id]).subscribe(() => {
          // xử lý thanh toán
          this.cusPayments.controls.forEach(pay => {
            this.actionPayment(result.id, pay.value);
          })
          this.notificationService.show({
            content: 'thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
          this.resetForm();
        });

      });

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


  getPriceSubTotal() {
    this.orderLines.controls.forEach(line => {
      var discountType = line.get('discountType') ? line.get('discountType').value : 'percentage';
      var discountFixedValue = line.get('discountFixed').value;
      var discountNumber = line.get('discount').value;
      var getquanTity = line.get('productUOMQty').value;
      var priceUnit = line.get('priceUnit') ? line.get('priceUnit').value : 0;
      var price = discountType == 'percentage' ? priceUnit * (1 - discountNumber / 100) :
        Math.max(0, priceUnit - discountFixedValue);
      var subtotal = price * getquanTity;
      line.get('priceSubTotal').setValue(subtotal);
    });

  }

  onChangeQuantity(line: FormGroup) {
    var res = this.orderLines.controls.find(x => x.value.cardType.id === line.value.cardType.id);
    if (res) {
      res.patchValue(line.value);
    }
    this.getPriceSubTotal();
    this.computeAmountTotal();
    this.computeAmountCustomerPaymentTotal();
    this.computeAmountRefund();
  }

  onChangeDiscountFixed(line: FormGroup) {
    var res = this.orderLines.controls.find(x => x.value.cardType.id === line.value.cardType.id);
    if (res) {
      res.patchValue(line.value);
    }
    this.getPriceSubTotal();
    this.computeAmountTotal();
    this.computeAmountCustomerPaymentTotal();
    this.computeAmountRefund();
  }

  onChangeDiscount(line: FormGroup) {
    var res = this.orderLines.controls.find(x => x.value.cardType.id === line.value.cardType.id);
    if (res) {
      res.patchValue(line.value);
    }
    this.getPriceSubTotal();
    this.computeAmountTotal();
    this.computeAmountCustomerPaymentTotal();
    this.computeAmountRefund();
  }

  onChangeDiscountType(line: FormGroup) {
    var res = this.orderLines.controls.find(x => x.value.cardType.id === line.value.cardType.id);
    if (res) {
      res.value.discount = 0;
      res.value.discountFixed = 0;
      res.patchValue(line.value);
    }

    this.getPriceSubTotal();
    this.computeAmountTotal();
    this.computeAmountCustomerPaymentTotal();
    this.computeAmountRefund();
  }

  onChangeCustomerPaymentTotal(mount) {
    this.formGroup.get('custommerAmountPayment').patchValue(mount);
    let controlpayment = this.formGroup.get('cusPayments') as FormArray;
    controlpayment.clear();
    this.computeAmountTotal();
    this.computeAmountRefund();
  }

  addLineModal(val) {
    var res = this.fb.group({
      cardTypeId: val.id,
      cardType: val,
      priceUnit: val.price,
      productUOMQty: 1,
      discount: 0,
      discountType: 'percentage',
      discountFixed: 0,
      priceSubTotal: 0
    });

    if (!this.orderLines.controls.some(x => x.value.cardType.id === res.value.cardType.id)) {
      this.orderLines.push(res);
    } else {
      var line = this.orderLines.controls.find(x => x.value.cardType.id === res.value.cardType.id);
      if (line) {
        line.value.productUOMQty += 1;
        line.patchValue(line.value);
      }
    }

    this.getPriceSubTotal();
    this.orderLines.markAsDirty();
    this.computeAmountTotal();
    this.computeAmountCustomerPaymentTotal();
    this.computeAmountRefund();

  }


  get amountTotalValue() {
    return this.formGroup.get('amountTotal').value;
  }

  get amountCustomerPaymentTotalValue() {
    return this.formGroup.get('custommerAmountPayment').value;
  }

  get amountRefundTotalValue() {
    return this.formGroup.get('amountRefund').value;
  }

  deleteLine(index: number) {
    this.orderLines.removeAt(index);
    this.getPriceSubTotal();
    this.orderLines.markAsDirty();
    this.computeAmountTotal();
    this.computeAmountCustomerPaymentTotal();
    this.computeAmountRefund();
  }



  //---Payment--//
  loadJournals() {
    this.searchJournals().subscribe(result => {
      this.filteredJournals = result;
    })
  }

  searchJournals(search?: string) {
    var val = new AccountJournalFilter();
    val.type = 'bank,cash';
    val.search = search || '';
    val.companyId = this.authService.userInfo.companyId;
    return this.accountJournalService.autocomplete(val);
  }



  actionPayment(id, val: any) {
    var pay = this.fb.group({
      amount: 0,
      paymentDateObj: [null, Validators.required],
      paymentDate: null,
      communication: null,
      paymentType: null,
      journalId: null,
      journal: [null, Validators.required],
      partnerType: null,
      partnerId: null,
      invoiceIds: null,
      saleOrderIds: null,
      serviceCardOrderIds: null,
    });

    if (id) {
      this.paymentService.serviceCardOrderDefaultGet([id]).subscribe(rs2 => {
        pay.patchValue(rs2);
        pay.value.amount = val.amount;
        pay.value.journal = val.journal;
        pay.value.journalId = val.journalId;
        pay.value.paymentDateObj = new Date();
        pay.value.paymentDate = this.intlService.formatDate(pay.value.paymentDateObj, 'd', 'en-US');
        this.paymentService.create(pay.value).subscribe((result: any) => {
          this.paymentService.post([result.id]).subscribe(() => {
          }, (err) => {
            this.notificationService.show({
              content: err,
              hideAfter: 3000,
              position: { horizontal: 'center', vertical: 'top' },
              animation: { type: 'fade', duration: 400 },
              type: { style: 'error', icon: true }
            });
          });
        }, (err) => {
          this.notificationService.show({
            content: err,
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'error', icon: true }
          });
        });

      })
    }
  }




  resetForm() {
    let dateOrder = new Date();
    this.formGroup.get('partner').patchValue(null);
    this.formGroup.get('dateOrderObj').patchValue(dateOrder);
    let control = this.formGroup.get('orderLines') as FormArray;
    control.clear();

    this.getPriceSubTotal();
    this.computeAmountTotal();
    this.computeAmountCustomerPaymentTotal();
    this.computeAmountRefund();
    this.formGroup.markAsPristine();

  }

  onKeyPressPayment() {

    if (this.formGroup.value.partner == null) {
      this.notificationService.show({
        content: 'Chọn khách hàng trước khi thanh toán',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });
      return false;
    }

    if (!this.orderLines.length) {
      this.notificationService.show({
        content: 'Không tìm thấy loại thẻ trong đơn bán thẻ để thanh toán',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });
      return false;
    }


    // 120 is the F9 key
    let modalRef = this.modalService.open(ServiceCardOrderPaymentsDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thanh toán';
    modalRef.componentInstance.defaultVal = this.formGroup.value;
    modalRef.result.then(res => {
      debugger
      console.log(res);
      var value = this.formGroup.value;
      value.partnerId = value.partner.id;
      value.userId = value.user ? value.user.id : null;
      value.amountRefund = Math.abs(res.controls[res.length - 1].value.amount);
      value.dateOrder = this.intlService.formatDate(value.dateOrderObj, 'yyyy-MM-ddTHH:mm:ss');
      this.cardOrderService.create(value)
        .subscribe((result: any) => {
          this.cardOrderService.actionConfirm([result.id]).subscribe(() => {
            // xử lý thanh toán
            res.controls.forEach(pay => {
              this.actionPayment(result.id, pay.value);
            })
            this.notificationService.show({
              content: 'thành công',
              hideAfter: 3000,
              position: { horizontal: 'center', vertical: 'top' },
              animation: { type: 'fade', duration: 400 },
              type: { style: 'success', icon: true }
            });
            this.resetForm();
          });

        });
    }, () => {
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
    }, () => {
    });
  }

}
