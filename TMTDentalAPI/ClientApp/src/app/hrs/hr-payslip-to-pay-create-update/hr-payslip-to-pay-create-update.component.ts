import { Component, OnInit, ViewChild } from '@angular/core';
import { HrPayslipDisplay, HrPayslipService } from '../hr-payslip.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { NotificationService } from '@progress/kendo-angular-notification';
import { EmployeeDisplay, EmployeePaged, EmployeeBasic } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { IntlService } from '@progress/kendo-angular-intl';
import { HrPayslipLineListComponent } from '../hr-payslip-line-list/hr-payslip-line-list.component';
import { ToaThuocLinesSaveCuFormComponent } from 'src/app/toa-thuocs/toa-thuoc-lines-save-cu-form/toa-thuoc-lines-save-cu-form.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { EmployeeCreateUpdateComponent } from 'src/app/employees/employee-create-update/employee-create-update.component';
import { HrPayrollStructureService, HrPayrollStructurePaged } from '../hr-payroll-structure.service';
import { validator } from 'fast-json-patch';

@Component({
  selector: 'app-hr-payslip-to-pay-create-update',
  templateUrl: './hr-payslip-to-pay-create-update.component.html',
  styleUrls: ['./hr-payslip-to-pay-create-update.component.css']
})
export class HrPayslipToPayCreateUpdateComponent implements OnInit {

  @ViewChild('empCbx', { static: true }) empCbx: ComboBoxComponent;
  @ViewChild(HrPayslipLineListComponent, { static: false }) hrPayslipLineListComponent: HrPayslipLineListComponent;

  date = new Date();
  payslipRecord: HrPayslipDisplay;
  payslipForm: FormGroup;
  listEmployees: any[];
  id: any;
  IsShowLines = false;
  listStructs: any[];

  constructor(
    private fb: FormBuilder, private router: Router,
    private hrPayslipService: HrPayslipService,
    private employeeService: EmployeeService,
    private hrPayrollStructureService: HrPayrollStructureService,
    private intlService: IntlService,
    private notificationService: NotificationService,
    private modalService: NgbModal,
    private activeroute: ActivatedRoute
  ) { }

