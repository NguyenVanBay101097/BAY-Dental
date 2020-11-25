import { Component, Input, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { AccountJournalFilter } from 'src/app/account-journals/account-journal.service';
import { AuthService } from 'src/app/auth/auth.service';
import { AccountJournalService } from 'src/app/shared/services/account-journal.service';
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
    private authService: AuthService,
    private activeModal: NgbActiveModal,
    private paymentService: SalaryPaymentService,
    private fb: FormBuilder,
    private journalService: AccountJournalService,
    private notificationService: NotificationService
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

  get paymentFA() { return this.paymentForm.get('paymentFA') as FormArray; }

  loadDefaultRecord() {
    if (this.defaultPara) {
      this.paymentService.defaulCreateBy(this.defaultPara).subscribe((res: any) => {
        this.paymentFA.clear();
        res.value.forEach(e => {
          const fg = this.fb.group(new SalaryPaymentSaveDefault());
          fg.patchValue(e);
          this.paymentFA.push(fg);
        });

      });
    }
  }

  loadFilteredJournals() {
    const state = {
      filter: {
        logic: 'and',
        filters: [
          {
            logic: 'or',
            filters: [
              { field: 'Type', operator: 'eq', value: 'bank' },
              { field: 'Type', operator: 'eq', value: 'cash' }
            ]
          },
          { field: 'CompanyId ', operator: 'eq', value: this.authService.userInfo.companyId }
        ]
      }
    };

    this.journalService.fetch2(state).subscribe(
      (result: any) => {
        this.filteredJournals = result.data;

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
    const val = e.target;
    console.log(val);


  }
  onSave() {
    const val = this.paymentFA.value;
    this.paymentService.actionMultiSalaryPayment(val).subscribe(() => {
      this.notify('success', 'xác nhận thành công');
      this.activeModal.close();
    });
  }

  onSaveAndPrint() {
    const val = this.paymentFA.value;
    this.paymentService.actionMultiSalaryPayment(val).subscribe((res: any) => {
      this.notify('success', 'xác nhận thành công');
      this.activeModal.close();
      if (!res.value) {
        this.notify('error', 'không có phiếu chi lương để in');
      }
      this.paymentService.onPrint(res.value).subscribe(
        result => {
          const popupWin = window.open('', '_blank', 'top=0,left=0,height=auto,width=auto');
          popupWin.document.open();

          popupWin.document.write(result['html']);
          popupWin.document.close();
        }
      );

    });
  }

  notify(Style, Content) {
    this.notificationService.show({
      content: Content,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: Style, icon: true }
    });
  }

}
