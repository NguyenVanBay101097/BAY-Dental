import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import * as _ from 'lodash';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { AccountJournalFilter, AccountJournalService } from 'src/app/account-journals/account-journal.service';
import { AccountPaymentSave, AccountPaymentService } from 'src/app/account-payments/account-payment.service';
import { AuthService } from 'src/app/auth/auth.service';
import { EmployeeService } from 'src/app/employees/employee.service';
import { PartnerSimple } from 'src/app/partners/partner-simple';
import { PartnerFilter, PartnerService } from 'src/app/partners/partner.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PrintService } from 'src/app/shared/services/print.service';

@Component({
  selector: 'app-salary-payment-dialog-v2',
  templateUrl: './salary-payment-dialog-v2.component.html',
  styleUrls: ['./salary-payment-dialog-v2.component.css']
})
export class SalaryPaymentDialogV2Component implements OnInit {
  formGroup: FormGroup;
  title: string;
  id: string;
  salaryPayment: any;
  filteredJournals: any = [];
  filteredPartners: PartnerSimple[] = [];
  public minDateTime: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  public maxDateTime: Date = new Date(new Date(this.monthEnd.setHours(23, 59, 59)).toString());
  @ViewChild("journalCbx", { static: true }) journalCbx: ComboBoxComponent;
  @ViewChild("partnerCbx", { static: true }) partnerCbx: ComboBoxComponent;
  submitted = false;
  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private modalService: NgbModal,
    private notificationService: NotificationService,
    private accountPaymentService: AccountPaymentService,
    private authService: AuthService,
    private partnerService: PartnerService,
    private accountJournalService: AccountJournalService,
    private intlService: IntlService,
    private printService: PrintService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: null,
      paymentDate: [null, Validators.required],
      journal: [null, Validators.required],
      partner: [null, Validators.required],
      amount: [0, Validators.required],
      communication: null,
    });

    this.partnerCbx.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.partnerCbx.loading = true)),
        switchMap((val) => this.searchEmployees(val.toString().toLowerCase()))
      )
      .subscribe((result) => {
        this.filteredPartners = result;
        this.partnerCbx.loading = false;
      });

    this.loadFilteredJournals();
    this.loadEmployees();

    if (this.id) {
      this.loadData();
    } else {
      this.defaultGet();
    }
  }

  defaultGet() {
    this.formGroup.get('paymentDate').patchValue(new Date());
  }

  loadData() {
    this.accountPaymentService.get(this.id).subscribe(
      result => {
        this.salaryPayment = result;
        this.salaryPayment.paymentDate = new Date(result.paymentDate);
        this.formGroup.patchValue(this.salaryPayment);
      },
      (error) => {
        console.log(error);
      }
    );
  }

  loadFilteredJournals() {
    var val = new AccountJournalFilter();
    val.type = "bank,cash";
    val.companyId = this.authService.userInfo.companyId;
    this.accountJournalService.autocomplete(val).subscribe(
      (result) => {
        this.filteredJournals = result;
        if (this.filteredJournals.length) {
          this.formGroup.get('journal').patchValue(this.filteredJournals[0]);
        }
      },
      (error) => {
        console.log(error);
      }
    );
  }

  loadEmployees() {
    this.searchEmployees().subscribe((result) => {
      this.filteredPartners = _.unionBy(this.filteredPartners, result, "id");
    });
  }

  searchEmployees(filter?: string) {
    var val = new PartnerFilter();
    val.employee = true;
    val.active = true;
    val.search = filter || "";
    return this.partnerService.autocomplete2(val);
  }

  getValueForm() {
    var value = this.formGroup.value;
    value.journalId = value.journal.id;
    value.partnerId = value.partner.id;
    value.partnerType = "employee";
    value.paymentDate = this.intlService.formatDate(value.paymentDate, "yyyy-MM-ddTHH:mm");
    value.paymentType = "outbound";
    return value;
  }

  onSaveV2() {
    var value = this.getValueForm();
    if (this.id) {
      this.accountPaymentService.update(this.id, value).subscribe(
        result => {
          this.notify('success', 'Sửa tạm ứng lương thành công');
          this.activeModal.close()
          this.printSalaryPayment(this.id);
        }
      )
    } else {
      this.accountPaymentService.create(value).subscribe(
        result => {
          this.notify('success', 'Tạo tạm ứng lương thành công');
          this.activeModal.close()
          this.printSalaryPayment(this.id);
        }
      )
    }
  }

  actionConfirmV2() {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: "sm", windowClass: "o_technical_modal", keyboard: false, backdrop: "static" });
    modalRef.componentInstance.title = "Xác nhận tạm ứng lương";
    modalRef.componentInstance.body = "Bạn có chắc chắn muốn tạm ứng lương?";
    modalRef.result.then(
      () => {
        if (this.id) {
          this.accountPaymentService.post([this.id]).subscribe(
            result => {
              this.notify('success', 'Xác nhận tạm ứng lương thành công');
              this.activeModal.close();
            }
          )
        } else {
          var value = this.getValueForm();
          this.accountPaymentService.create(value).subscribe(
            result => {
              if (result) {
                this.accountPaymentService.post([result.id]).subscribe(
                  () => {
                    this.notify('success', 'Tạo tạm ứng và xác nhận lương thành công');
                    this.activeModal.close();
                  }
                )
              }
            }
          )
        }
      })
  }

  notify(style, content) {
    this.notificationService.show({
      content: content,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: style, icon: true }
    });
  }

  printSalaryPayment(ids) {
    // this.salaryPaymentService.onPrint([ids]).subscribe(
    //   result => {
    //     if (result && result['html']) {
    //       this.printService.printHtml(result['html']);
    //     } else {
    //       alert('Có lỗi xảy ra, thử lại sau');
    //     }
    //   }
    // )
  }

  checkIsDisable(id) {
    if (id && this.salaryPayment && this.salaryPayment.state == 'posted') {
      return true;
    } else {
      return false;
    }
  }

  get f() {
    return this.formGroup.controls;
  }
}
