import { Component, Input, OnInit } from '@angular/core';
import { FormArray } from '@angular/forms';
import { AccountJournalFilter, AccountJournalService } from 'src/app/account-journals/account-journal.service';
import { AuthService } from 'src/app/auth/auth.service';

@Component({
  selector: 'app-hr-salary-payment',
  templateUrl: './hr-salary-payment.component.html',
  styleUrls: ['./hr-salary-payment.component.css']
})
export class HrSalaryPaymentComponent implements OnInit {
  @Input() payments: any;

  filteredJournals: any;
  paymentFA: FormArray;

  constructor(
    private accountJournalService: AccountJournalService,
    private authService: AuthService
  ) { }

  ngOnInit() {
    this.paymentFA = new FormArray([]);
    this.paymentFA.patchValue(this.payments);
    this.loadFilteredJournals();
  }

  loadFilteredJournals() {
    const val = new AccountJournalFilter();
    val.type = 'bank,cash';
    val.companyId = this.authService.userInfo.companyId;
    this.accountJournalService.autocomplete(val).subscribe(
      (result) => {
        this.filteredJournals = result;
        if (this.filteredJournals.length) {
          this.paymentFA.controls.forEach(fc => {
            fc.get('Journal').patchValue(this.filteredJournals[0]);
            fc.get('JournalId').patchValue(this.filteredJournals[0].id);
          });
        }
      },
      (error) => {
        console.log(error);
      }
    );
  }
}
