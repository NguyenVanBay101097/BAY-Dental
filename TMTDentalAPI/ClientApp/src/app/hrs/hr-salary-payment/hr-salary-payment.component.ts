import { Component, Input, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AccountJournalFilter, AccountJournalService } from 'src/app/account-journals/account-journal.service';
import { AuthService } from 'src/app/auth/auth.service';
import { SalaryPaymentSave, SalaryPaymentSaveDefault, SalaryPaymentService } from 'src/app/shared/services/salary-payment.service';

@Component({
  selector: 'app-hr-salary-payment',
  templateUrl: './hr-salary-payment.component.html',
  styleUrls: ['./hr-salary-payment.component.css']
})
export class HrSalaryPaymentComponent implements OnInit {
  @Input() defaultPara: any;

  filteredJournals: any;
  // paymentFA: FormArray;
  paymentForm: FormGroup;

  constructor(
    private accountJournalService: AccountJournalService,
    private authService: AuthService,
    private activeModal: NgbActiveModal,
    private paymentService: SalaryPaymentService,
    private fb: FormBuilder
  ) { }

  ngOnInit() {
    // this.paymentFA = new FormArray([]);
    this.paymentForm = this.fb.group({
      paymentFA: new FormArray([])
    });
    setTimeout(() => {
      this.loadDefaultRecord();
      this.loadFilteredJournals();
    });
  }

  get paymentFA() {return this.paymentForm.get('paymentFA') as FormArray; }

  loadDefaultRecord() {
    if (this.defaultPara) {
      this.paymentService.defaulCreateBy(this.defaultPara).subscribe((res: any) => {
        this.paymentFA.clear();
        res.value.forEach(e => {
          // e.Journal = {
          //   id: e.Journal.Id,
          //   name: e.Journal.Name
          // };
          const fg = this.fb.group(new SalaryPaymentSaveDefault());
          fg.patchValue(e);
          this.paymentFA.push(fg);
        });
        console.log(this.paymentFA);
        console.log(res.value);
        
      });
    }
  }

  loadFilteredJournals() {
    const val = new AccountJournalFilter();
    val.type = 'bank,cash';
    val.companyId = this.authService.userInfo.companyId;
    this.accountJournalService.autocomplete(val).subscribe(
      (result) => {
        this.filteredJournals = result;
        // if (this.filteredJournals.length) {
        //   this.paymentFA.controls.forEach(fc => {
        //     fc.get('Journal').patchValue(this.filteredJournals[0]);
        //     fc.get('JournalId').patchValue(this.filteredJournals[0].id);
        //   });
        // }
      },
      (error) => {
        console.log(error);
      }
    );
  }

  onChangeJournal(e) {
    console.log(e);
    
  }
}
