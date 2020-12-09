import { Component, Input, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { AccountJournalFilter } from 'src/app/account-journals/account-journal.service';
import { AuthService } from 'src/app/auth/auth.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { AccountJournalService } from 'src/app/shared/services/account-journal.service';
import { PrintService } from 'src/app/shared/services/print.service';
import { SalaryPaymentSave, SalaryPaymentSaveDefault, SalaryPaymentService } from 'src/app/shared/services/salary-payment.service';

@Component({
  selector: 'app-hr-salary-payment',
  templateUrl: './hr-salary-payment.component.html',
  styleUrls: ['./hr-salary-payment.component.css']
})
export class HrSalaryPaymentComponent implements OnInit {
  @Input() defaultPara: any;
  isDisable = false;

  filteredJournals: any;
  // paymentFA: FormArray;
  paymentForm: FormGroup;
  title: string;

  constructor(
    private authService: AuthService,
    public activeModal: NgbActiveModal,
    private paymentService: SalaryPaymentService,
    private fb: FormBuilder,
    private journalService: AccountJournalService,
    private notificationService: NotificationService,
    private printService: PrintService,
    private modalService: NgbModal
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
    if (!this.defaultPara.PayslipIds || !this.defaultPara.PayslipIds.length) {
      this.isDisable = true;
    }
    if (this.defaultPara) {
      this.paymentService.defaulCreateBy(this.defaultPara).subscribe((res: any) => {
        this.paymentFA.clear();
        res.value.forEach(e => {
          const fg = this.fb.group(new SalaryPaymentSaveDefault());
          fg.patchValue(e);
          this.paymentFA.push(fg);
        });
      },
        (error: any) => {
          this.isDisable = true;
        }
      );
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
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Chi lương';
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn chi lương?';
    modalRef.result.then(() => {
      const val = this.paymentFA.value;
      this.paymentService.actionMultiSalaryPayment(val).subscribe(() => {
        this.notify('success', 'Xác nhận thành công');
        this.activeModal.close();
      });
    });
  }

  onSaveAndPrint() {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Chi lương';
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn chi lương?';
    modalRef.result.then(() => {
      const val = this.paymentFA.value;
      this.paymentService.actionMultiSalaryPayment(val).subscribe((res: any) => {
        this.notify('success', 'Xác nhận thành công');
        this.activeModal.close();
        if (!res.value) {
          this.notify('error', 'Không có phiếu chi lương để in');
        }
        this.paymentService.onPrint(res.value).subscribe(
          result => {
            this.printService.print2(result['html']);
          }
        );

      });
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

  onRemovePayment(i) {
    this.paymentFA.removeAt(i);
    console.log(this.paymentFA);

  }

}
