import { Component, OnInit, ViewChild } from '@angular/core';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { AccountJournalSimple, AccountJournalService, AccountJournalFilter } from 'src/app/account-journals/account-journal.service';
import { AccountRegisterPaymentDisplay } from 'src/app/account-payments/account-register-payment.service';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { AccountPaymentService } from 'src/app/account-payments/account-payment.service';
import { IntlService } from '@progress/kendo-angular-intl';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';
import { AuthService } from 'src/app/auth/auth.service';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { AccountPaymentsOdataService } from 'src/app/shared/services/account-payments-odata.service';

@Component({
  selector: 'app-sale-order-payment-dialog',
  templateUrl: './sale-order-payment-dialog.component.html',
  styleUrls: ['./sale-order-payment-dialog.component.css']
})
export class SaleOrderPaymentDialogComponent implements OnInit {
  paymentForm: FormGroup;
  defaultVal: any;
  filteredJournals: AccountJournalSimple[];
  @ViewChild('journalCbx', { static: true }) journalCbx: ComboBoxComponent;
  loading = false;
  title: string;
  maxAmount: number = 0;

  showPrint = false;
  showError_1 = false;
  showError_2 = false;

  constructor(private paymentService: AccountPaymentService, private fb: FormBuilder, private intlService: IntlService,
    public activeModal: NgbActiveModal, private notificationService: NotificationService, private accountJournalService: AccountJournalService,
    private errorService: AppSharedShowErrorService, private authService: AuthService,
    private accountPaymenetOdataService: AccountPaymentsOdataService) { }

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
        this.paymentForm.patchValue(this.defaultVal);
        var paymentDate = new Date(this.defaultVal.paymentDate);
        this.paymentForm.get('paymentDateObj').setValue(paymentDate);
        this.maxAmount = this.getValueForm('amount');
        this.paymentForm.get('amount').setValue(0);

        const control = this.saleOrderLinePaymentRels;
        control.clear();
        this.defaultVal.saleOrderLinePaymentRels.forEach(line => {
          line.amountPrepaid = 0;
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
      if (this.filteredJournals && this.filteredJournals.length > 1) {
        this.paymentForm.get('journal').patchValue(this.filteredJournals[0])
      }
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
    this.getValueForm('saleOrderLinePaymentRels').forEach(function (v) {
      sumAmountPrepaid += v.amountPrepaid;
    });
    this.paymentForm.get('amount').setValue(sumAmountPrepaid);
  }

  payOff() {
    var lines = this.paymentForm.get('saleOrderLinePaymentRels') as FormArray;

    var total = 0;
    lines.controls.forEach(control => {
      var residual = control.get('amountResidual').value || 0;
      control.get('amountPrepaid').setValue(residual);

      total += residual;
    });

    this.paymentForm.get('amount').setValue(total);
  }

  enterMoney() {
    var amount = this.paymentForm.get('amount').value || 0;

    var lines = this.paymentForm.get('saleOrderLinePaymentRels') as FormArray;

    lines.controls.forEach(control => {
      var amountResidual = control.get('amountResidual').value || 0;
      var amountPaid = Math.min(amount, amountResidual);
      control.get('amountPrepaid').setValue(amountPaid);

      amount -= amountPaid;
    });
  }

}
