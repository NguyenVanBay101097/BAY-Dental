import { Component, OnInit, ViewChild } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
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

@Component({
  selector: "app-salary-payment-form",
  templateUrl: "./salary-payment-form.component.html",
  styleUrls: ["./salary-payment-form.component.css"],
})
export class SalaryPaymentFormComponent implements OnInit {
  formGroup: FormGroup;
  title: string;
  id: string;
  filteredJournals: any = [];
  filteredEmployees: EmployeeSimple[] = [];
  @ViewChild("journalCbx", { static: true }) journalCbx: ComboBoxComponent;
  @ViewChild("employeeCbx", { static: true }) employeeCbx: ComboBoxComponent;

  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private notificationService: NotificationService,
    private salaryPaymentService: SalaryPaymentService,
    private authService: AuthService,
    private accountJournalService: AccountJournalService,
    private employeeService: EmployeeService, 
    private intlService: IntlService
  ) {}

  ngOnInit() {
    this.formGroup = this.fb.group({
      Name: null,
      DateObj: [null, Validators.required],
      Journal: [null, Validators.required],
      Employee: [null, Validators.required],
      Amount: 0,
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
    this.salaryPaymentService.get(this.id, null).subscribe(
      (result) => {
        console.log(result);
        this.formGroup.patchValue(result);
        // this.formGroup.get('name').patchValue(result.Name);
        if (result.employee) {
          this.filteredEmployees = _.unionBy(this.filteredEmployees, [result.employee], "id");
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

  loadEmployees() {
    this.searchEmployees().subscribe((result) => {
      this.filteredEmployees = _.unionBy(this.filteredEmployees, result.items, "id");
    });
  }

  searchEmployees(filter?: string) {
    var val = new EmployeePaged();
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

  onSave() {
    if (!this.formGroup.valid) {
      return false;
    }

    var salaryPayment = this.formGroup.value;
    salaryPayment.JournalId = salaryPayment.Journal.id;
    salaryPayment.EmployeeId = salaryPayment.Employee ? salaryPayment.Employee.id : null;
    salaryPayment.Date = this.intlService.formatDate(salaryPayment.DateObj, 'yyyy-MM-ddTHH:mm:ss');

    console.log(salaryPayment);
    this.salaryPaymentService.create(salaryPayment).subscribe(
      (result) => {
        
      },
      (error) => {
        console.log(error);
      }
    );
  }
}
