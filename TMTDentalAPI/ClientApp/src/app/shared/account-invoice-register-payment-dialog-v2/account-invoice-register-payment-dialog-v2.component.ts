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
  maxAmount: number = 0;

  showPrint = false;
  showError = false;

  constructor(private paymentService: AccountPaymentService, private fb: FormBuilder, private intlService: IntlService,
    public activeModal: NgbActiveModal, private notificationService: NotificationService, private accountJournalService: AccountJournalService,
    private errorService: AppSharedShowErrorService, private authService: AuthService) { }

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
        this.maxAmount = this.getValueForm('amount');
        this.paymentForm.get('amount').setValue(0);
        
        const control = this.saleOrderLinePaymentRels;
        control.clear();
        this.defaultVal['saleOrderLinePaymentRels'].forEach(line => {
          var g = this.fb.group(line);
          control.push(g);
        });
        this.paymentForm.markAsPristine();
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

  getValueForm(key) {
    return this.paymentForm.get(key).value;
  }

  searchJournals(search?: string) {
    var val = new AccountJournalFilter();
    val.type = 'bank,cash';
    val.search = search || '';
    val.companyId = this.authService.userInfo.companyId;
    return this.accountJournalService.autocomplete(val);
  }

  save() {
    if (!this.paymentForm.valid) {
      return;
    }

    var val = this.getValueFormSave();
    if (val == null)
      return;

    this.paymentService.create(val).subscribe((result: any) => {
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

    var val = this.getValueFormSave();
    if (val == null)
      return;

    this.paymentService.create(val).subscribe((result: any) => {
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

  getValueFormSave() {   
    var val = this.paymentForm.value;
    val.journalId = val.journal.id;
    val.paymentDate = this.intlService.formatDate(val.paymentDateObj, 'd', 'en-US');
    var sumAmountPrepaid = 0;
    if (val.amount != sumAmountPrepaid) {
      this.showError = true;
      return null;
    } else {
      this.showError = false;
    }
    return val;
  }

  cancel() {
    this.activeModal.dismiss();
  }

  get saleOrderLinePaymentRels() {
    return this.paymentForm.get('saleOrderLinePaymentRels') as FormArray;
  }

  getMaxMoneyLine(line: FormGroup) {
    return line.get('saleOrderLine').value['priceSubTotal'] - line.get('amountPayment').value;
  }

  changeMoneyLine(line: FormGroup) {
    var sumAmountPrepaid = 0;
    this.getValueForm('saleOrderLinePaymentRels').forEach(function(v){ 
      sumAmountPrepaid += v.amountPrepaid;
    });
    this.paymentForm.get('amount').setValue(sumAmountPrepaid);
  }

  payOff() {
    this.paymentForm.get('amount').setValue(this.maxAmount);

    var lines = this.getValueForm('saleOrderLinePaymentRels');
    const control = this.saleOrderLinePaymentRels;
    control.clear();

    lines.forEach(line => {
      line.amountPrepaid = line.saleOrderLine.priceSubTotal - line.amountPayment;
      var g = this.fb.group(line);
      control.push(g);
    });
    this.paymentForm.markAsPristine();
    this.showError = false;
  }

  enterMoney() {
    var amount = this.getValueForm('amount');

    var lines = this.getValueForm('saleOrderLinePaymentRels');
    const control = this.saleOrderLinePaymentRels;
    control.clear();

    var amountPrepaid = 0;
    lines.forEach(line => {
      amountPrepaid = line.saleOrderLine.priceSubTotal - line.amountPayment;
      if (amount >= amountPrepaid) {
        amount -= amountPrepaid;
        line.amountPrepaid = line.saleOrderLine.priceSubTotal - line.amountPayment;
      } else {
        line.amountPrepaid = amount;
        amount = 0;
      }
      var g = this.fb.group(line);
      control.push(g);
    });
    this.paymentForm.markAsPristine();
    this.showError = false;
  }
}
