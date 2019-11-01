import { Component, OnInit, ViewChild } from '@angular/core';
import { AccountRegisterPaymentService, AccountRegisterPaymentDefaultGet, AccountRegisterPaymentCreatePayment, AccountRegisterPaymentDisplay } from 'src/app/account-payments/account-register-payment.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { AccountJournalService, AccountJournalSimple, AccountJournalFilter } from 'src/app/account-journals/account-journal.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
@Component({
  selector: 'app-account-invoice-register-payment-dialog-v2',
  templateUrl: './account-invoice-register-payment-dialog-v2.component.html',
  styleUrls: ['./account-invoice-register-payment-dialog-v2.component.css']
})
export class AccountInvoiceRegisterPaymentDialogV2Component implements OnInit {
  paymentForm: FormGroup;
  defaultVal: AccountRegisterPaymentDisplay;
  filteredJournals: AccountJournalSimple[];
  @ViewChild('journalCbx', { static: true }) journalCbx: ComboBoxComponent;
  loading = false;
  title: string;

  constructor(private registerPaymentService: AccountRegisterPaymentService, private fb: FormBuilder, private intlService: IntlService,
    public activeModal: NgbActiveModal, private notificationService: NotificationService, private accountJournalService: AccountJournalService) { }

  ngOnInit() {
    this.paymentForm = this.fb.group({
      amount: 0,
      paymentDateObj: [null, Validators.required],
      paymentDate: null,
      communication: null,
      paymentType: null,
      journalId: null,
      journal: [null, Validators.required],
      partnerType: null,
      partnerId: null,
      invoiceIds: null
    });

    if (this.defaultVal) {
      this.paymentForm.patchValue(this.defaultVal);
      var paymentDate = new Date(this.defaultVal.paymentDate);
      this.paymentForm.get('paymentDateObj').setValue(paymentDate);
    }

    this.loadFilteredJournals();

    this.journalCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.journalCbx.loading = true)),
      switchMap(value => this.accountJournalService.autocomplete(value))
    ).subscribe(result => {
      this.filteredJournals = result;
      this.journalCbx.loading = false;
    });
  }

  loadFilteredJournals(search?: string) {
    var val = new AccountJournalFilter();
    val.type = 'bank,cash';
    val.search = search;
    this.accountJournalService.autocomplete(val).subscribe(result => {
      this.filteredJournals = result;
    });

  }

  save() {
    if (!this.paymentForm.valid) {
      return;
    }

    this.loading = true;
    this.create().subscribe(result => {
      this.createPayment(result.id).subscribe(() => {
        this.activeModal.close(true);
        this.loading = false;
      }, () => {
        this.loading = false;
      });
    }, () => {
      this.loading = false;
    });
  }

  create() {
    var val = this.paymentForm.value;
    val.journalId = val.journal.id;
    val.paymentDate = this.intlService.formatDate(val.paymentDateObj, 'd', 'en-US');
    return this.registerPaymentService.create(val);
  }

  createPayment(id) {
    var val = new AccountRegisterPaymentCreatePayment();
    val.id = id;
    return this.registerPaymentService.createPayment(val);
  }

  cancel() {
    this.activeModal.dismiss();
  }

}
