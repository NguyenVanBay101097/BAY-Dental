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
import { SalaryPaymentDisplay, SalaryPaymentService } from '../salary-payment.service';

@Component({
  selector: "app-salary-payment-form",
  templateUrl: "./salary-payment-form.component.html",
  styleUrls: ["./salary-payment-form.component.css"],
})
export class SalaryPaymentFormComponent implements OnInit {
  formGroup: FormGroup;
  type: string;
  title: string;
  id: string;
  salaryPayment: SalaryPaymentDisplay = new SalaryPaymentDisplay();
  filteredJournals: any = [];
  filteredEmployees: EmployeeSimple[] = [];
  public minDateTime: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  public maxDateTime: Date = new Date(new Date(this.monthEnd.setHours(23, 59, 59)).toString());
  @ViewChild("journalCbx", { static: true }) journalCbx: ComboBoxComponent;
  @ViewChild("employeeCbx", { static: true }) employeeCbx: ComboBoxComponent;
  submitted = false;

  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private modalService: NgbModal,
    private notificationService: NotificationService,
    private salaryPaymentService: SalaryPaymentService,
    private authService: AuthService,
    private accountJournalService: AccountJournalService,
    private employeeService: EmployeeService,
    private intlService: IntlService,
    private printService: PrintService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: null,
      dateObj: [null, Validators.required],
      journal: [null, Validators.required],
      employee: [null, Validators.required],
      amount: [0, Validators.required],
      reason: null,
    });


    setTimeout(() => {
      this.loadFilteredJournals();
      this.loadEmployees();
      this.filterChangeCombobox();

      if (this.id) {
        this.loadData();
      } else {
        this.defaultGet();
      }
    });
  }

  loadData() {
    this.salaryPaymentService.get(this.id).subscribe(
      (result: any) => {
        console.log(result);
        this.salaryPayment = result;
        let date = new Date(result.date);
        this.formGroup.get('dateObj').patchValue(date);
        this.formGroup.patchValue(result);
        if (result.employee) {
          this.filteredEmployees = _.unionBy(this.filteredEmployees, [result.employee], "id");
        }
        if (result.journal) {
          this.filteredJournals = _.unionBy(this.filteredJournals, [result.journal], "id");
        }
      },
      (error) => {
        console.log(error);
      }
    );
  }

  defaultGet() {
    this.salaryPaymentService.defaultGet(this.type).subscribe(
      (result: any) => {
        this.salaryPayment = result;
        let date = new Date(result.date);
        this.formGroup.get('dateObj').patchValue(date);
        this.formGroup.patchValue(result);
        if (result.journal) {
          this.filteredJournals = _.unionBy(this.filteredJournals, [result.journal], "id");
        }
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
        this.filteredJournals = _.unionBy(this.filteredJournals, result, "id");
      },
      (error) => {
        console.log(error);
      }
    );
  }

  loadEmployees() {
    this.searchEmployees().subscribe((result) => {
      this.filteredEmployees = _.unionBy(this.filteredEmployees, result.items, "id");
    });
  }

  searchEmployees(filter?: string) {
    var val = new EmployeePaged();
    val.active = true;
    val.search = filter || "";
    return this.employeeService.getEmployeePaged(val);
  }

  filterChangeCombobox() {
    this.employeeCbx.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.employeeCbx.loading = true)),
        switchMap((val) => this.searchEmployees(val.toString().toLowerCase()))
      )
      .subscribe((result) => {
        this.filteredEmployees = result.items;
        this.employeeCbx.loading = false;
      });
  }

  onSave$(): Observable<Object> {
    const salaryPayment = Object.assign({}, this.formGroup.value);
    salaryPayment.journalId = salaryPayment.journal.id;
    salaryPayment.employeeId = salaryPayment.employee.id;
    salaryPayment.date = this.intlService.formatDate(salaryPayment.dateObj, 'yyyy-MM-ddTHH:mm:ss');
    salaryPayment.type = this.type;

    if (this.id) {
      return this.salaryPaymentService.update(this.id, salaryPayment);
    } else {
      return this.salaryPaymentService.create(salaryPayment);
    }
  }

  onSavePrint() {
    this.submitted = true;
    if (!this.formGroup.valid) {
      return false;
    }

    this.onSave$().subscribe((res: any) => {
      this.activeModal.close({
        id: res ? res.id : this.id,
        print: true
      });
    });
  }

  actionConfirm() {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: "sm", windowClass: "o_technical_modal", keyboard: false, backdrop: "static" });
    modalRef.componentInstance.title = "Xác nhận tạm ứng lương";
    modalRef.componentInstance.body = "Bạn có chắc chắn muốn tạm ứng lương?";
    modalRef.result.then(
      () => {
        if (!this.id) {
          const save$ = this.onSave$() as Observable<any>;
          save$.subscribe((res: any) => {
            this.salaryPaymentService.actionConfirm([res.id]).subscribe(
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

  get editable() {
    return this.salaryPayment.state == 'waiting';
  }

  printItem(id) {
    this.salaryPaymentService.getPrint([id]).subscribe(
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
