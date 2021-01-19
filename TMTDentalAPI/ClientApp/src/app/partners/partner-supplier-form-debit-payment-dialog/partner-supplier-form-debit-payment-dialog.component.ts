import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { aggregateBy } from '@progress/kendo-data-query';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { AccountJournalFilter, AccountJournalService } from 'src/app/account-journals/account-journal.service';
import { AccountPaymentService } from 'src/app/account-payments/account-payment.service';
import { AuthService } from 'src/app/auth/auth.service';

@Component({
  selector: 'app-partner-supplier-form-debit-payment-dialog',
  templateUrl: './partner-supplier-form-debit-payment-dialog.component.html',
  styleUrls: ['./partner-supplier-form-debit-payment-dialog.component.css']
})
export class PartnerSupplierFormDebitPaymentDialogComponent implements OnInit {
  @ViewChild('journalCbx', { static: true }) journalCbx: ComboBoxComponent;

  rowsSelected: any[] = [];
  partnerId: string;
  partnerType: string;
  formGroup: FormGroup;
  loading = false;
  public aggregates: any[] = [
    { field: 'amountResidual', aggregate: 'sum' }
  ];
  filteredJournals: any;
  constructor(
    private fb: FormBuilder,
    private activeModal: NgbActiveModal,
    private authService: AuthService,
    private accountJournalService: AccountJournalService,
    private paymentService: AccountPaymentService,
    private intlService: IntlService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      amount: 0,
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

    if (this.rowsSelected) {
      var total = aggregateBy(this.rowsSelected, this.aggregates);
      this.formGroup.get('amount').setValue(total && total.amountResidual ? total.amountResidual.sum : 0)
    }

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
    this.create().subscribe((result: any) => {
      this.paymentService.post([result.id]).subscribe(() => {
        this.activeModal.close(true);
      }, (err) => {
      });
    }, (err) => {
    });
  }

  create() {
    var val = this.formGroup.value;
    val.journalId = val.journal.id;
    val.partnerId = this.partnerId;
    val.partnerType = this.partnerType;
    val.invoiceIds = this.rowsSelected.map(x => x.id);
    val.paymentDate = this.intlService.formatDate(val.paymentDateObj, 'd', 'en-US');
    return this.paymentService.create(val);
  }

}
