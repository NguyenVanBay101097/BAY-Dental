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
import { HrPayslipLineListComponent } from '../hr-payslip-line-list/hr-payslip-line-list.component';
import { ToaThuocLinesSaveCuFormComponent } from 'src/app/toa-thuocs/toa-thuoc-lines-save-cu-form/toa-thuoc-lines-save-cu-form.component';

@Component({
  selector: 'app-hr-payslip-to-pay-create-update',
  templateUrl: './hr-payslip-to-pay-create-update.component.html',
  styleUrls: ['./hr-payslip-to-pay-create-update.component.css']
})
export class HrPayslipToPayCreateUpdateComponent implements OnInit {

  @ViewChild(HrPayslipLineListComponent, { static: false }) hrPayslipLineListComponent: HrPayslipLineListComponent;

  payslipRecord: HrPayslipDisplay;
  payslipForm: FormGroup;
  employee: any;
  listEmployees: any[];
  id: any;
  IsShowLines = false;

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
      structId: [null, [Validators.required]],
      struct: null,
      employeeId: [null, [Validators.required]],
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
  get employeeId() { return this.payslipForm.get('employeeId'); }
  get dateFrom() { return this.payslipForm.get('dateFrom'); }
  get dateTo() { return this.payslipForm.get('dateTo'); }
  get name() { return this.payslipForm.get('name'); }

  LoadRecord() {
    this.hrPayslipService.get(this.id).subscribe((res: any) => {
      res.dateFrom = new Date(res.dateFrom);
      res.dateTo = new Date(res.dateTo);
      this.payslipRecord = Object.assign({}, res);
      this.employee = res.employee;
      this.employee.struct = res.struct;
      this.payslipForm.patchValue(res);
      if (res.state === 'done') { this.Form.disable(); }
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
      this.structId.setValue(res.struct ? res.struct.id : null);
      this.employeeId.setValue(res.id);

      this.name.setValue('Lương tháng ' + (this.dateFrom.value ? (this.dateFrom.value.getMonth() + 1) : '') + ' của ' + res.name);
    });
  }

  ChangeDateFrom(e) {
    this.name.setValue('Lương tháng ' + (e.getMonth() + 1) + (this.employee ? ' của ' + this.employee.name : ''));
  }

  onSaveOrUpdate() {
    this.ValidateForm();
    const val = this.payslipForm.value;
    val.dateFrom = this.intlService.formatDate(val.dateFrom, 'g', 'en-US');
    val.dateTo = this.intlService.formatDate(val.dateTo, 'g', 'en-US');
    if (!this.id) {
      this.hrPayslipService.create(val).subscribe(res => {
        this.router.navigate(['/hr/payslip-to-pay/edit/' + res.id]);
      });
    } else {
      this.hrPayslipService.update(this.id, val).subscribe(res => {
        if (this.state.value === 'process') {
          this.hrPayslipService.ComputeLinePut(this.id).subscribe(res2 => {
            this.notificationService.show({
              content: ' thành công!',
              hideAfter: 3000,
              position: { horizontal: 'center', vertical: 'top' },
              animation: { type: 'fade', duration: 400 },
              type: { style: 'success', icon: true }
            });
            this.hrPayslipLineListComponent.loadDataFromApi();
          });
        } else {
          this.notificationService.show({
            content: ' thành công!',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
        }
      });
    }

  }

  ComputeSalary() {
    this.ValidateForm();

    if (!this.id) {
      const val = this.payslipForm.value;
      val.dateFrom = this.intlService.formatDate(val.dateFrom, 'g', 'en-US');
      val.dateTo = this.intlService.formatDate(val.dateTo, 'g', 'en-US');
      val.state = 'process';
      this.hrPayslipService.ComputeLinePost(val).subscribe((res: any) => {
        this.router.navigate(['/hr/payslip-to-pay/edit/' + res.id]);
      });
    } else {
      this.hrPayslipService.ComputeLinePut(this.id).subscribe((res: any) => {
        this.state.setValue('process');
      });
    }
  }

  ConfirmSalary() {
    this.hrPayslipService.ConfirmCompute(this.id).subscribe(res => {
      this.notificationService.show({
        content: ' thành công!',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });

      this.state.setValue('done');
    });
  }

  ComputeSalaryAgain() {
    this.ValidateForm();
    const val = this.payslipForm.value;
    val.dateFrom = this.intlService.formatDate(val.dateFrom, 'g', 'en-US');
    val.dateTo = this.intlService.formatDate(val.dateTo, 'g', 'en-US');

    this.hrPayslipService.update(this.id, val).subscribe(res => {
      this.hrPayslipService.ComputeLinePut(this.id).subscribe(res2 => {
        this.notificationService.show({
          content: ' thành công!',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.hrPayslipLineListComponent.loadDataFromApi();
      });
    });
  }

  CancelCompute() {
    this.hrPayslipService.CancelCompute(this.id).subscribe(res => {
      this.notificationService.show({
        content: ' thành công!',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });

      this.state.setValue('draft');
    });
  }

  ValidateForm() {

    for (const i in this.payslipForm.controls) {
      this.payslipForm.controls[i].markAsDirty();
      this.payslipForm.controls[i].updateValueAndValidity();
    }

    if (!this.payslipForm.valid && this.payslipForm.enabled) {
      return false;
    }
  }
}
