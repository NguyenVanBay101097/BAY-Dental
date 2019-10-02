import { Component, OnInit, ViewChild } from '@angular/core';
import { AccountRegisterPaymentService, AccountRegisterPaymentDefaultGet, AccountRegisterPaymentCreatePayment, AccountRegisterPaymentDisplay } from 'src/app/account-payments/account-register-payment.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { IntlService } from '@progress/kendo-angular-intl';
import { WindowRef } from '@progress/kendo-angular-dialog';
import { NotificationService } from '@progress/kendo-angular-notification';
import { AccountJournalService, AccountJournalSimple, AccountJournalFilter } from 'src/app/account-journals/account-journal.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-account-invoice-register-payment-dialog',
  templateUrl: './account-invoice-register-payment-dialog.component.html',
  styleUrls: ['./account-invoice-register-payment-dialog.component.css']
})
export class AccountInvoiceRegisterPaymentDialogComponent implements OnInit {
  invoiceId: string;
  paymentForm: FormGroup;
  filteredJournals: AccountJournalSimple[];
  @ViewChild('journalCbx', { static: true }) journalCbx: ComboBoxComponent;
  loading = false;

  constructor(private registerPaymentService: AccountRegisterPaymentService, private fb: FormBuilder, private intlService: IntlService,
    public windowRef: WindowRef, private notificationService: NotificationService, private accountJournalService: AccountJournalService) { }

  ngOnInit() {
    this.paymentForm = this.fb.group({
      amount: 0,
      paymentDateObj: null,
      paymentDate: null,
      communication: null,
      paymentType: null,
      journalId: null,
      journal: [null, Validators.required],
      partnerType: null,
      partnerId: null,
      invoiceIds: null
    });

    this.defaultGet();
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

  defaultGet() {
    var val = new AccountRegisterPaymentDefaultGet();
    val.invoiceIds = [this.invoiceId];
    this.registerPaymentService.defaultGet(val).subscribe(result => {
      this.paymentForm.patchValue(result);
      var paymentDate = this.intlService.parseDate(result.paymentDate);
      this.paymentForm.get('paymentDateObj').patchValue(paymentDate);
    }, err => {
      console.log(err);
    })
  }

  loadFilteredJournals(search?: string) {
    var val = new AccountJournalFilter();
    val.type = 'bank,cash';
    val.search = search;
    this.accountJournalService.autocomplete(val).subscribe(result => {
      this.filteredJournals = result;
      this.paymentForm.get('journal').setValue(result.filter(x => x.name == 'Tiền mặt')[0]);
    });

  }

  save() {
    if (!this.paymentForm.valid) {
      return;
    }

    this.loading = true;
    this.create().subscribe(result => {
      this.createPayment(result.id).subscribe(() => {
        this.windowRef.close(true);
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
    val.paymentDate = this.intlService.formatDate(val.paymentDateObj, 'g', 'en-US');
    return this.registerPaymentService.create(val);
  }

  createPayment(id) {
    var val = new AccountRegisterPaymentCreatePayment();
    val.id = id;
    return this.registerPaymentService.createPayment(val);
  }

  cancel() {
    this.windowRef.close();
  }

}
