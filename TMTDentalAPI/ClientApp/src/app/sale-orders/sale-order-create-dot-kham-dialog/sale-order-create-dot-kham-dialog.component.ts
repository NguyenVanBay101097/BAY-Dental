import { UserService, UserPaged } from './../../users/user.service';
import { UserSimple } from './../../users/user-simple';
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
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';
import { UserCuDialogComponent } from 'src/app/users/user-cu-dialog/user-cu-dialog.component';

@Component({
  selector: 'app-sale-order-create-dot-kham-dialog',
  templateUrl: './sale-order-create-dot-kham-dialog.component.html',
  styleUrls: ['./sale-order-create-dot-kham-dialog.component.css']
})

export class SaleOrderCreateDotKhamDialogComponent implements OnInit {
  dotKhamForm: FormGroup;
  id: string;
  saleOrderId: string;
  filteredDoctors: EmployeeSimple[];
  filteredAssistants: EmployeeSimple[];
  filteredUsers: UserSimple[];
  assistantUserSimpleFilter: UserSimple[];
  @ViewChild('doctorCbx', { static: true }) doctorCbx: ComboBoxComponent;
  @ViewChild('assistantCbx', { static: true }) assistantCbx: ComboBoxComponent;
  @ViewChild('assistantUserCbx', { static: true }) assistantUserCbx: ComboBoxComponent;
  @ViewChild('userCbx', { static: true }) userCbx: ComboBoxComponent;
  title: string;

  constructor(
    private fb: FormBuilder,
    private dotKhamService: DotKhamService,
    private intlService: IntlService,
    private employeeService: EmployeeService,
    private userService: UserService,
    public activeModal: NgbActiveModal,
    private modalService: NgbModal,
    private errorService: AppSharedShowErrorService
  ) { }

  ngOnInit() {
    this.dotKhamForm = this.fb.group({
      dateObj: [null, Validators.required],
      note: null,
      companyId: null,
      doctor: [null, Validators.required],
      saleOrderId: null,
      partnerId: null,
      assistant: null
    });

    // setTimeout(() => {
    //   this.userCbx.focus();
    // }, 200);


    setTimeout(() => {
      this.getDoctorList();
      this.getAssistantList();
      // this.getUserList();

      this.doctorCbx.filterChange.asObservable().pipe(
        debounceTime(300),
        tap(() => (this.doctorCbx.loading = true)),
        switchMap(value => this.searchEmployees(value))
      ).subscribe(result => {
        this.filteredDoctors = result;
        this.doctorCbx.loading = false;
      });

      this.assistantCbx.filterChange.asObservable().pipe(
        debounceTime(300),
        tap(() => (this.assistantCbx.loading = true)),
        switchMap(value => this.searchEmployees(value))
      ).subscribe(result => {
        this.filteredAssistants = result;
        this.assistantCbx.loading = false;
      });
      if (this.id)
        this.loadData();
      else
        this.getDefault();
    });
  }

  loadData() {
    this.dotKhamService.get(this.id).subscribe(
      result => {
        this.dotKhamForm.patchValue(result);
        let date = new Date(result.date);
        this.dotKhamForm.get('dateObj').patchValue(date);
      }
    )
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
    let modalRef = this.modalService.open(EmployeeCreateUpdateComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
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
    let modalRef = this.modalService.open(EmployeeCreateUpdateComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
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

  createUserDialog() {
    let modalRef = this.modalService.open(UserCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm bác sĩ';
    // modalRef.componentInstance.isDoctor = true;

    modalRef.result.then(result => {
      var p = new UserSimple();
      p.id = result.id;
      p.name = result.name;
      this.dotKhamForm.get('user').patchValue(p);
      this.filteredUsers = _.unionBy(this.filteredUsers, [p], 'id');
    }, () => {
    });
  }

  searchEmployees(q?: string) {
    var val = new EmployeePaged();
    val.search = q;
    val.isDoctor = true;
    return this.employeeService.getEmployeeSimpleList(val);
  }

  searchUsers(q?: string) {
    var val = new UserPaged();
    val.search = q;
    return this.userService.autocompleteSimple(val);
  }

  onSave() {
    if (!this.dotKhamForm.valid) {
      return;
    }

    var val = this.dotKhamForm.value;
    val.doctorId = val.doctor ? val.doctor.id : null;
    val.assistantId = val.assistant ? val.assistant.id : null;
    val.date = this.intlService.formatDate(val.dateObj, 'yyyy-MM-ddTHH:mm:ss');
    this.dotKhamService.create(val).subscribe(result => {
      this.dotKhamService.actionConfirm(result.id).subscribe(() => {
        this.activeModal.close({
          view: false
        });
      }, err1 => {
        this.errorService.show(err1);
      });
    }, err2 => {
      this.errorService.show(err2);
    });
  }

  onSaveAndView() {
    if (!this.dotKhamForm.valid) {
      return;
    }

    var val = this.dotKhamForm.value;
    val.doctorId = val.doctor ? val.doctor.id : null;
    val.assistantId = val.assistant ? val.assistant.id : null;
    val.date = this.intlService.formatDate(val.dateObj, 'yyyy-MM-ddTHH:mm:ss');
    if (this.id) {
      this.dotKhamService.update(this.id, val).subscribe(
        () => {
          this.activeModal.close(val);
        }
      )
    } else {
      this.dotKhamService.create(val).subscribe(result => {
        console.log(result);
        this.dotKhamService.actionConfirm(result.id).subscribe(() => {
          this.activeModal.close({
            view: true,
            result
          });
        }, err1 => {
          this.errorService.show(err1);
        });
      }, err2 => {
        this.errorService.show(err2);
      });
    }
  }

  onCancel() {
    this.activeModal.dismiss();
  }

  getDoctorList() {
    this.searchEmployees().subscribe(
      rs => {
        this.filteredDoctors = _.unionBy(this.filteredDoctors, rs, 'id');
      });
  }

  getAssistantList() {
    this.searchEmployees().subscribe(
      rs => {
        this.filteredAssistants = _.unionBy(this.filteredAssistants, rs, 'id');
      });
  }

  getUserList() {
    this.searchUsers().subscribe(
      rs => {
        this.filteredUsers = _.unionBy(this.filteredUsers, rs, 'id');
        this.assistantUserSimpleFilter = _.unionBy(this.assistantUserSimpleFilter, rs, 'id');
      });
  }
}

