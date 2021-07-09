import { Component, OnInit, ViewChild } from '@angular/core';
import { AccountRegisterPaymentService, AccountRegisterPaymentDefaultGet, AccountRegisterPaymentCreatePayment, AccountRegisterPaymentDisplay } from 'src/app/account-payments/account-register-payment.service';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { AccountJournalService, AccountJournalSimple, AccountJournalFilter } from 'src/app/account-journals/account-journal.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';
import { AccountPaymentService } from 'src/app/account-payments/account-payment.service';
import { AuthService } from 'src/app/auth/auth.service';
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
  purchaseType: string;
  showPrint = false;
  submitted = false;

  get f() { return this.paymentForm.controls; }

  constructor(private paymentService: AccountPaymentService, private fb: FormBuilder, private intlService: IntlService,
    public activeModal: NgbActiveModal, private notificationService: NotificationService, private accountJournalService: AccountJournalService,
    private errorService: AppSharedShowErrorService, private authService: AuthService) { }

  ngOnInit() {
    this.paymentForm = this.fb.group({
      amount: [0, Validators.required],
      paymentDateObj: [null, Validators.required],
      paymentDate: null,
      communication: null,
      paymentType: null,
      journalId: null,
      journal: [null, Validators.required],
      partnerType: null,
      partnerId: null,
      invoiceIds: null,
      saleOrderIds: null,
      serviceCardOrderIds: null,
    });

    setTimeout(() => {
      if (this.defaultVal) {
        this.paymentForm.patchValue(this.defaultVal);
        var paymentDate = new Date(this.defaultVal.paymentDate);
        this.paymentForm.get('paymentDateObj').setValue(paymentDate);
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

  save() {
    this.submitted = true;
    
    if (!this.paymentForm.valid) {
      return;
    }

    this.create().subscribe((result: any) => {
      this.paymentService.post([result.id]).subscribe(() => {
        this.activeModal.close(true);
      }, (err) => {
        this.errorService.show(err);
      });
    }, (err) => {
      this.errorService.show(err);
    });
  }

  saveAndPrint() {
    this.submitted = true;

    if (!this.paymentForm.valid) {
      return;
    }

    this.create().subscribe((result: any) => {
      this.paymentService.post([result.id]).subscribe(() => {
        this.activeModal.close({
          print: true,
          paymentId: result.id
        });
      }, (err) => {
        this.errorService.show(err);
      });
    }, (err) => {
      this.errorService.show(err);
    });
  }

  create() {
    var val = this.paymentForm.value;
    val.journalId = val.journal.id;
    val.paymentDate = this.intlService.formatDate(val.paymentDateObj, 'd', 'en-US');
    return this.paymentService.create(val);
  }

  cancel() {
    this.activeModal.dismiss();
  }
}
