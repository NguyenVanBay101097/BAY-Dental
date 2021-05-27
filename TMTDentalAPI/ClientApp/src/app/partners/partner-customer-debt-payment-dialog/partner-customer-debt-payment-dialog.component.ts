import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import * as _ from 'lodash';
import { AccountJournalFilter, AccountJournalService } from 'src/app/account-journals/account-journal.service';
import { AuthService } from 'src/app/auth/auth.service';
import { PhieuThuChiService } from 'src/app/phieu-thu-chi/phieu-thu-chi.service';

@Component({
  selector: 'app-partner-customer-debt-payment-dialog',
  templateUrl: './partner-customer-debt-payment-dialog.component.html',
  styleUrls: ['./partner-customer-debt-payment-dialog.component.css']
})
export class PartnerCustomerDebtPaymentDialogComponent implements OnInit {
  title: string;
  type: string;
  partnerId: string;
  formGroup: FormGroup;
  submitted: boolean = false;
  filteredJournals: any = [];

  @ViewChild("journalCbx", { static: true }) journalCbx: ComboBoxComponent;
  
  constructor(private phieuthuchiService: PhieuThuChiService, private fb: FormBuilder, private intlService: IntlService,
    public activeModal: NgbActiveModal, private notificationService: NotificationService, private accountJournalService: AccountJournalService,
     private authService: AuthService) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      amount: 0,
      dateObj: [null, Validators.required],
      journal: [null, Validators.required],
      reason: null,
    });
    this.loadDefault();
    this.loadFilteredJournals();
  }

  loadDefault(){
    this.phieuthuchiService.defaultGet({ type: this.type }).subscribe((rs:any) =>{
      this.formGroup.patchValue(rs);
      var paymentDate = new Date(rs.date);
      this.formGroup.get('dateObj').setValue(paymentDate);

      if(rs.journal){
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

  actionPayment(){
    this.submitted = true;

    if (!this.formGroup.valid) {
      return false;
    }

    var val = this.formGroup.value;
    val.journalId = val.journal ? val.journal.id : null;
    val.partnerId = this.partnerId ? this.partnerId : null;
    val.date = this.intlService.formatDate(val.dateObj, "yyyy-MM-ddTHH:mm:ss");
    this.phieuthuchiService.actionPaymentCustomerDebt(val).subscribe(
      () => {
        this.activeModal.close(true);
      },
    )
  }

  onCancel() {
    this.activeModal.dismiss();
  }

}
