import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { PartnerSimple, PartnerPaged } from 'src/app/partners/partner-simple';
import { CardTypeBasic } from 'src/app/card-types/card-type.service';
import { UserSimple } from 'src/app/users/user-simple';
import { PartnerService } from 'src/app/partners/partner.service';
import * as _ from 'lodash';
import { ServiceCardTypePaged } from 'src/app/service-card-types/service-card-type-paged';
import { ServiceCardTypeService } from 'src/app/service-card-types/service-card-type.service';
import { UserPaged, UserService } from 'src/app/users/user.service';
import { ServiceCardOrderService } from '../service-card-order.service';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PartnerCustomerCuDialogComponent } from 'src/app/partners/partner-customer-cu-dialog/partner-customer-cu-dialog.component';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { AccountPaymentService } from 'src/app/account-payments/account-payment.service';
import { AccountInvoiceRegisterPaymentDialogV2Component } from 'src/app/account-invoices/account-invoice-register-payment-dialog-v2/account-invoice-register-payment-dialog-v2.component';

@Component({
  selector: 'app-service-card-order-create-update',
  templateUrl: './service-card-order-create-update.component.html',
  styleUrls: ['./service-card-order-create-update.component.css']
})
export class ServiceCardOrderCreateUpdateComponent implements OnInit {

  cardOrder: any;
  formGroup: FormGroup;
  filteredPartners: PartnerSimple[];
  filteredPartners2: PartnerSimple[];
  filteredCardTypes: CardTypeBasic[];
  filteredUsers: UserSimple[];
  id: string;
  title = 'Đơn bán thẻ dịch vụ';

  gridPartners: PartnerSimple[] = [];

  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;
  @ViewChild('userCbx', { static: true }) userCbx: ComboBoxComponent;
  @ViewChild('cardTypeCbx', { static: true }) cardTypeCbx: ComboBoxComponent;
  @ViewChild('partner2Cbx', { static: true }) partner2Cbx: ComboBoxComponent;

  constructor(private fb: FormBuilder, private partnerService: PartnerService,
    private cardTypeService: ServiceCardTypeService, private userService: UserService,
    private cardOrderService: ServiceCardOrderService, private route: ActivatedRoute,
    private intlService: IntlService, private router: Router,
    private notificationService: NotificationService, private modalService: NgbModal,
    private paymentService: AccountPaymentService) { }

