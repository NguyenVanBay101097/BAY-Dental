import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import * as _ from 'lodash';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { EmployeePaged, EmployeeSimple } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { ProductRequestService } from 'src/app/shared/product-request.service';
import { ProductRequestDefaultGet, ProductRequestDisplay } from '../product-request';

@Component({
  selector: 'app-sale-order-product-request-dialog',
  templateUrl: './sale-order-product-request-dialog.component.html',
  styleUrls: ['./sale-order-product-request-dialog.component.css']
})
export class SaleOrderProductRequestDialogComponent implements OnInit {
  title: string = null;
  id: string;
  saleOrderId: string;
  formGroup: FormGroup;
  productRequestDisplay: ProductRequestDisplay = new ProductRequestDisplay();
  
  @ViewChild('employeeCbx', { static: true }) employeeCbx: ComboBoxComponent;
  filteredEmployees: EmployeeSimple[] = [];

  reload = false;

  get f() { return this.formGroup.controls; }

  get seeForm() {
    var state = this.formGroup.get('state').value;
    var val = state == 'confirmed' || state == 'done';
    return val;
  }

  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private productRequestService: ProductRequestService,
    private employeeService: EmployeeService,
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      dateObj: [null, Validators.required],
      user: [null, Validators.required],
      employee: [null, Validators.required],
      state: null,
      lines: this.fb.array([])
    });
    setTimeout(() => {
      this.loadEmployees();

      if (!this.id) {
        this.loadDefault();
      } else {
        this.loadRecord();
      }
    });
    this.employeeCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.employeeCbx.loading = true)),
      switchMap(value => this.searchEmployees(value))
    ).subscribe(result => {
      this.filteredEmployees = result;
      this.employeeCbx.loading = false;
    });
  }

  loadEmployees() {
    this.searchEmployees().subscribe(result => {
      this.filteredEmployees = _.unionBy(this.filteredEmployees, result, 'id');
    });
  }

  searchEmployees(filter?: string) {
    var val = new EmployeePaged();
    val.search = filter || '';
    return this.employeeService.getEmployeeSimpleList(val);
  }

  loadDefault() {
    var val = new ProductRequestDefaultGet();
    val.saleOrderId = this.saleOrderId;
    this.productRequestService.defaultGet(val).subscribe((res: any) => {
      this.productRequestDisplay = res;
      this.formGroup.patchValue(res);
      var date = new Date(res.date);
      this.formGroup.get('dateObj').setValue(date);
      console.log(res);
    });
  }

  loadRecord() {
    this.productRequestService.get(this.id).subscribe((res: any) => {
      this.productRequestDisplay = res;
      this.formGroup.patchValue(res);
      var date = new Date(res.date);
      this.formGroup.get('dateObj').setValue(date);
    });
  }

  getValueForm(key) {
    return this.formGroup.get(key).value;
  }

  onSave() {

  }

  onRequest() {

  }

  onCancel() {

  }

  onClose() {
    if (this.reload) {
      this.activeModal.close();
    } else {
      this.activeModal.dismiss();
    }
  }
}
