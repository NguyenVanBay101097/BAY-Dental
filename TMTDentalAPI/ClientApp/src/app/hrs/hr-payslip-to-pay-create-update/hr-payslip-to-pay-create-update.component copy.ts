import { Component, OnInit, ViewChild } from '@angular/core';
import { HrPayslipDisplay, HrPayslipService } from '../hr-payslip.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { NotificationService } from '@progress/kendo-angular-notification';
import { EmployeeDisplay, EmployeePaged } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-hr-payslip-to-pay-create-update',
  templateUrl: './hr-payslip-to-pay-create-update.component.html',
  styleUrls: ['./hr-payslip-to-pay-create-update.component.css']
})
export class HrPayslipToPayCreateUpdateComponent implements OnInit {


  payslipRecord: HrPayslipDisplay;
  payslipForm: FormGroup;
  employee: any;
  listEmployees: any[];

  constructor(
    private fb: FormBuilder, private router: Router,
    private hrPayslipService: HrPayslipService,
    private employeeService: EmployeeService,
    private notificationService: NotificationService,
    private activeroute: ActivatedRoute
  ) { }

  ngOnInit() {
    this.payslipForm = this.fb.group({
      strucktId: null,
      employeeId: null,
      employee: [null, [Validators.required]],
      dateFrom: [null, [Validators.required]],
      dateTo: [null, [Validators.required]],
      name: [null, [Validators.required]],
      state: 'draft',
      number: null,
      Lines: null,
    });

    this.GetEmployeePaged();
  }
  get Form(){ return this.payslipForm; }

  GetEmployeePaged() {
    const val = new EmployeePaged();
    this.employeeService.getEmployeeSimpleList(val).subscribe(res => {
      this.listEmployees = res;
    });

  }

  EmployeeFilter(value) {
    const val = new EmployeePaged();
    val.search = value;
    return this.employeeService.getEmployeeSimpleList(val).subscribe((result: any) => {
      this.listEmployees = result;
    });
  }

  EmployeeChange(e) {
    this.employeeService.getEmployee(e.id).subscribe(res => {
      this.employee = res;
    });
  }

  onSaveOrUpdate() {
    console.log(this.payslipForm.value);
  }

}