  ngOnInit() {
    this.cardOrder = {
      state: 'draft'
    };

    this.formGroup = this.fb.group({
      partner: [null, Validators.required],
      cardType: [null, Validators.required],
      dateOrderObj: [null, Validators.required],
      activatedDateObj: null,
      user: null,
      priceUnit: 0,
      quantity: 1,
      generationType: 'nbr_card'
    });

    this.route.queryParamMap.subscribe((param: ParamMap) => {
      this.id = param.get('id');
      if (this.id) {
        this.loadRecord();
      } else {
        this.cardOrderService.defaultGet().subscribe((result: any) => {
          this.formGroup.patchValue(result);

          let dateOrder = new Date(result.dateOrder);
          this.formGroup.get('dateOrderObj').patchValue(dateOrder);
        });
      }
    });

    this.loadFilteredPartners();
    this.loadFilteredCardTypes();
    this.loadFilteredUsers();
    this.loadFilteredPartners2();
    this.loadGridPartners();

    this.partnerCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.partnerCbx.loading = true)),
      switchMap(value => this.searchPartners(value))
    ).subscribe(result => {
      this.filteredPartners = result;
      this.partnerCbx.loading = false;
    });

    this.partner2Cbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.partner2Cbx.loading = true)),
      switchMap(value => this.searchPartners(value))
    ).subscribe(result => {
      this.filteredPartners2 = result;
      this.partner2Cbx.loading = false;
    });

    this.userCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.userCbx.loading = true)),
      switchMap(value => this.searchUsers(value))
    ).subscribe(result => {
      this.filteredUsers = result;
      this.userCbx.loading = false;
    });

    this.cardTypeCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.cardTypeCbx.loading = true)),
      switchMap(value => this.searchCardTypes(value))
    ).subscribe((result: any) => {
      this.filteredCardTypes = result.items;
      this.cardTypeCbx.loading = false;
    });
  }

  loadRecord() {
    this.cardOrderService.get(this.id).subscribe((result: any) => {
      this.cardOrder = result;
      this.formGroup.patchValue(result);

      let dateOrder = new Date(result.dateOrder);
      this.formGroup.get('dateOrderObj').patchValue(dateOrder);

      if (result.activatedDate) {
        let activatedDate = new Date(result.activatedDate);
        this.formGroup.get('activatedDateObj').patchValue(activatedDate);
      }

      this.filteredPartners = _.unionBy(this.filteredPartners, result.partner, 'id');
      this.filteredCardTypes = _.unionBy(this.filteredCardTypes, result.cardType, 'id');
      if (result.user) {
        this.filteredUsers = _.unionBy(this.filteredUsers, result.user, 'id');
      }
    });
  }

  get generationTypeValue() {
    return this.formGroup.get('generationType').value;
  }

  loadFilteredPartners() {
    this.searchPartners().subscribe(result => {
      this.filteredPartners = _.unionBy(this.filteredPartners, result, 'id');
    });
  }

  loadGridPartners() {
    if (this.id) {
      this.cardOrderService.getPartners(this.id).subscribe((result: any) => {
        this.gridPartners = result;
      });
    }
  }

  loadFilteredPartners2() {
    this.searchPartners().subscribe(result => {
      this.filteredPartners2 = _.unionBy(this.filteredPartners2, result, 'id');
    });
  }

  searchPartners(filter?: string) {
    var val = new PartnerPaged();
    val.customer = true;
    val.search = filter;
    return this.partnerService.getAutocompleteSimple(val);
  }

  loadFilteredCardTypes() {
    this.searchCardTypes().subscribe((result: any) => {
      this.filteredCardTypes = _.unionBy(this.filteredCardTypes, result.items, 'id');
    });
  }

  searchCardTypes(filter?: string) {
    var val = new ServiceCardTypePaged();
    val.search = filter || '';
    return this.cardTypeService.getPaged(val);
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

  onChangeCardType(value) {
    if (value) {
      this.formGroup.get('priceUnit').setValue(value.price);
    }
  }

  saveOrUpdate() {
    var value = this.formGroup.value;
    value.partnerId = value.partner.id;
    value.cardTypeId = value.cardType.id;
    value.userId = value.user ? value.user.id : null;
    value.dateOrder = this.intlService.formatDate(value.dateOrderObj, 'yyyy-MM-ddTHH:mm:ss');
    value.activatedDate = value.activatedDateObj ? this.intlService.formatDate(value.activatedDateObj, 'yyyy-MM-ddTHH:mm:ss') : null;

    if (!this.id) {
      return this.cardOrderService.create(value);
    } else {
      return this.cardOrderService.update(this.id, value);
    }
  }

  onSave() {
    if (!this.formGroup.valid) {
      return false;
    }

    var value = this.formGroup.value;
    value.partnerId = value.partner.id;
    value.cardTypeId = value.cardType.id;
    value.userId = value.user ? value.user.id : null;
    value.dateOrder = this.intlService.formatDate(value.dateOrderObj, 'yyyy-MM-ddTHH:mm:ss');
    value.activatedDate = value.activatedDateObj ? this.intlService.formatDate(value.activatedDateObj, 'yyyy-MM-ddTHH:mm:ss') : null;

    if (!this.id) {
      this.cardOrderService.create(value).subscribe((result: any) => {
        this.router.navigate(['/service-card-orders/form'], { queryParams: { id: result.id } });
      });
    } else {
      this.cardOrderService.update(this.id, value).subscribe(() => {
        this.notificationService.show({
          content: 'Lưu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });

        this.loadRecord();
      });
    }
  }

  addCustomer() {
    if (!this.id) {
      this.notificationService.show({
        content: 'Bạn cần lưu lại trước khi thêm khách hàng',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });

      return false;
    }

    var value = this.partner2Cbx.value;
    if (!value) {
      this.notificationService.show({
        content: 'Vui lòng chọn 1 khách hàng',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });

      return false;
    }

    this.cardOrderService.addPartners(this.id, [value.id]).subscribe(() => {
      this.loadGridPartners();
      this.loadRecord();
    });
  }

  removeCustomer(partner) {
    this.cardOrderService.removePartners(this.id, [partner.id]).subscribe(() => {
      this.loadGridPartners();
      this.loadRecord();
    });
  }

  actionConfirm() {
    if (this.id) {
      this.cardOrderService.actionConfirm([this.id]).subscribe(() => {
        this.loadRecord();
      });
    }
  }

  actionPayment() {
    if (this.id) {
      this.paymentService.serviceCardOrderDefaultGet([this.id]).subscribe(rs2 => {
        let modalRef = this.modalService.open(AccountInvoiceRegisterPaymentDialogV2Component, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
        modalRef.componentInstance.title = 'Thanh toán';
        modalRef.componentInstance.defaultVal = rs2;
        modalRef.result.then(() => {
          this.notificationService.show({
            content: 'Thanh toán thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });

          this.loadRecord();
        }, () => {
        });
      })
    }
  }

  actionCancel() {
    if (this.id) {
      this.cardOrderService.actionCancel([this.id]).subscribe(() => {
        this.loadRecord();
      });
    }
  }

  onSaveConfirm() {
    if (!this.id) {
      if (!this.formGroup.valid) {
        return false;
      }

      this.saveOrUpdate().subscribe((result: any) => {
        this.cardOrderService.actionConfirm([result.id]).subscribe(() => {
          this.router.navigate(['/service-card-orders/form'], { queryParams: { id: result.id } });
        });
      });
    }
  }

  createNew() {
    this.router.navigate(['/service-card-orders/form']);
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
