import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { NotificationService } from '@progress/kendo-angular-notification';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { AccountJournalFilter, AccountJournalService } from 'src/app/account-journals/account-journal.service';
import { AuthService } from 'src/app/auth/auth.service';
import { SalaryPaymentService } from 'src/app/salary-payment/salary-payment.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PrintService } from 'src/app/shared/services/print.service';

@Component({
  selector: 'app-hr-salary-payment',
  templateUrl: './hr-salary-payment.component.html',
  styleUrls: ['./hr-salary-payment.component.css']
})
export class HrSalaryPaymentComponent implements OnInit {
  @ViewChild('journalCbx', { static: true }) journalCbx: ComboBoxComponent;

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
    this.searchFilteredJournals().subscribe(result => {
      this.filteredJournals = result;
    });
  }

  searchFilteredJournals(q?: string) {
    var val = new AccountJournalFilter();
    val.type = "bank,cash";
    val.search = q || '';
    val.companyId = this.authService.userInfo.companyId;
    val.limit = 0;
    return this.journalService.autocomplete(val);
  }

  onChangeJournal(e) {
    // const val = e.target;
  }

  onSave() {
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
          (result: any) => {
            this.printService.printHtml(result.html);
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
  }

}
