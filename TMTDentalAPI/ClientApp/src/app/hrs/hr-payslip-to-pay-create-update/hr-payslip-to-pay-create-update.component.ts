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
import { IfStmt } from '@angular/compiler';

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
      struct: null,
      employeeId: [null, [Validators.required]],
      employee: null,
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
    this.employeeService.getEmployeePaged(val).subscribe(res => {
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
    // if (!e) {
    //   this.listStructs = null;
    //   this.struct.setValue(null);
    //   this.structId.setValue(null);
    // } else {
    //   this.employeeId.setValue(e.id);
    //   this.employee = e;
    //   this.name.setValue('Lương tháng ' + (this.dateFrom.value ? (this.dateFrom.value.getMonth() + 1) : '') + ' của ' + e.name);
    //   if (e.structureTypeId) {
    //     this.LoadStructList(e.structureTypeId, null);
    //   } else {
    //     this.listStructs = null;
    //     this.struct.setValue(null);
    //     this.structId.setValue(null);
    //   }
    // }
    console.log('a');
    console.log(e);
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
    this.LoadStructList(this.employee.structureTypeId, value);
  }

  SelectStructChange(value) {
    this.structId.setValue(value.id);
    console.log(value);
  }

  ChangeDateFrom(e) {
    this.name.setValue('Lương tháng ' + (e.getMonth() + 1) + (this.employee ? ' của ' + this.employee.name : ''));
  }

  onSaveOrUpdate() {
    if (!this.ValidateForm()) { return false; }
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
    if (!this.ValidateForm()) { return false; }

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
    if (!this.ValidateForm()) { return false; }

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
    modalRef.componentInstance.empId = this.employee.id;
    modalRef.result.then(() => {
      // this.getEmployeesList();
    });
  }
}
