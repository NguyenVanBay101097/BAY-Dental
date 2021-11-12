import { AfterViewInit, Component, Input, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import * as _ from 'lodash';
import * as moment from 'moment';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { PartnerPaged } from 'src/app/partners/partner-simple';
import { PartnerService } from 'src/app/partners/partner.service';
import { ServiceCardTypeService } from 'src/app/service-card-types/service-card-type.service';
import { PartnerCustomerCuDialogComponent } from 'src/app/shared/partner-customer-cu-dialog/partner-customer-cu-dialog.component';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { ServiceCardCardService } from '../service-card-card.service';

@Component({
  selector: 'app-service-card-cards-preferential-cu-dialog',
  templateUrl: './service-card-cards-preferential-cu-dialog.component.html',
  styleUrls: ['./service-card-cards-preferential-cu-dialog.component.css']
})
export class ServiceCardCardsPreferentialCuDialogComponent implements OnInit, AfterViewInit {
  @Input() title: string;
  @Input() id: string;
  partnerId: string;
  partnerDisable = false;
  @ViewChild('customerCbx', { static: false }) customerCbx: ComboBoxComponent;
  @ViewChild('cardTypeCbx', { static: true }) cardTypeCbx: ComboBoxComponent;


  formGroup: FormGroup;
  customerSimpleFilter: any[] = [];
  cardTypeSimpleFilter: any[] = [];
  submitted: boolean = false;
  state: string;

  get f() {
    return this.formGroup.controls;
  }

  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private modalService: NgbModal,
    private notifyService: NotifyService,
    private partnerService: PartnerService,
    private serviceCardsService: ServiceCardCardService,
    private serviceCardTypeService: ServiceCardTypeService
  ) { }

  ngOnInit(): void {
    this.formGroup = this.fb.group({
      barcode: ['', [Validators.required, createLengthValidator()]],
      activatedDateObj: null,
      expiredDateObj: null,
      partner: null,
      cardType: [null, Validators.required],
      state: 'draft',
    });
    this.cardTypeCbx.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.cardTypeCbx.loading = true)),
        switchMap((value) => this.searchCardTypes(value)
        )
      )
      .subscribe((x: any) => {
        this.cardTypeSimpleFilter = x;
        this.cardTypeCbx.loading = false;
      });

    if (this.id) {
      this.loadDataFromApi();
    }

    this.loadCustomers();
    this.loadCardTypes();
  }

  ngAfterViewInit(): void {
    this.customerCbxFilterChange();
  }

  customerCbxFilterChange() {
    this.customerCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.customerCbx.loading = true)),
      switchMap((value) => this.searchCustomers(value))
    ).subscribe((result) => {
      console.log(result);
      this.customerSimpleFilter = result;
      this.customerCbx.loading = false;
    });
  }


  setValueFC(key: string, value: any) {
    this.formGroup.controls[key].setValue(value);
  }

  getValueFC(key: string) {
    return this.formGroup.controls[key].value;
  }

  loadDataFromApi() {
    this.serviceCardsService.get(this.id).subscribe((res: any) => {
      this.formGroup.patchValue(res);
      this.formGroup.get('activatedDateObj').patchValue(new Date(res.activatedDate));
      this.formGroup.get('expiredDateObj').patchValue(new Date(res.expiredDate));
      this.customerSimpleFilter = _.unionBy(this.customerSimpleFilter,[res.partner], 'id');
      
      this.state = res.state;
      if (this.id && this.state !== 'draft') {
        this.formGroup.disable();
      }
    })
  }

  searchCustomers(q?: string) {
    let val = new PartnerPaged();
    val.limit = 20;
    val.offset = 0;
    val.search = q || '';
    val.customer = true;
    val.employee = false;
    return this.partnerService.autocomplete3(val);
  }

  loadCustomers() {
    this.searchCustomers().subscribe((res: any) => {
      this.customerSimpleFilter = res;
    })
  }

  searchCardTypes(q?: string) {
    let search = q ? q : '';
    return this.serviceCardTypeService.autoComplete(search);
  }

  loadCardTypes() {
    this.searchCardTypes().subscribe((res: any) => {
      this.cardTypeSimpleFilter = res;
    })
  }

  onSave() {
    this.submitted = true;

    if (this.formGroup.invalid) {
      return;
    }

    let val = this.formGroup.value;
    val.partnerId = this.partnerId ? this.partnerId : (val.partner ? val.partner.id : '');
    val.cardTypeId = val.cardType ? val.cardType.id : '';

    if (this.id) {
      this.serviceCardsService.update(this.id, val).subscribe((res: any) => {
        this.activeModal.close(res);
      }, (error) => { console.log(error) });
    } else {
      this.serviceCardsService.create(val).subscribe((res: any) => {
        console.log(res);
        this.activeModal.close(res);
      }, (error) => { console.log(error) });
    }

  }

  actionActivate() {
    this.submitted = true;
    if (this.formGroup.invalid) {
      return;
    }
    let val = this.formGroup.value;
    val.partnerId = this.partnerId ? this.partnerId : (val.partner ? val.partner.id : '');
    val.cardTypeId = val.cardType ? val.cardType.id : '';

    if (this.id) {
      this.serviceCardsService.update(this.id, val).subscribe((res: any) => {
        this.serviceCardsService.buttonActive([this.id]).subscribe((res: any) => {
          this.notifyService.notify('success', 'Kích hoạt thành công');
          this.activeModal.close('activate');
        })
      }, (error) => { console.log(error) });

    }
    else {
      this.serviceCardsService.create(val).subscribe((res: any) => {
        this.serviceCardsService.buttonActive([res.id]).subscribe((res: any) => {
          this.notifyService.notify('success', 'Kích hoạt thành công');
          this.activeModal.close(true);
        })
      }, (error) => { console.log(error) });
    }

  }

  quickCreateCustomerModal() {
    let modalRef = this.modalService.open(PartnerCustomerCuDialogComponent, { size: 'xl', scrollable: true, windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm khách hàng';
    modalRef.result.then((res) => {
      this.setValueFC('partner', res);
    });
  }


  getState(state) {
    switch (state) {
      case "in_use":
        return "Đã kích hoạt";
      case "cancelled":
        return "Hủy thẻ";
      case "locked":
        return "Tạm dừng";
      default:
        return "Chưa kích hoạt";
    }
  }
}


export function createLengthValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const valueLength = control.value.toString().length;
    var lengthValid = true;
    if (valueLength < 10 || valueLength > 15)
      lengthValid = false;
    return !lengthValid ? { lengthError: true } : null;
  }
}
