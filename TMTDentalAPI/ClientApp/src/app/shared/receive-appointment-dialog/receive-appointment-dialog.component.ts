
import { CustomerReceiptDisplay } from './../../customer-receipt/customer-receipt.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent, MultiSelectComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import * as _ from 'lodash';
import { debounceTime, mergeMap, switchMap, tap } from 'rxjs/operators';
import { CustomerReceiptBasic, CustomerReceiptService } from 'src/app/customer-receipt/customer-receipt.service';
import { EmployeeBasic, EmployeePaged } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { ProductSimple } from 'src/app/products/product-simple';
import { ProductPaged, ProductService } from 'src/app/products/product.service';
import { AppSharedShowErrorService } from '../shared-show-error.service';
import { ReceiveAppointmentService } from 'src/app/customer-receipt/receive-appointment.service';

@Component({
  selector: 'app-receive-appointment-dialog',
  templateUrl: './receive-appointment-dialog.component.html',
  styleUrls: ['./receive-appointment-dialog.component.css']
})
export class ReceiveAppointmentDialogComponent implements OnInit {

  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;
  @ViewChild('doctorCbx', { static: true }) doctorCbx: ComboBoxComponent;
  @ViewChild('serviceMultiSelect', { static: true }) serviceMultiSelect: MultiSelectComponent

  filteredServices: ProductSimple[] = [];
  filteredEmployees: EmployeeBasic[] = [];
  appointId: string;
  title: string = 'Tiếp nhận';

  formGroup: FormGroup;
  receiveAppointmentDisplay: any;

  submitted = false;

  constructor(
    private fb: FormBuilder,
    private intlService: IntlService,
    public activeModal: NgbActiveModal,
    private errorService: AppSharedShowErrorService,
    private productService: ProductService,
    private employeeService: EmployeeService,
    private notificationService: NotificationService,
    private receiveAppointmentService: ReceiveAppointmentService,
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      dateObj: [new Date(this.receiveAppointmentDisplay.dateWaiting), Validators.required],
      note: this.receiveAppointmentDisplay.note,
      doctor: [this.receiveAppointmentDisplay.doctor, Validators.required],
      timeExpected: this.receiveAppointmentDisplay.timeExpected,
      services: [this.receiveAppointmentDisplay.products],
      isRepeatCustomer: this.receiveAppointmentDisplay.isRepeatCustomer,
    })

    setTimeout(() => {
      this.loadEmployees();
      this.filterChangeCombobox();
      this.filterChangeMultiselect();
      this.loadService();
    });
  }

  get f() { return this.formGroup.controls; }

  filterChangeMultiselect() {
    this.serviceMultiSelect.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.serviceMultiSelect.loading = true)),
        switchMap((value) => this.searchService(value))
      )
      .subscribe((result) => {
        this.filteredServices = result;
        this.serviceMultiSelect.loading = false;
      });
  }

  loadEmployees() {
    this.searchEmployees().subscribe(result => {
      this.filteredEmployees = _.unionBy(this.filteredEmployees, result.items, 'id');
    });
  }

  loadService() {
    this.searchService().subscribe(
      result => {
        this.filteredServices = result;
      }
    )
  }

  searchService(q?: string) {
    var val = new ProductPaged();
    val.limit = 20;
    val.offset = 0
    val.search = q || '';
    val.type = "service";
    val.type2 = "service";
    return this.productService.autocomplete2(val);
  }

  searchEmployees(filter?: string) {
    var val = new EmployeePaged();
    val.search = filter || '';
    val.isDoctor = true;
    return this.employeeService.getEmployeePaged(val);
  }

  onSave() {
    this.submitted = true;

    if (!this.formGroup.valid) {
      return false;
    }

    var receipt = this.formGroup.value;
    receipt.partnerId = this.receiveAppointmentDisplay.partner.id;
    receipt.doctorId = receipt.doctor ? receipt.doctor.id : '';
    receipt.dateWaiting = this.intlService.formatDate(receipt.dateObj, 'yyyy-MM-ddTHH:mm:ss');
    receipt.timeExpected = receipt.timeExpected || 0;
    receipt.productIds = receipt.services ? receipt.services.map(x => x.id) : [];
    receipt.appointmentId = this.receiveAppointmentDisplay.appointmentId;
    this.receiveAppointmentService.actionSave(receipt).subscribe(
      (res) => {
        this.activeModal.close(res);
      },
      er => {
        this.errorService.show(er);
        this.submitted = false;
      },
    )
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

  filterChangeCombobox() {
    this.doctorCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => this.doctorCbx.loading = true),
      switchMap(val => this.searchEmployees(val.toString().toLowerCase()))
    ).subscribe(
      rs => {
        this.filteredEmployees = rs.items;
        this.doctorCbx.loading = false;
      }
    )
  }

}