  ngOnInit() {
    this.payslipForm = this.fb.group({
      structId: [null, [Validators.required]],
      struct: [null, [Validators.required]],
      employeeId: [null, [Validators.required]],
      employee: [null, [Validators.required]],
      dateFrom: [null, [Validators.required]],
      dateTo: [null, [Validators.required]],
      name: ['lương tháng ' + (this.date.getMonth() + 1), [Validators.required]],
      state: 'draft',
      number: null,
      listHrPayslipWorkedDaySave: null,
      companyId: [null, Validators.required]
    });

    this.empCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.empCbx.loading = true)),
      switchMap(value => this.SearchEmployee(value))
    ).subscribe(result => {
      this.listEmployees = result.items;
      this.empCbx.loading = false;
    });

    this.GetEmployeePaged();
    this.id = this.activeroute.snapshot.paramMap.get('id');
    if (this.id) {
      this.LoadRecord();
    } else {
      this.getDefault();
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
  get employee() { return this.payslipForm.get('employee'); }

  getDefault() {
    const val = new Object();
    this.hrPayslipService.defaultget(val).subscribe((res: any) => {
      this.payslipForm.get('companyId').setValue(res.companyId);
      this.dateFrom.setValue(new Date(res.dateFrom));
      this.dateTo.setValue(new Date(res.dateTo));
    });
  }

  LoadRecord() {
    this.hrPayslipService.get(this.id).subscribe((res: any) => {
      res.dateFrom = new Date(res.dateFrom);
      res.dateTo = new Date(res.dateTo);
      this.payslipRecord = Object.assign({}, res);
      this.payslipForm.patchValue(res);
      this.LoadStructList(res.employee.structureTypeId, null);
      this.DisableFormControl();
      this.hrPayslipLineListComponent.loadWordDayFromApi();
      this.hrPayslipLineListComponent.tongtien = res.totalAmount;
    });
  }

  DisableFormControl() {
    if (this.state.value === 'done') { this.Form.disable(); }
    if (this.state.value === 'verify') {
      this.employee.disable();
      this.struct.disable();
    }
    if (this.state.value === 'draft') { this.Form.enable(); }
  }

  SearchEmployee(search?: string) {
    const val = new EmployeePaged();
    val.search = search ? search : '';
    return this.employeeService.getEmployeePaged(val);
  }

  GetEmployeePaged() {
    this.SearchEmployee().subscribe(res => {
      this.listEmployees = res.items;
    });
  }

  EmployeeFilter(value) {
    const val = new EmployeePaged();
    val.search = value;
    return this.employeeService.getEmployeePaged(val).subscribe((result: any) => {
      this.listEmployees = result;
    });
  }

  EmployeeChange(e) {
    if (!e) {
      this.listStructs = null;
      this.employeeId.setValue(null);
      this.struct.setValue(null);
      this.structId.setValue(null);
    } else {
      this.employeeId.setValue(e.id);

      this.name.setValue('Lương tháng ' + (this.dateFrom.value ? (this.dateFrom.value.getMonth() + 1) : '') + ' của ' + e.name);
      if (e.structureTypeId) {
        this.LoadStructList(e.structureTypeId, null);
      } else {
        this.listStructs = null;
        this.struct.setValue(null);
        this.structId.setValue(null);
      }
    }
  }

  EmployeeValueChange(e) {
    this.hrPayslipLineListComponent.onEmployeeChange();
  }

  LoadStructList(typeId, filter) {
    const val = new HrPayrollStructurePaged();
    val.structureTypeId = typeId;
    val.limit = 20;
    val.offset = 0;
    if (filter != null) {
      val.filter = filter;
    }
    this.hrPayrollStructureService.getPaged(val).subscribe(res => {
      this.listStructs = res.items;
    });
  }

  handleFilterStruct(value) {
    this.LoadStructList(this.employee.value.structureTypeId, value);
  }

  SelectStructChange(e) {
    if (e) {
      this.structId.setValue(e.id);
      this.struct.setValue(e);
    }
  }

  ChangeDateFrom(e) {
    if (this.dateFrom.value > this.dateTo.value) {
      this.notificationService.show({
        content: ' thời gian bắt đầu phải bé hơn thời gian kết thúc!',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });
      return false;
    }
    this.name.setValue('Lương tháng ' + (e.getMonth() + 1) + (this.employee.value ? ' của ' + this.employee.value.name : ''));
    this.hrPayslipLineListComponent.onEmployeeChange();
  }

  onSaveOrUpdate() {
    if (!this.ValidateForm()) { return false; }
    const val = this.payslipForm.value;
    val.dateFrom = this.intlService.formatDate(val.dateFrom, 'g', 'en-US');
    val.dateTo = this.intlService.formatDate(val.dateTo, 'g', 'en-US');
    val.workedDaysLines = this.payslipForm.get('listHrPayslipWorkedDaySave').value;
    if (!this.id) {
      this.hrPayslipService.create(val).subscribe(res => {
        this.router.navigate(['/hr/payslips/edit/' + res.id]);
      });
    } else {
      this.hrPayslipService.update(this.id, val).subscribe(res => {
        this.DisableFormControl();
        this.notificationService.show({
          content: ' thành công!',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
      });
    }

  }

  ComputeSalaryAgain() {
    if (!this.ValidateForm()) { return false; }
    const val = this.payslipForm.value;
    val.dateFrom = this.intlService.formatDate(val.dateFrom, 'g', 'en-US');
    val.dateTo = this.intlService.formatDate(val.dateTo, 'g', 'en-US');
    val.workedDaysLines = this.payslipForm.get('listHrPayslipWorkedDaySave').value;

    if (this.id) {
      this.hrPayslipService.update(this.id, val).subscribe(res => {
        this.ComputeSalary();
        this.notificationService.show({
          content: ' thành công!',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
      });
    }
  }

  ComputeSalary() {
    if (!this.ValidateForm()) { return false; }

    const val = this.payslipForm.value;
    val.dateFrom = this.intlService.formatDate(val.dateFrom, 'g', 'en-US');
    val.dateTo = this.intlService.formatDate(val.dateTo, 'g', 'en-US');
    val.state = 'verify';
    val.workedDaysLines = this.payslipForm.get('listHrPayslipWorkedDaySave').value;

    if (!this.id) {
      this.hrPayslipService.create(val).subscribe(res => {
        this.hrPayslipService.computeSheet([res.id]).subscribe((res2: any) => {
          this.router.navigate(['/hr/payslips/edit/' + res.id]);
        });
      });
    } else {
      this.hrPayslipService.update(this.id, val).subscribe(res1 => {
        this.hrPayslipService.computeSheet([this.id]).subscribe((res: any) => {
          this.state.setValue('verify');
          this.DisableFormControl();
          this.LoadRecord();
          this.hrPayslipLineListComponent.loadLineDataFromApi();
        });
      });
    }
  }

  ConfirmSalary() {
    this.hrPayslipService.confirmCompute([this.id]).subscribe(res => {
      this.notificationService.show({
        content: ' thành công!',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });

      this.state.setValue('done');
      this.DisableFormControl();
    });
  }

  CancelCompute() {
    this.hrPayslipService.cancelCompute(this.id).subscribe(res => {
      this.notificationService.show({
        content: ' thành công!',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });

      this.state.setValue('draft');
      this.DisableFormControl();
      this.LoadRecord();
      this.hrPayslipLineListComponent.loadLineDataFromApi();
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

    if (this.dateFrom.value > this.dateTo.value) {
      this.notificationService.show({
        content: ' thời gian bắt đầu phải bé hơn thời gian kết thúc!',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });
      return false;
    }
    return true;
  }

  editEmployee() {
    const modalRef = this.modalService.open(EmployeeCreateUpdateComponent, { scrollable: true, size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa nhân viên';
    modalRef.componentInstance.empId = this.employee.value.id;
    modalRef.result.then((res) => {
      this.employeeService.getEmployee(this.employee.value.id).subscribe((emp: any) => {
        this.employee.setValue(emp);
        this.EmployeeChange(emp);
        this.hrPayslipLineListComponent.onEmployeeChange();

      });
    });
  }
}
