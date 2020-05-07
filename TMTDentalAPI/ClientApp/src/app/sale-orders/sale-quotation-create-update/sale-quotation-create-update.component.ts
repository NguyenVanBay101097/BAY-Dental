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
import { SaleOrderApplyCouponDialogComponent } from '../sale-order-apply-coupon-dialog/sale-order-apply-coupon-dialog.component';
import { PartnerCustomerCuDialogComponent } from 'src/app/partners/partner-customer-cu-dialog/partner-customer-cu-dialog.component';
import { PartnerSearchDialogComponent } from 'src/app/partners/partner-search-dialog/partner-search-dialog.component';
import { EmployeeBasic, EmployeePaged, EmployeeSimple } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';

@Component({
  selector: 'app-sale-quotation-create-update',
  templateUrl: './sale-quotation-create-update.component.html',
  styleUrls: ['./sale-quotation-create-update.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class SaleQuotationCreateUpdateComponent implements OnInit {
  formGroup: FormGroup;
  id: string;
  filteredPartners: PartnerSimple[];
  filteredUsers: UserSimple[];
  filteredEmployees: EmployeeSimple[] = [];
  filteredPricelists: ProductPriceListBasic[];
  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;
  @ViewChild('userCbx', { static: true }) userCbx: ComboBoxComponent;
  @ViewChild('pricelistCbx', { static: true }) pricelistCbx: ComboBoxComponent;
  @ViewChild('employeeCbx', { static: true }) employeeCbx: ComboBoxComponent;
  saleOrder: SaleOrderDisplay = new SaleOrderDisplay();

  constructor(
    private fb: FormBuilder,
    private partnerService: PartnerService,
    private userService: UserService,
    private route: ActivatedRoute,
    private saleOrderService: SaleOrderService,
    private productService: ProductService,
    private intlService: IntlService,
    private modalService: NgbModal,
    private router: Router,
    private notificationService: NotificationService,
    private cardCardService: CardCardService,
    private pricelistService: PriceListService,
    private employeeService:EmployeeService
    ) {
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
      pricelist: [null, Validators.required],
      isQuotation: true,
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

    this.employeeCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.employeeCbx.loading = true)),
      switchMap(value => this.searchEmployees(value))
    ).subscribe(result => {
      this.filteredEmployees = result;
      this.employeeCbx.loading = false;
    });

    // this.pricelistCbx.filterChange.asObservable().pipe(
    //   debounceTime(300),
    //   tap(() => (this.pricelistCbx.loading = true)),
    //   switchMap(value => this.searchPricelists(value))
    // ).subscribe(result => {
    //   this.filteredPricelists = result.items;
    //   this.pricelistCbx.loading = false;
    // });

    this.loadPartners();
    this.loadUsers();
    // this.loadPricelists();
  }

  routeActive() {
    this.route.queryParamMap.pipe(
      switchMap((params: ParamMap) => {
        this.id = params.get("id");
        if (this.id) {
          return this.saleOrderService.get(this.id);
        } else {
          return this.saleOrderService.defaultGet({ isQuotation: true });
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

  get partner() {
    return this.formGroup.get('partner').value;
  }

  updateCustomerModal() {
    let modalRef = this.modalService.open(PartnerCustomerCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa khách hàng';
    modalRef.componentInstance.id = this.partner.id;

    modalRef.result.then(() => {
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

  createRecord() {
    var val = this.formGroup.value;
    val.dateOrder = this.intlService.formatDate(val.dateOrderObj, 'yyyy-MM-ddTHH:mm:ss');
    val.partnerId = val.partner.id;
    val.pricelistId = val.pricelist.id;
    val.userId = val.user ? val.user.id : null;
    val.cardId = val.card ? val.card.id : null;
    return this.saleOrderService.create(val);
  }

  saveRecord() {
    var val = this.formGroup.value;
    val.dateOrder = this.intlService.formatDate(val.dateOrderObj, 'yyyy-MM-ddTHH:mm:ss');
    val.partnerId = val.partner.id;
    val.pricelistId = val.pricelist.id;
    val.userId = val.user ? val.user.id : null;
    val.cardId = val.card ? val.card.id : null;
    return this.saleOrderService.update(this.id, val);
  }

  loadUsers() {
    this.searchUsers().subscribe(result => {
      this.filteredUsers = _.unionBy(this.filteredUsers, result, 'id');
    });
  }

  searchPartners(filter?: string) {
    var val = new PartnerPaged();
    val.customer = true;
    val.search = filter;
    return this.partnerService.getAutocompleteSimple(val);
  }

  searchUsers(filter?: string) {
    var val = new UserPaged();
    val.search = filter;
    return this.userService.autocompleteSimple(val);
  }

  searchEmployees(filter?: string) {
    var val = new EmployeePaged();
    val.search = filter;
    return this.employeeService.getEmployeeSimpleList(val);
  }

  searchPricelists(filter?: string) {
    var val = new ProductPricelistPaged();
    val.search = filter || '';
    return this.pricelistService.loadPriceListList(val);
  }

  createNew() {
    this.router.navigate(['/sale-quotations/form']);
  }

  actionConvertToOrder() {
    if (this.id) {
      this.saleOrderService.actionConvertToOrder(this.id).subscribe(() => {
        this.loadRecord();
      });
    }
  }

  actionDone() {
    if (this.id) {
      let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = 'Khóa phiếu tư vấn';
      modalRef.componentInstance.body = 'Khi khóa phiếu tư vấn bạn sẽ không thể thay đổi được nữa, bạn có chắc chắn muốn khóa?';
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
    val.dateOrder = this.intlService.formatDate(val.dateOrderObj, 'yyyy-MM-ddTHH:mm:ss');
    val.partnerId = val.partner.id;
    val.pricelistId = val.pricelist.id;
    val.userId = val.user ? val.user.id : null;
    val.orderLines.forEach(line => {
      line.toothIds = line.teeth.map(x => x.id);
    });
    this.saleOrderService.create(val).subscribe(result => {
      this.saleOrderService.actionConfirm([result.id]).subscribe(() => {
        this.router.navigate(['/sale-quotations/form'], { queryParams: { id: result.id } });
      }, () => {
        this.router.navigate(['/sale-quotations/form'], { queryParams: { id: result.id } });
      });
    });
  }

  onSave() {
    if (!this.formGroup.valid) {
      return false;
    }

    var val = this.formGroup.value;
    val.dateOrder = this.intlService.formatDate(val.dateOrderObj, 'yyyy-MM-ddTHH:mm:ss');
    val.partnerId = val.partner.id;
    val.pricelistId = val.pricelist.id;
    val.userId = val.user ? val.user.id : null;
    val.orderLines.forEach(line => {
      line.toothIds = line.teeth.map(x => x.id);
    });
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
        this.router.navigate(['/sale-quotations/form'], { queryParams: { id: result.id } });
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

  get getPartner() {
    return this.formGroup.get('partner').value;
  }

  computeAmountTotal() {
    let total = 0;
    this.orderLines.controls.forEach(line => {
      total += line.get('priceSubTotal').value;
    });
    this.formGroup.get('amountTotal').patchValue(total);
  }
}

