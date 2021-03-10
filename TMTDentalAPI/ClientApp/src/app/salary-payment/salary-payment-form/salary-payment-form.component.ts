import { State } from '@progress/kendo-data-query';
import { Component, OnInit, ViewChild } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { NgbActiveModal, NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { ComboBoxComponent } from "@progress/kendo-angular-dropdowns";
import { NotificationService } from "@progress/kendo-angular-notification";
import {
  AccountJournalFilter,
  AccountJournalService,
} from "src/app/account-journals/account-journal.service";
import { AuthService } from "src/app/auth/auth.service";
import { EmployeePaged, EmployeeSimple } from "src/app/employees/employee";
import { SalaryPaymentService } from "src/app/shared/services/salary-payment.service";
import * as _ from "lodash";
import { EmployeeService } from "src/app/employees/employee.service";
import { debounceTime, switchMap, tap } from "rxjs/operators";
import { IntlService } from '@progress/kendo-angular-intl';
import { PrintService } from 'src/app/shared/services/print.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { Observable } from 'rxjs';
import { AccountPaymentSave, AccountPaymentService } from 'src/app/account-payments/account-payment.service';
import { PartnerFilter, PartnerService } from 'src/app/partners/partner.service';
import { PartnerSimple } from 'src/app/partners/partner-simple';

@Component({
  selector: "app-salary-payment-form",
  templateUrl: "./salary-payment-form.component.html",
  styleUrls: ["./salary-payment-form.component.css"],
})
export class SalaryPaymentFormComponent implements OnInit {
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
    private salaryPaymentService: SalaryPaymentService,
    private authService: AuthService,
    private partnerService: PartnerService,
    private accountPaymentService: AccountPaymentService,
    private accountJournalService: AccountJournalService,
    private employeeService: EmployeeService,
    private intlService: IntlService,
    private printService: PrintService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      Name: null,
      DateObj: [null, Validators.required],
      Journal: [null, Validators.required],
      Partner: [null, Validators.required],
      Amount: [0, Validators.required],
      Reason: null,
    });


    setTimeout(() => {
      this.loadFilteredJournals();
      this.loadEmployees();
      this.filterChangeCombobox();
    });

    if (this.id) {
      this.loadData();
    } else {
      this.defaultGet();
    }
  }

  loadData() {
    this.salaryPaymentService.getIdSP(this.id).subscribe(
      (result) => {
        this.salaryPayment = result;
        let date = new Date(result.Date);
        this.formGroup.get('DateObj').patchValue(date);
        this.formGroup.patchValue(result);
        // this.formGroup.get('name').patchValue(result.Name);
        if (result.Employee) {
          result.Employee.id = result.Employee.Id;
          result.Employee.name = result.Employee.Name;
          this.filteredPartners = _.unionBy(this.filteredPartners, [result.Employee], "id");
        }

        if (result.Journal) {
          result.Journal.id = result.Journal.Id;
          result.Journal.name = result.Journal.Name;
          this.filteredJournals = _.unionBy(this.filteredJournals, [result.Journal], "id");
        }

      },
      (error) => {
        console.log(error);
      }
    );
  }

  defaultGet() {
    this.formGroup.get('DateObj').patchValue(new Date());
  }

  loadFilteredJournals() {
    var val = new AccountJournalFilter();
    val.type = "bank,cash";
    val.companyId = this.authService.userInfo.companyId;
    this.accountJournalService.autocomplete(val).subscribe(
      (result) => {
        this.filteredJournals = result;
        if (this.filteredJournals.length) {
          this.formGroup.get('Journal').patchValue(this.filteredJournals[0]);
        }
      },
      (error) => {
        console.log(error);
      }
    );
  }

  onChangeDate(date) {
    console.log(date);
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

  filterChangeCombobox() {
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
  }

  onSave$() {
    this.submitted = true;
    if (!this.formGroup.valid) {
      return false;
    }


    const salaryPayment = Object.assign({}, this.formGroup.value);
    salaryPayment.JournalId = salaryPayment.Journal.id;
    salaryPayment.EmployeeId = salaryPayment.Employee ? salaryPayment.Employee.id : null;
    salaryPayment.Date = this.intlService.formatDate(salaryPayment.DateObj, 'yyyy-MM-ddTHH:mm:ss');
    salaryPayment.Type = salaryPayment.Type;
    salaryPayment.CompanyId = this.authService.userInfo.companyId;

    if (this.id) {
      return this.salaryPaymentService.update(this.id, salaryPayment);
    } else {
      return this.salaryPaymentService.create(salaryPayment);
    }
  }

  onSave() {
    const save$ = this.onSave$() as Observable<any>;
    save$.subscribe((res: any) => {
      if (this.id) {
        this.id = this.id;
      } else {
        this.id = res.Id;
      }
      this.loadData();
      this.activeModal.close()
      this.printSalaryPayment(this.id);
    });
  }

  onSaveV2() {
    var value = this.formGroup.value;
    var val = new AccountPaymentSave();
    val.amount = value.Amount;
    val.communication = value.Reason;
    val.journalId = value.Journal.id;
    val.partnerId = value.Partner.id;
    val.partnerType = "employee.advance";
    val.paymentDate = this.intlService.formatDate(value.DateObj, "yyyy-MM-ddTHH:mm");
    val.paymentType = "outbound";
    if (this.id) {
      this.accountPaymentService.update(this.id, val).subscribe(
        result => {
          this.loadData();
          this.activeModal.close()
          this.printSalaryPayment(this.id);
        }
      )
    } else {
      this.accountPaymentService.create(val).subscribe(
        result => {
          this.loadData();
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
        this.accountPaymentService.post(['e7b74910-12b4-43cc-c74e-08d8c1d8ca78']).subscribe(
          result=>{
            this.notify('success','Xác nhận tạm ứng lương thành công')
          }
        )
      })
  }

  actionConfirm() {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: "sm", windowClass: "o_technical_modal", keyboard: false, backdrop: "static" });
    modalRef.componentInstance.title = "Xác nhận tạm ứng lương";
    modalRef.componentInstance.body = "Bạn có chắc chắn muốn tạm ứng lương?";
    modalRef.result.then(
      () => {
        if (!this.salaryPayment) {
          const save$ = this.onSave$() as Observable<any>;
          save$.subscribe((res: any) => {
            this.salaryPaymentService.actionConfirm([res.Id]).subscribe(
              () => {
                this.notify('success', 'Tạm ứng lương thành công');
                this.activeModal.close();
              },
              (error) => {
                console.log(error);
              }
            );
          });
        } else {
          this.salaryPaymentService.actionConfirm([this.id]).subscribe(
            () => {
              this.notify('success', 'Tạm ứng lương thành công');
              this.activeModal.close();
            },
            (error) => {
              console.log(error);
            }
          );
        }
      },
      () => { }
    );
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

  checkIsDisable(id) {
    if (id && this.salaryPayment && this.salaryPayment.State !== 'waiting') {
      return true;
    } else {
      return false;
    }
  }

  printSalaryPayment(ids) {
    this.salaryPaymentService.onPrint([ids]).subscribe(
      result => {
        if (result && result['html']) {
          this.printService.printHtml(result['html']);
        } else {
          alert('Có lỗi xảy ra, thử lại sau');
        }
      }
    )
  }

  get f() {
    return this.formGroup.controls;
  }
}
