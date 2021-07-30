import { Component, Input, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { AccountJournalFilter, AccountJournalService } from 'src/app/account-journals/account-journal.service';
import { AccountPaymentSave, AccountPaymentService } from 'src/app/account-payments/account-payment.service';
import { AuthService } from 'src/app/auth/auth.service';
import { SalaryPaymentService } from 'src/app/salary-payment/salary-payment.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PrintService } from 'src/app/shared/services/print.service';
import { SalaryPaymentSaveDefault } from 'src/app/shared/services/salary-payment.service';

@Component({
  selector: 'app-hr-salary-payment',
  templateUrl: './hr-salary-payment.component.html',
  styleUrls: ['./hr-salary-payment.component.css']
})
export class HrSalaryPaymentComponent implements OnInit {
  @Input() payslipIds: any;
  isDisable = false;

  filteredJournals: any;
  // paymentFA: FormArray;
  paymentForm: FormGroup;
  payments = [];
  title: string;
  payslipRunId: string;

  constructor(
    private authService: AuthService,
    public activeModal: NgbActiveModal,
    private paymentService: SalaryPaymentService,
    private fb: FormBuilder,
    private journalService: AccountJournalService,
    private notificationService: NotificationService,
    private printService: PrintService,
    private modalService: NgbModal,
    private intlService: IntlService
  ) { }

  ngOnInit() {
    // this.paymentFA = new FormArray([]);
    this.paymentForm = this.fb.group({
      paymentFA: this.fb.array([])
    });
    setTimeout(() => {
      this.loadDefaultRecord();
      this.loadFilteredJournals();
    });
  }

  get paymentFA() { return this.paymentForm.get('paymentFA') as FormArray; }

  loadDefaultRecord() {
      if (!this.payslipIds || !this.payslipIds.length) {
      this.isDisable = true;
    }
    if (this.payments.length > 0) {
        this.paymentFA.clear();
        this.payments.forEach(e => {
            const fg = this.fb.group(e);
            this.paymentFA.push(fg);
        });
    }
  }

  loadFilteredJournals() {
    var val = new AccountJournalFilter();
    val.type = 'bank,cash';
    val.companyId = this.authService.userInfo.companyId;
    this.journalService.autocomplete(val).subscribe(result => {
      this.filteredJournals = result;
    });
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
      debugger;
      const val = this.paymentFA.value;
      var data = val.map(x => {
        return {
          date: x.date,
          employeeId: x.employee.id,
          journalId: x.journal.id,
          amount: x.amount,
          reason: x.reason,
          payslipId: x.payslipId
        };
      });
      this.paymentService.actionMultiSalaryPayment(data).subscribe(() => {
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
      var data = val.map(x => {
        return {
          date: x.date,
          employeeId: x.employee.id,
          journalId: x.journal.id,
          amount: x.amount,
          reason: x.reason,
          payslipId: x.payslipId
        };
      });
      this.paymentService.actionMultiSalaryPayment(data).subscribe((res: any) => {
        this.notify('success', 'Xác nhận thành công');
        this.activeModal.close();
        if (!res) {
          this.notify('error', 'Không có phiếu chi lương để in');
        }
        this.paymentService.getPrint(res).subscribe(
          result => {
            this.printService.printHtml(result);
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
