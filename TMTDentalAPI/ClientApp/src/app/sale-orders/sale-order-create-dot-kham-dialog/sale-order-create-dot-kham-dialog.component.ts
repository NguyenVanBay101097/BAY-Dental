import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { DotKhamService } from 'src/app/dot-khams/dot-kham.service';
import { IntlService } from '@progress/kendo-angular-intl';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { EmployeePaged, EmployeeSimple } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { DotKhamDefaultGet } from 'src/app/dot-khams/dot-khams';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { EmployeeCreateUpdateComponent } from 'src/app/employees/employee-create-update/employee-create-update.component';
import * as _ from 'lodash';

@Component({
  selector: 'app-sale-order-create-dot-kham-dialog',
  templateUrl: './sale-order-create-dot-kham-dialog.component.html',
  styleUrls: ['./sale-order-create-dot-kham-dialog.component.css']
})

export class SaleOrderCreateDotKhamDialogComponent implements OnInit {
  dotKhamForm: FormGroup;
  saleOrderId: string;
  filteredDoctors: EmployeeSimple[];
  filteredAssistants: EmployeeSimple[];
  @ViewChild('doctorCbx', { static: true }) doctorCbx: ComboBoxComponent;
  @ViewChild('assistantCbx', { static: true }) assistantCbx: ComboBoxComponent;
  title: string;

  constructor(private fb: FormBuilder, private dotKhamService: DotKhamService, private intlService: IntlService,
    private employeeService: EmployeeService, public activeModal: NgbActiveModal, private modalService: NgbModal) { }

  ngOnInit() {
    this.dotKhamForm = this.fb.group({
      doctor: [null, Validators.required],
      assistant: null,
      dateObj: [null, Validators.required],
      note: null,
      companyId: null,
      userId: null,
      saleOrderId: null,
      partnerId: null
    });

    setTimeout(() => {
      this.doctorCbx.focus();
    }, 200);


    setTimeout(() => {
      this.getDoctorList();
      this.getAssistantList();

      this.doctorCbx.filterChange.asObservable().pipe(
        debounceTime(300),
        tap(() => (this.doctorCbx.loading = true)),
        switchMap(value => this.searchDoctors(value))
      ).subscribe(result => {
        this.filteredDoctors = result;
        this.doctorCbx.loading = false;
      });

      this.assistantCbx.filterChange.asObservable().pipe(
        debounceTime(300),
        tap(() => (this.assistantCbx.loading = true)),
        switchMap(value => this.searchAssistants(value))
      ).subscribe(result => {
        this.filteredAssistants = result;
        this.assistantCbx.loading = false;
      });

      this.getDefault();
    });
  }

  getDefault() {
    var val = new DotKhamDefaultGet();
    val.saleOrderId = this.saleOrderId;
    this.dotKhamService.defaultGet(val).subscribe(result => {
      this.dotKhamForm.patchValue(result);
      let date = new Date(result.date);
      this.dotKhamForm.get('dateObj').patchValue(date);
    });
  }

  createDoctorDialog() {
    let modalRef = this.modalService.open(EmployeeCreateUpdateComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm bác sĩ';
    modalRef.componentInstance.isDoctor = true;

    modalRef.result.then(result => {
      var p = new EmployeeSimple();
      p.id = result.id;
      p.name = result.name;
      this.dotKhamForm.get('doctor').patchValue(p);
      this.filteredDoctors = _.unionBy(this.filteredDoctors, [p], 'id');
    }, () => {
    });
  }

  createAssistantDialog() {
    let modalRef = this.modalService.open(EmployeeCreateUpdateComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm phụ tá';
    modalRef.componentInstance.isAssistant = true;

    modalRef.result.then(result => {
      var p = new EmployeeSimple();
      p.id = result.id;
      p.name = result.name;
      this.dotKhamForm.get('assistant').patchValue(p);
      this.filteredAssistants = _.unionBy(this.filteredAssistants, [p], 'id');
    }, () => {
    });
  }

  searchEmployees(filter: string, position: string) {
    var val = new EmployeePaged();
    val.search = filter.toLowerCase();
    val.position = position
    return this.employeeService.getEmployeeSimpleList(val);
  }

  searchDoctors(q?: string) {
    var val = new EmployeePaged();
    val.search = q;
    val.isDoctor = true;
    return this.employeeService.getEmployeeSimpleList(val);
  }

  searchAssistants(q?: string) {
    var val = new EmployeePaged();
    val.search = q;
    val.isAssistant = true;
    return this.employeeService.getEmployeeSimpleList(val);
  }

  onSave() {
    if (!this.dotKhamForm.valid) {
      return;
    }

    var val = this.dotKhamForm.value;
    val.doctorId = val.doctor ? val.doctor.id : null;
    val.assistantId = val.assistant ? val.assistant.id : null;
    this.dotKhamService.create(val).subscribe(result => {
      this.dotKhamService.actionConfirm(result.id).subscribe(() => {
        this.activeModal.close({
          view: false
        });
      });
    });
  }

  onSaveAndView() {
    if (!this.dotKhamForm.valid) {
      return;
    }

    var val = this.dotKhamForm.value;
    val.doctorId = val.doctor ? val.doctor.id : null;
    val.assistantId = val.assistant ? val.assistant.id : null;
    this.dotKhamService.create(val).subscribe(result => {
      this.dotKhamService.actionConfirm(result.id).subscribe(() => {
        this.activeModal.close({
          view: true,
          result
        });
      });
    });
  }

  onCancel() {
    this.activeModal.dismiss();
  }

  getDoctorList() {
    this.searchDoctors().subscribe(
      rs => {
        this.filteredDoctors = _.unionBy(this.filteredDoctors, rs, 'id');
      });
  }

  getAssistantList() {
    this.searchAssistants().subscribe(
      rs => {
        this.filteredAssistants = _.unionBy(this.filteredAssistants, rs, 'id');
      });
  }
}

