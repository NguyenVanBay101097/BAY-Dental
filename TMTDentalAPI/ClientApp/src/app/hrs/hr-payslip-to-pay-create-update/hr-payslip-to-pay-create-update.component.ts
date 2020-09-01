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
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { EmployeeCreateUpdateComponent } from 'src/app/employees/employee-create-update/employee-create-update.component';
import { HrPayrollStructureService, HrPayrollStructurePaged } from '../hr-payroll-structure.service';
import { validator } from 'fast-json-patch';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-hr-payslip-to-pay-create-update',
  templateUrl: './hr-payslip-to-pay-create-update.component.html',
  styleUrls: ['./hr-payslip-to-pay-create-update.component.css']
})
export class HrPayslipToPayCreateUpdateComponent implements OnInit {

  @ViewChild('empCbx', { static: true }) empCbx: ComboBoxComponent;
  @ViewChild('structCbx', { static: true }) structCbx: ComboBoxComponent;

  date = new Date();
  payslipForm: FormGroup;
  listEmployees: any[];
  id: any;
  IsShowLines = false;
  listStructs: any[];
  payslip: any;
  listWorkDays: any;
  listLines: any;


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
    this.payslip = {};
    this.payslipForm = this.fb.group({
      struct: [null, [Validators.required]],
      employee: [null, [Validators.required]],
      dateFrom: [null, [Validators.required]],
      dateTo: [null, [Validators.required]],
      name: [null, [Validators.required]],
      state: 'draft',
      number: null,
      listHrPayslipWorkedDaySave: [[]],
      companyId: [null, Validators.required],
      structureType: null
    });

    this.empCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.empCbx.loading = true)),
      switchMap(value => this.SearchEmployee(value))
    ).subscribe(result => {
      this.listEmployees = result.items;
      this.empCbx.loading = false;
    });

    this.structCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.structCbx.loading = true)),
      switchMap(value => this.searchStructs(value))
    ).subscribe(result => {
      this.listStructs = result.items;
      this.structCbx.loading = false;
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
  get number() { return this.payslipForm.get('number'); }
  get employee() { return this.payslipForm.get('employee'); }
  get WorkedDay() { return this.payslipForm.get('listHrPayslipWorkedDaySave'); }
  get structureType() { return this.payslipForm.get('structureType'); }

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
      this.payslipForm.patchValue(res);
      this.LoadStructList();
      this.loadWordDayFromApi();
      this.loadLineDataFromApi();
      this.payslip = res;
    });
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

  EmployeeValueChange(isEmployeeChange = true) {
    let val = {
      employeeId: this.employee.value ? this.employee.value.id : null,
      dateFrom: this.dateFrom.value ? this.intlService.formatDate(this.dateFrom.value, 'yyyy-MM-ddTHH:mm:ss') : null,
      dateTo: this.dateTo.value ? this.intlService.formatDate(this.dateTo.value, 'yyyy-MM-ddTHH:mm:ss') : null,
    };

    this.hrPayslipService.onChangeEmployee(val).subscribe((res: any) => {
      this.WorkedDay.setValue(res.workedDayLines);
      this.name.setValue(res.name);
      this.structureType.setValue(res.structureType);

      if (isEmployeeChange) {
        this.struct.setValue(null);
      }
      this.LoadStructList();
    });
  }

  LoadStructList(filter?: string) {
    this.searchStructs(filter).subscribe(res => {
      this.listStructs = res.items;
    });
  }

  searchStructs(filter?: string) {
    const val = new HrPayrollStructurePaged();
    val.structureTypeId = this.structureType.value ? this.structureType.value.id : '';
    val.limit = 20;
    val.offset = 0;
    val.filter = filter || '';

    return this.hrPayrollStructureService.getPaged(val);
  }

  SelectStructChange(e) {
    if (e) {
      this.structId.setValue(e.id);
      this.struct.setValue(e);
    }
  }

  onSaveOrUpdate() {
    if (!this.ValidateForm()) { return false; }
    var val = this.getDataToSave();

    if (!this.id) {
      this.hrPayslipService.create(val).subscribe(res => {
        this.router.navigate(['/hr/payslips/edit/' + res.id]);
      });
    } else {
      this.hrPayslipService.update(this.id, val).subscribe(res => {
        this.notificationService.show({
          content: 'Lưu thành công!',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
      });
    }
  }

  getDataToSave() {
    const val = this.payslipForm.value;
    val.dateFrom = this.intlService.formatDate(val.dateFrom, 'yyyy-MM-ddTHH:mm:ss');
    val.dateTo = this.intlService.formatDate(val.dateTo, 'yyyy-MM-ddTHH:mm:ss');
    val.workedDaysLines = this.payslipForm.get('listHrPayslipWorkedDaySave').value;
    val.employeeId = this.employee.value ? this.employee.value.id : null;
    val.structId = this.struct.value ? this.struct.value.id : null;
    val.structureTypeId = this.structureType.value ? this.structureType.value.id : null;
    return val;
  }

  ComputeSalary() {
    if (!this.ValidateForm()) { return false; }
    var val = this.getDataToSave();

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
          this.LoadRecord();
          this.loadLineDataFromApi();
        });
      });
    }
  }

  ConfirmSalary() {
    this.hrPayslipService.actionDone([this.id]).subscribe(res => {
      this.LoadRecord();
    });
  }

  actionCancel() {
    const modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Hủy phiếu lương';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn hủy?';
    modalRef.result.then(() => {
      this.hrPayslipService.actionCancel([this.id]).subscribe(res => {
        this.LoadRecord();
      });
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
        this.EmployeeValueChange();
      });
    });
  }

  loadWordDayFromApi() {
    if (this.employee.value && this.employee.value.structureTypeId) {
      this.hrPayslipService.getWorkedDaysLines(this.id).subscribe((res: any) => {
        this.listWorkDays = res;
        this.WorkedDay.setValue(res);
      });

    } else {
      this.listWorkDays = [];
    }
  }

  loadLineDataFromApi() {
    if (this.id) {
      this.hrPayslipService.getLines(this.id).subscribe((res: any) => {
        this.listLines = res;
      });
    }
  }
}
