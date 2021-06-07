import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import * as _ from 'lodash';
import { mergeMap } from 'rxjs/operators';
import { AccountJournalFilter, AccountJournalService } from 'src/app/account-journals/account-journal.service';
import { AuthService } from 'src/app/auth/auth.service';
import { AmountCustomerDebtFilter, CustomerDebtReportService } from 'src/app/core/services/customer-debt-report.service';
import { PhieuThuChiService } from 'src/app/phieu-thu-chi/phieu-thu-chi.service';
import { PartnerService } from '../partner.service';

@Component({
  selector: 'app-partner-customer-debt-payment-dialog',
  templateUrl: './partner-customer-debt-payment-dialog.component.html',
  styleUrls: ['./partner-customer-debt-payment-dialog.component.css']
})
export class PartnerCustomerDebtPaymentDialogComponent implements OnInit {
  title: string;
  type: string;
  partnerId: string;
  accountType: string;
  formGroup: FormGroup;
  submitted: boolean = false;
  filteredJournals: any = [];
  amountDebtBalanceTotal = 0;


  @ViewChild("journalCbx", { static: true }) journalCbx: ComboBoxComponent;

  constructor(private phieuthuchiService: PhieuThuChiService, private fb: FormBuilder, private intlService: IntlService,  private customerDebtReportService: CustomerDebtReportService,
    public activeModal: NgbActiveModal, private accountJournalService: AccountJournalService, private partnerService: PartnerService,
    private authService: AuthService) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      amount: [0, Validators.required],
      dateObj: [null, Validators.required],
      journal: [null, Validators.required],
      reason: null,
    });
    this.loadDefault();
    this.loadFilteredJournals();
    this.loadAmountDebtBalanceTotal();
  }

  loadDefault() {
    this.phieuthuchiService.defaultGet({ type: this.type }).subscribe((rs: any) => {
      this.formGroup.patchValue(rs);
      var paymentDate = new Date(rs.date);
      this.formGroup.get('dateObj').setValue(paymentDate);

      if (rs.journal) {
        this.filteredJournals = _.unionBy(this.filteredJournals, rs.journal, 'id');
      }

    })
  }


  loadFilteredJournals() {
    var val = new AccountJournalFilter();
    val.type = "bank,cash";
    val.companyId = this.authService.userInfo.companyId;
    this.accountJournalService.autocomplete(val).subscribe((res) => {
      this.filteredJournals = _.unionBy(this.filteredJournals, res, 'id');
    },
      (error) => {
        console.log(error);
      }
    );
  }

  get f() { return this.formGroup.controls; }

  actionPayment() {
    this.submitted = true;

    if (!this.formGroup.valid) {
      return false;
    }

    var val = this.formGroup.value;
    val.journalId = val.journal ? val.journal.id : null;
    val.partnerId = this.partnerId ? this.partnerId : null;
    val.accountType = this.accountType;
    val.type = this.type;
    val.date = this.intlService.formatDate(val.dateObj, "yyyy-MM-ddTHH:mm:ss");
    this.phieuthuchiService.create(val).pipe(mergeMap((res: any) => {
      return this.phieuthuchiService.actionConfirm([res.id]);
    }))
      .subscribe(r => {
        this.activeModal.close(true);
      });
  }

  loadAmountDebtBalanceTotal() {
    var val = new AmountCustomerDebtFilter();
    val.partnerId = this.partnerId;
    val.companyId = this.authService.userInfo.companyId;
    this.customerDebtReportService.getAmountDebtTotal(val).subscribe((rs:any) => {
      this.amountDebtBalanceTotal = rs.balanceTotal;
    });
  }

  onClose() {
    this.activeModal.dismiss();
  }

}
