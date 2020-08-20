import { Component, OnInit, ViewChild } from '@angular/core';
import { HrPayslipDisplay, HrPayslipService } from '../hr-payslip.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { NotificationService } from '@progress/kendo-angular-notification';
import { EmployeeDisplay, EmployeePaged } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { IntlService } from '@progress/kendo-angular-intl';

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
  id: any;
  lines: any;

  constructor(
    private fb: FormBuilder, private router: Router,
    private hrPayslipService: HrPayslipService,
    private employeeService: EmployeeService,
    private intlService: IntlService,
    private notificationService: NotificationService,
    private activeroute: ActivatedRoute
  ) { }

  ngOnInit() {
    this.payslipForm = this.fb.group({
      structId: null,
      struct: null,
      employeeId: null,
      employee: [null, [Validators.required]],
      dateFrom: [null, [Validators.required]],
      dateTo: [null, [Validators.required]],
      name: [null, [Validators.required]],
      state: 'draft',
      number: null,
    });

    this.GetEmployeePaged();
    this.id = this.activeroute.snapshot.paramMap.get('id');
    if (this.id) {
      this.LoadRecord();
    }
  }
  get Form() { return this.payslipForm; }
  get state() { return this.payslipForm.get('state'); }
  get structId() { return this.payslipForm.get('structId'); }
  get struct() { return this.payslipForm.get('struct'); }
  get dateFrom() { return this.payslipForm.get('dateFrom'); }
  get dateTo() { return this.payslipForm.get('dateTo'); }

  LoadRecord() {
    this.hrPayslipService.get(this.id).subscribe((res: any) => {
      this.dateFrom.setValue(this.intlService.parseDate(res.dateFrom));
      this.dateTo.setValue(this.intlService.parseDate(res.dateTo));
      this.payslipRecord = Object.assign({}, res);
      this.employee = res.employee;
      this.employee.struct = res.struct;
      this.struct.setValue(res.struct);
      this.payslipForm.patchValue(res);

    });
  }

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
    this.employeeService.getEmployee(e.id).subscribe((res: any) => {
      this.employee = res;
      this.struct.setValue(res.struct);
    });
  }

  onSaveOrUpdate() {
    for (const i in this.payslipForm.controls) {
      this.payslipForm.controls[i].markAsDirty();
      this.payslipForm.controls[i].updateValueAndValidity();
    }

    if (!this.payslipForm.valid && this.payslipForm.enabled) {
      return false;
    }
    const val = this.payslipForm.value;
    val.employeeId = val.employee.id;
    val.structId = val.struct.id;
    val.dateFrom = this.intlService.formatDate(val.dateFrom, 'g', 'en-US');
    val.dateTo = this.intlService.formatDate(val.dateTo, 'g', 'en-US');
    this.hrPayslipService.create(val).subscribe(res => {
      this.router.navigate(['/hr/payslip-to-pay/edit/' + res.id]);
    });

  }

  ComputeSalary() {
    this.hrPayslipService.
  }

}
