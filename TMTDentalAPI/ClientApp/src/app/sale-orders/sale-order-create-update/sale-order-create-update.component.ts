import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, FormBuilder, FormArray, Validators } from '@angular/forms';
import { debounceTime, switchMap, tap, map } from 'rxjs/operators';
import { PartnerSimple, PartnerPaged } from 'src/app/partners/partner-simple';
import { PartnerService } from 'src/app/partners/partner.service';
import { UserService, UserPaged } from 'src/app/users/user.service';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { SaleOrderService, AccountPaymentFilter } from '../sale-order.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { ProductService, ProductFilter } from 'src/app/products/product.service';
import { IntlService } from '@progress/kendo-angular-intl';
import { WindowService, WindowCloseResult } from '@progress/kendo-angular-dialog';
import { SaleOrderDisplay } from '../sale-order-display';
import * as _ from 'lodash';
import { UserSimple } from 'src/app/users/user-simple';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { SaleOrderLineDialogComponent } from '../sale-order-line-dialog/sale-order-line-dialog.component';
import { NotificationService } from '@progress/kendo-angular-notification';
import { SaleOrderCreateDotKhamDialogComponent } from '../sale-order-create-dot-kham-dialog/sale-order-create-dot-kham-dialog.component';
import { DotKhamBasic } from 'src/app/dot-khams/dot-khams';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { AccountInvoiceRegisterPaymentDialogV2Component } from 'src/app/account-invoices/account-invoice-register-payment-dialog-v2/account-invoice-register-payment-dialog-v2.component';
import { AccountRegisterPaymentDisplay, AccountRegisterPaymentDefaultGet } from 'src/app/account-payments/account-register-payment.service';
import { AccountPaymentBasic, AccountPaymentPaged } from 'src/app/account-payments/account-payment.service';
import { PaymentInfoContent } from 'src/app/account-invoices/account-invoice.service';
import { CardCardService, CardCardPaged } from 'src/app/card-cards/card-card.service';
import { ProductPriceListBasic, ProductPricelistPaged } from 'src/app/price-list/price-list';
import { PriceListService } from 'src/app/price-list/price-list.service';

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
  id: string;
  filteredPartners: PartnerSimple[];
  filteredUsers: UserSimple[];
  filteredPricelists: ProductPriceListBasic[];
  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;
  @ViewChild('userCbx', { static: true }) userCbx: ComboBoxComponent;
  @ViewChild('pricelistCbx', { static: true }) pricelistCbx: ComboBoxComponent;
  saleOrder: SaleOrderDisplay = new SaleOrderDisplay();
  saleOrderPrint: any;
  dotKhams: DotKhamBasic[] = [];

  payments: AccountPaymentBasic[] = [];
  paymentsInfo: PaymentInfoContent[] = [];

  searchCardBarcode: string;

  constructor(private fb: FormBuilder, private partnerService: PartnerService,
    private userService: UserService, private route: ActivatedRoute, private saleOrderService: SaleOrderService,
    private productService: ProductService, private intlService: IntlService, private modalService: NgbModal,
    private router: Router, private notificationService: NotificationService, private cardCardService: CardCardService,
    private pricelistService: PriceListService) {
  }

  ngOnInit() {
    this.formGroup = this.fb.group({
      partner: [null, Validators.required],
      user: null,
      dateOrderObj: [null, Validators.required],
      orderLines: this.fb.array([]),
      companyId: null,
      userId: null,
      amountTotal: 0,
      state: null,
      residual: null,
      card: null,
      pricelist: [null, Validators.required]
    });
    this.routeActive();
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

    this.pricelistCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.pricelistCbx.loading = true)),
      switchMap(value => this.searchPricelists(value))
    ).subscribe(result => {
      this.filteredPricelists = result.items;
      this.pricelistCbx.loading = false;
    });

    // this.getAccountPayments();
    // this.getAccountPaymentReconcicles();
    this.loadPartners();
    this.loadUsers();
    this.loadDotKhamList();
    this.loadPricelists();
  }

  routeActive() {
    this.route.paramMap.pipe(
      switchMap((params: ParamMap) => {
        this.id = params.get("id");
        // this.getAccountPayments(this.id);
        if (this.id) {
          return this.saleOrderService.get(this.id);
        } else {
          return this.saleOrderService.defaultGet();
        }
      })).subscribe(result => {

        this.saleOrder = result;
        this.formGroup.patchValue(result);
        let dateOrder = new Date(result.dateOrder);
        this.formGroup.get('dateOrderObj').patchValue(dateOrder);

        if (result.user) {
          this.filteredUsers = _.unionBy(this.filteredUsers, [result.user], 'id');
        }

        const control = this.formGroup.get('orderLines') as FormArray;
        control.clear();
        result.orderLines.forEach(line => {
          var g = this.fb.group(line);
          g.setControl('teeth', this.fb.array(line.teeth));
          control.push(g);
        });
      });
  }

  loadDotKhamList() {
    if (this.id) {
      return this.saleOrderService.getDotKhamList(this.id).subscribe(result => {
        this.dotKhams = result;
      });
    }
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

  removeCard() {
    this.formGroup.get('card').patchValue(null);
  }

  loadUsers() {
    this.searchUsers().subscribe(result => {
      this.filteredUsers = _.unionBy(this.filteredUsers, result, 'id');
    });
  }

  searchPartners(filter?: string) {
    var val = new PartnerPaged();
    val.customer = true;
    val.searchNamePhoneRef = filter;
    return this.partnerService.getAutocompleteSimple(val);
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

  createNew() {
    this.router.navigate(['/sale-orders/create']);
  }

  actionConfirm() {
    if (this.id) {
      this.saleOrderService.actionConfirm([this.id]).subscribe(() => {
        this.loadRecord();
      });
    }
  }

  actionCreateDotKham() {
    if (this.id) {
      let modalRef = this.modalService.open(SaleOrderCreateDotKhamDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = 'Tạo đợt khám';
      modalRef.componentInstance.saleOrderId = this.id;

      modalRef.result.then(result => {
        if (result.view) {
          this.router.navigate(['/dot-khams/edit/', result.result.id]);
        } else {
          this.loadDotKhamList();
          // $('#myTab a[href="#profile"]').tab('show');
        }
      }, () => {
      });
    }
  }

  printSaleOrder() {
    if (this.id) {
      this.saleOrderService.getPrint(this.id).subscribe((result: any) => {
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
    if (this.id) {
      let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = 'Khóa phiếu điều trị';
      modalRef.componentInstance.body = 'Khi khóa phiếu điều trị bạn sẽ không thể thay đổi được nữa, bạn có chắc chắn muốn khóa?';
      modalRef.result.then(() => {
        this.saleOrderService.actionDone([this.id]).subscribe(() => {
          this.loadRecord();
        });
      });
    }
  }

  onSaveConfirm() {
    if (!this.formGroup.valid) {
      return false;
    }

    var val = this.formGroup.value;
    val.dateOrder = this.intlService.formatDate(val.dateOrderObj, 'g', 'en-US');
    val.partnerId = val.partner.id;
    val.pricelistId = val.pricelist.id;
    val.userId = val.user ? val.user.id : null;
    val.cardId = val.card ? val.card.id : null;
    this.saleOrderService.create(val).subscribe(result => {
      this.saleOrderService.actionConfirm([result.id]).subscribe(() => {
        this.router.navigate(['/sale-orders/edit/' + result.id]);
      }, () => {
        this.router.navigate(['/sale-orders/edit/' + result.id]);
      });
    });
  }

  onSave() {
    if (!this.formGroup.valid) {
      return false;
    }

    var val = this.formGroup.value;
    val.dateOrder = this.intlService.formatDate(val.dateOrderObj, 'g', 'en-US');
    val.partnerId = val.partner.id;
    val.pricelistId = val.pricelist.id;
    val.userId = val.user ? val.user.id : null;
    val.cardId = val.card ? val.card.id : null;
    if (this.id) {
      this.saleOrderService.update(this.id, val).subscribe(() => {
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
      this.saleOrderService.create(val).subscribe(result => {
        this.router.navigate(['/sale-orders/edit/' + result.id]);
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
    if (this.id) {
      this.saleOrderService.get(this.id).subscribe(result => {
        this.saleOrder = result;
        this.formGroup.patchValue(result);
        let dateOrder = new Date(result.dateOrder);
        this.formGroup.get('dateOrderObj').patchValue(dateOrder);

        let control = this.formGroup.get('orderLines') as FormArray;
        control.clear();
        result.orderLines.forEach(line => {
          var g = this.fb.group(line);
          g.setControl('teeth', this.fb.array(line.teeth));
          control.push(g);
        });
      });
    }
  }

  actionCancel() {
    if (this.id) {
      this.saleOrderService.actionCancel([this.id]).subscribe(() => {
        this.loadRecord();
      });
    }
  }

  get orderLines() {
    return this.formGroup.get('orderLines') as FormArray;
  }

  showAddLineModal() {
    var partner = this.formGroup.get('partner').value;
    if (!partner) {
      this.notificationService.show({
        content: 'Vui lòng chọn khách hàng',
        hideAfter: 3000,
        position: { horizontal: 'right', vertical: 'bottom' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });
      return false;
    }

    let modalRef = this.modalService.open(SaleOrderLineDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm dịch vụ điều trị';
    modalRef.componentInstance.partnerId = partner.id;
    var pricelist = this.formGroup.get('pricelist').value;
    modalRef.componentInstance.pricelistId = pricelist ? pricelist.id : null;

    modalRef.result.then(result => {
      let line = result as any;
      line.teeth = this.fb.array(line.teeth);
      this.orderLines.push(this.fb.group(line));
      this.computeAmountTotal();
    }, () => {
    });
  }


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
      var a = result as any;
      line.patchValue(result);
      line.setControl('teeth', this.fb.array(a.teeth || []));
      this.computeAmountTotal();
    }, () => {
    });
  }

  lineTeeth(line: FormGroup) {
    var teeth = line.get('teeth').value as any[];
    return teeth.map(x => x.name).join(',');
  }

  deleteLine(index: number) {
    this.orderLines.removeAt(index);
    this.computeAmountTotal();
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
    var val = new AccountRegisterPaymentDefaultGet();
    val.invoiceIds = [this.id];
    this.saleOrderService.defaultOrderGet(val).subscribe(rs2 => {
      if (this.id) {
        if (rs2.amount > 0) {
          let modalRef = this.modalService.open(AccountInvoiceRegisterPaymentDialogV2Component, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
          modalRef.componentInstance.title = 'Thanh toán';
          rs2.communication = this.saleOrder.name;
          modalRef.componentInstance.defaultVal = rs2;
          modalRef.result.then(() => {
            this.notificationService.show({
              content: 'Thanh toán thành công',
              hideAfter: 3000,
              position: { horizontal: 'right', vertical: 'bottom' },
              animation: { type: 'fade', duration: 400 },
              type: { style: 'success', icon: true }
            });
            this.routeActive();
            this.getAccountPayments();
          }, () => {

          });
        } else {
          this.notificationService.show({
            content: 'Không còn hóa đơn nào để thanh toán',
            hideAfter: 3000,
            position: { horizontal: 'right', vertical: 'bottom' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'error', icon: true }
          });
        }
      }
    })
  }

  getAccountPayments() {
    if (this.id) {
      var val = new AccountPaymentFilter;
      val.saleOrderId = this.id;
      this.saleOrderService.getPaymentBasicList(val).subscribe(
        rs => {
          this.payments = rs;
        }
      )
    }
  }

  getAccountPaymentReconcicles() {
    if (this.id) {
      this.saleOrderService.getAccountPaymentReconcicles(this.id).subscribe(
        rs => {
          this.paymentsInfo = rs;
        }
      )
    }
  }

}
