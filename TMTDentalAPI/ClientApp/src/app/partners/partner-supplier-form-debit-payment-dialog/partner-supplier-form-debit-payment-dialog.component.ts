import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { AccountJournalFilter, AccountJournalService } from 'src/app/account-journals/account-journal.service';
import { AccountPaymentService } from 'src/app/account-payments/account-payment.service';
import { AccountRegisterPaymentDisplay } from 'src/app/account-payments/account-register-payment.service';
import { AuthService } from 'src/app/auth/auth.service';

@Component({
  selector: 'app-partner-supplier-form-debit-payment-dialog',
  templateUrl: './partner-supplier-form-debit-payment-dialog.component.html',
  styleUrls: ['./partner-supplier-form-debit-payment-dialog.component.css']
})
export class PartnerSupplierFormDebitPaymentDialogComponent implements OnInit {
  @ViewChild('journalCbx', { static: true }) journalCbx: ComboBoxComponent;
  title: string;
  rowsSelected: any[] = [];
  partnerId: string;
  partnerType: string;
  formGroup: FormGroup;
  defaultVal: AccountRegisterPaymentDisplay;
  loading = false;
  public aggregates: any[] = [
    { field: 'amountResidual', aggregate: 'sum' }
  ];
  filteredJournals: any;
  submitted = false;
  
  get f() { return this.formGroup.controls; }

  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private authService: AuthService,
    private accountJournalService: AccountJournalService,
    private paymentService: AccountPaymentService,
    private intlService: IntlService,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      amount: [0, Validators.max(this.defaultVal.amount)],
      paymentDateObj: [new Date(), Validators.required],
      paymentDate: null,
      communication: null,
      paymentType: null,
      journalId: null,
      journal: [null, Validators.required],
      partnerType: null,
      partnerId: null,
      invoiceIds: null,
    });
    this.loadFilteredJournals();
    this.journalCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.journalCbx.loading = true)),
      switchMap(value => this.searchJournals(value))
    ).subscribe(result => {
      this.filteredJournals = result;
      this.journalCbx.loading = false;
    });
    if (this.defaultVal) {
      this.loadFormApi();
    }

  }

  loadFormApi() {
    this.formGroup.patchValue(this.defaultVal);
    this.rowsSelected = this.rowsSelected.filter(x => this.defaultVal.invoiceIds.includes(x.id));
  }

  loadFilteredJournals() {
    this.searchJournals().subscribe(result => {
      this.filteredJournals = result;
      this.formGroup.get("journal").patchValue(this.filteredJournals[0])
    })
  }

  searchJournals(search?: string) {
    var val = new AccountJournalFilter();
    val.type = 'bank,cash';
    val.search = search || '';
    val.companyId = this.authService.userInfo.companyId;
    return this.accountJournalService.autocomplete(val);
  }

  onPayment() {
    this.submitted = true;
    
    if (!this.formGroup.valid) {
      return;
    }

    this.create().subscribe((result: any) => {
      this.paymentService.post([result.id]).subscribe(() => {
        this.notificationService.show({
          content: 'Thanh to??n th??nh c??ng',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.activeModal.close(true);
      }, (err) => {
      });
    }, (err) => {
    });
  }

  checkAmount() {
    if (!this.formGroup.get('amount').value) {
      this.formGroup.get('amount').setValue(0);
    }
  }

  showType(type: string) {
    switch (type) {
      case 'in_invoice':
        return 'Mua h??ng';
      case 'in_refund':
        return 'Tr??? h??ng';
      default:
        return '';
    }
  }

  getTitle() {
    switch (this.defaultVal.paymentType) {
      case 'inbound':
        return 'Nh?? cung c???p ho??n ti???n';
      case 'outbound':
        return 'Thanh to??n nh?? cung c???p';
      default:
        return '';
    }
  }

  create() {
    var val = this.formGroup.value;
    val.journalId = val.journal.id;
    val.paymentDate = this.intlService.formatDate(val.paymentDateObj, 'd', 'en-US');
    return this.paymentService.create(val);
  }

  onChangeMoney() {
    if (this.formGroup.get('amount') && !this.formGroup.get('amount').value)
      this.formGroup.get('amount').setValue(0);
  }

}
