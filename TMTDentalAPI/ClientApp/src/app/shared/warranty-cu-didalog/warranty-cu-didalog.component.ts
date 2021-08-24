import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import * as _ from 'lodash';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { EmployeePaged } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';

@Component({
  selector: 'app-warranty-cu-didalog',
  templateUrl: './warranty-cu-didalog.component.html',
  styleUrls: ['./warranty-cu-didalog.component.css']
})
export class WarrantyCuDidalogComponent implements OnInit {

  @ViewChild('doctorCbx', { static: true }) doctorCbx: ComboBoxComponent;

  title: string = "Tạo phiếu bảo hành";
  myForm: FormGroup;
  id: string;
  filteredDoctors: any = [];

  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private modalService: NgbModal,
    private notificationService: NotificationService,
    private intlService: IntlService,
    private employeeService: EmployeeService,
  ) { }

  ngOnInit() {
    this.myForm = this.fb.group({
      state: 'draft',
      doctor: [null, Validators.required],
      dateReceiptObj: [null, Validators.required],
    });

    this.doctorCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.doctorCbx.loading = true)),
      switchMap(value => this.searchDoctors(value))
    ).subscribe(result => {
      this.filteredDoctors = result;
      this.doctorCbx.loading = false;
    });
    this.loadDoctors();
  }

  getControlFC(key: string) {
    return this.myForm.get(key);
  }

  getValueFC(key: string) {
    return this.myForm.get(key).value;
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

  searchDoctors(filter?: string) {
    var val = new EmployeePaged();
    val.isDoctor = true;
    val.search = filter;
    return this.employeeService.getEmployeeSimpleList(val);
  }

  loadDoctors() {
    this.searchDoctors().subscribe(result => {
      this.filteredDoctors = _.unionBy(this.filteredDoctors, result, 'id');
    });
  }

  onSave() {
    if (this.myForm.invalid) {
      return;
    }

    // Your code is here
  }

  onConfirm() {
    if (this.myForm.invalid) {
      return;
    }

    // Your code is here
  }

  onCancel() {

  }

  onClose() {
    if (this.id) {
      this.activeModal.close(true);
    } else {
      this.activeModal.close();
    }
  }
}
