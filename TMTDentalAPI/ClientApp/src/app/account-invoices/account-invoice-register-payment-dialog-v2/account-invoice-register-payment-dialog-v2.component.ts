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

  showPrint = false;

  constructor(private paymentService: AccountPaymentService, private fb: FormBuilder, private intlService: IntlService,
    public activeModal: NgbActiveModal, private notificationService: NotificationService, private accountJournalService: AccountJournalService,
    private errorService: AppSharedShowErrorService) { }

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
      invoiceIds: null,
      saleOrderIds: null,
      serviceCardOrderIds: null,
      saleOrderLinePaymentRels: this.fb.array([])
    });

    setTimeout(() => {
      if (this.defaultVal) {
        console.log(this.defaultVal);
        this.paymentForm.patchValue(this.defaultVal);
        var paymentDate = new Date(this.defaultVal.paymentDate);
        this.paymentForm.get('paymentDateObj').setValue(paymentDate);
        
        const control = this.paymentForm.get('saleOrderLinePaymentRels') as FormArray;
        control.clear();
        this.defaultVal['saleOrderLinePaymentRels'].forEach(line => {
          var g = this.fb.group(line);
          control.push(g);
        });
        this.paymentForm.markAsPristine();

        console.log(this.paymentForm.value);
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
    return this.accountJournalService.autocomplete(val);
  }

  save() {
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
    val.saleOrderLinePaymentRels.forEach(function(v){ 
      delete v.amountPayment; 
      delete v.saleOrderLine
    });
    console.log(val);
    return this.paymentService.create(val);
  }

  cancel() {
    this.activeModal.dismiss();
  }

  get saleOrderLinePaymentRels() {
    return this.paymentForm.get('saleOrderLinePaymentRels') as FormArray;
  }

  checkMoneyLine(value) {
    console.log(value);
  }
}
