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

  @ViewChild('partner2Cbx', { static: true }) partner2Cbx: ComboBoxComponent;

  constructor(private fb: FormBuilder, private partnerService: PartnerService,
    private cardTypeService: ServiceCardTypeService, private userService: UserService,
    private cardOrderService: ServiceCardOrderService, private route: ActivatedRoute,
    private intlService: IntlService, private router: Router,
    private notificationService: NotificationService) { }

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
    val.searchNamePhoneRef = filter;
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
    });
  }

  removeCustomer(partner) {
    this.cardOrderService.removePartners(this.id, [partner.id]).subscribe(() => {
      this.loadGridPartners();
    });
  }
}
