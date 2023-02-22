import { Component, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import * as _ from 'lodash';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/auth.service';
import { CardCardService } from 'src/app/card-cards/card-card.service';
import { CardTypeService } from 'src/app/card-types/card-type.service';
import { SessionInfoStorageService } from 'src/app/core/services/session-info-storage.service';
import { PartnerPaged, PartnerSimpleContact } from 'src/app/partners/partner-simple';
import { PartnerService } from 'src/app/partners/partner.service';
import { PartnerCustomerCuDialogComponent } from 'src/app/shared/partner-customer-cu-dialog/partner-customer-cu-dialog.component';
import { NotifyService } from 'src/app/shared/services/notify.service';

@Component({
  selector: 'app-card-cards-member-cu-dialog',
  templateUrl: './card-cards-member-cu-dialog.component.html',
  styleUrls: ['./card-cards-member-cu-dialog.component.css']
})
export class CardCardsMemberCuDialogComponent implements OnInit {
  @ViewChild('customerCbx', { static: true }) customerCbx: ComboBoxComponent;
  @ViewChild('cardTypeCbx', { static: true }) cardTypeCbx: ComboBoxComponent;

  formGroup: FormGroup;
  title: string = '';
  id: string;
  submitted = false;
  customerSimpleFilter: PartnerSimpleContact[] = [];
  cardTypeSimpleFilter: any[] = [];
  state: string = 'draft';

  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private modalService: NgbModal,
    private notifyService: NotifyService,
    private partnerService: PartnerService,
    private cardCardsService: CardCardService,
    private cardService: CardTypeService,
    private sessionInfoStorageService: SessionInfoStorageService,
    private authService: AuthService,
  ) { }

  ngOnInit(): void {
    this.formGroup = this.fb.group({
      barcode: ['', [Validators.required, createLengthValidator()]],
      type: null,
      partner: null,
    });
    this.customerCbx.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.customerCbx.loading = true)),
        switchMap((value) => this.searchCustomers(value)
        )
      )
      .subscribe((x) => {
        this.customerSimpleFilter = x;
        this.customerCbx.loading = false;
      });
    this.cardTypeCbx.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.cardTypeCbx.loading = true)),
        switchMap((value) => this.searchCardTypes(value)
        )
      )
      .subscribe((x) => {
        this.cardTypeSimpleFilter = x.items;
        this.cardTypeCbx.loading = false;
      });
    this.loadCustomers();
    this.loadCardTypes();
    this.loadDataFromApi();
  }

  getState(state) {
    if (state == "in_use")
      return "Đã kích hoạt";
    else if (state == "draft")
      return "Chưa kích hoạt";
    return "";
  }

  quickCreateCustomerModal() {
    let modalRef = this.modalService.open(PartnerCustomerCuDialogComponent, { size: 'xl', scrollable: true, windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm khách hàng';
    modalRef.result.then((res) => {
      this.setValueFC('partner', res);
      this.customerSimpleFilter = _.unionBy(this.customerSimpleFilter, [res], 'id');
    });
  }

  setValueFC(key: string, value: any) {
    this.formGroup.controls[key].setValue(value);
  }

  onSave() {
    this.submitted = true;
    if (this.formGroup.invalid) {
      return;
    }

    let val = this.formGroup.value;
    // val.partnerId = this.partnerId ? this.partnerId : (val.partner ? val.partner.id : '');
    val.typeId = val.type ? val.type.id : '';
    val.partnerId = val.partner ? val.partner.id : '';
    if (this.id) {
      this.cardCardsService.update(this.id, val).subscribe((res: any) => {
        this.notifyService.notify('success', 'Lưu thành công');
        this.activeModal.close(res);
      }, (error) => { console.log(error) });
    } else {
      this.cardCardsService.create(val).subscribe((res: any) => {
        this.notifyService.notify('success', 'Lưu thành công');
        this.activeModal.close(res);
      }, (error) => { console.log(error) });
    }

  }

  actionActivate() {
    this.submitted = true;
    if (this.formGroup.invalid) {
      return;
    }

    if (!this.formGroup.get('partner').value) {
      this.notifyService.notify('error', 'Khách hàng đang trống, cần bổ sung khách hàng');
      return false;
    }

    let val = this.formGroup.value;
    val.typeId = val.type ? val.type.id : '';
    val.partnerId = val.partner ? val.partner.id : '';
  
    if (this.id) {
      this.cardCardsService.update(this.id, val).subscribe(res => {
        this.cardCardsService.buttonActive([this.id]).subscribe(() => {
          this.notifyService.notify('success', 'Kích hoạt thành công');
          this.activeModal.close();
        })
      })
    }
    else {
      this.cardCardsService.create(val).subscribe(res => {
        this.cardCardsService.buttonActive([res.id]).subscribe(() => {
          this.notifyService.notify('success', 'Kích hoạt thành công');
          this.activeModal.close();
        })
      })
    }
  }

  loadDataFromApi() {
    if (this.id) {
      this.cardCardsService.get(this.id).subscribe(res => {
        this.formGroup.patchValue(res);
        this.setValueFC('type', res.type);
        this.state = res.state;
        if (res.state == 'in_use')
          this.formGroup.disable();
      this.customerSimpleFilter = _.unionBy(this.customerSimpleFilter, [res.partner], 'id');

      })
    } else {
      this.cardCardsService.getDefault().subscribe(res => {
        this.formGroup.patchValue(res);
        this.setValueFC('type', res.type);
        this.state = res.state;
        if (res.state == 'in_use')
          this.formGroup.disable();
      })
    }
  }

  loadCustomers() {
    this.searchCustomers().subscribe((res: any) => {
      this.customerSimpleFilter = res;
    })
  }

  loadCardTypes() {
    this.searchCardTypes().subscribe(result => {
      this.cardTypeSimpleFilter = result.items;
    })
  }

  searchCustomers(q?: string) {
    let val = new PartnerPaged();
    val.limit = 20;
    val.offset = 0;
    val.search = q || '';
    val.customer = true;
    val.employee = false;
    if (this.sessionInfoStorageService.getSessionInfo().settings && !this.sessionInfoStorageService.getSessionInfo().settings.companySharePartner) {
      val.companyId = this.authService.userInfo.companyId;
    }
    return this.partnerService.autocomplete3(val);
  }

  searchCardTypes(q?: string) {
    var val = { search: q || '', offset: 0, limit: 0 };
    return this.cardService.getPaged(val);
  }

  get f() {
    return this.formGroup.controls;
  }
}

export function createLengthValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value;
    var lengthValid = true;
    if (!value || value.length < 5 || value.length > 15)
      lengthValid = false;
    return !lengthValid ? { lengthError: true } : null;
  }
}
