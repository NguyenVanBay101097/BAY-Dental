import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { AccountJournalFilter, AccountJournalService, AccountJournalSimple } from 'src/app/account-journals/account-journal.service';
import { AccountPaymentService } from 'src/app/account-payments/account-payment.service';
import { AccountRegisterPaymentDisplay } from 'src/app/account-payments/account-register-payment.service';
import { AuthService } from 'src/app/auth/auth.service';
import { NotifyService } from 'src/app/shared/services/notify.service';

@Component({
  selector: 'app-res-insurance-debt-payment-dialog',
  templateUrl: './res-insurance-debt-payment-dialog.component.html',
  styleUrls: ['./res-insurance-debt-payment-dialog.component.css']
})
export class ResInsuranceDebtPaymentDialogComponent implements OnInit {
  @ViewChild('journalCbx', { static: true }) journalCbx: ComboBoxComponent;
  formGroup: FormGroup;
  defaultVal: AccountRegisterPaymentDisplay;
  filteredJournals: AccountJournalSimple[];
  loading = false;
  title: string;
  purchaseType: string;
  submitted = false;
  partnerId: string;
  debitItems: any[];

  constructor(private paymentService: AccountPaymentService,
    private fb: FormBuilder,
    private intlService: IntlService,
    public activeModal: NgbActiveModal,
    private accountJournalService: AccountJournalService,
    private authService: AuthService,
    private notifyService: NotifyService) { }

  ngOnInit(): void {
    this.formGroup = this.fb.group({
      amount:new FormControl({
        value: 0,
        disabled: true
      }),
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

    setTimeout(() => {
      if (this.defaultVal) {
        this.debitItems = this.defaultVal.debitItems;
        this.formGroup.patchValue(this.defaultVal);
        var paymentDate = new Date(this.defaultVal.paymentDate);
        this.formGroup.get('paymentDateObj').setValue(paymentDate);
      }

      this.loadFilteredJournals();

      this.journalCbx.filterChange.asObservable().pipe(
        debounceTime(300),
        tap(() => (this.journalCbx.loading = true)),
        switchMap(value => this.searchJournals(value))
      ).subscribe(result => {
        this.filteredJournals = result;
        this.journalCbx.loading = false;
      });
    });

  }

  loadFilteredJournals() {
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

  onSave() {
    this.submitted = true;
    var val = this.formGroup.getRawValue();
    val.partnerId = this.partnerId ? this.partnerId : val.partnerId;
    val.journalId = val.journal.id;
    val.paymentDate = this.intlService.formatDate(val.paymentDateObj, 'd', 'en-US');
    this.paymentService.create(val).subscribe((result: any) => {
      this.paymentService.post([result.id]).subscribe((res) => {
        this.activeModal.close(res);
      });
    }, (err) => {
      this.notifyService.notify('error', err);
    });
  }

  get f() { return this.formGroup.controls; }

  deleteItem(item, index) {
    this.debitItems.splice(index, 1);
    const invoiceIds = this.defaultVal.invoiceIds.filter(val => val !== item.moveId);
    const amountComputed = this.f.amount.value - item.balance;
    this.f.amount.setValue(amountComputed);
    this.f.invoiceIds.setValue(invoiceIds);
  }

  cancel() {
    this.activeModal.dismiss();
  }

}
