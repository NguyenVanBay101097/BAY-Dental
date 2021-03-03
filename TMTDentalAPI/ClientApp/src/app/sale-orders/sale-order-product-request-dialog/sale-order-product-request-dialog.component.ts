import { Component, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { NotificationService } from '@progress/kendo-angular-notification';
import * as _ from 'lodash';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { SaleOrderLineForProductRequest } from 'src/app/core/services/sale-order-line.service';
import { SaleOrderService } from 'src/app/core/services/sale-order.service';
import { EmployeePaged, EmployeeSimple } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
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
  listProductBoms: SaleOrderLineForProductRequest[] = [];
  
  @ViewChild('employeeCbx', { static: true }) employeeCbx: ComboBoxComponent;
  filteredEmployees: EmployeeSimple[] = [];

  reload = false;

  get f() { return this.formGroup.controls; }

  get lines() { return this.formGroup.get('lines') as FormArray; }

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
    private modalService: NgbModal,
    private notificationService: NotificationService,
    private saleOrderService: SaleOrderService,
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
      this.loadListProductBoms();
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
      this.loadToFormArray(res.lines);
    });
  }

  loadRecord() {
    this.productRequestService.get(this.id).subscribe((res: any) => {
      this.productRequestDisplay = res;
      this.formGroup.patchValue(res);
      var date = new Date(res.date);
      this.formGroup.get('dateObj').setValue(date);
      this.loadToFormArray(res.lines);
    });
  }
  
  loadToFormArray(lines) {
    let control = this.formGroup.get('lines') as FormArray;
    control = this.fb.array([]); //reset form array
    lines.forEach(line => {
      control.push(this.fb.group(line));
    });
  }

  loadListProductBoms() {
    this.saleOrderService.getLineForProductRequest(this.saleOrderId).subscribe((res: any) => {
      this.listProductBoms = res;
      console.log(res);
    });
  }

  getValueForm(key) {
    return this.formGroup.get(key).value;
  }

  onSave() {

  }

  onConfirmed() {
    this.productRequestService.actionConfirm([this.id]).subscribe((result) => {
      this.notificationService.show({
        content: "Gửi yêu cầu thành công đến bộ phận Kho",
        hideAfter: 3000,
        position: { horizontal: "center", vertical: "top" },
        animation: { type: "fade", duration: 400 },
        type: { style: "success", icon: true },
      });
      this.activeModal.close();
    }, (error) => {
    });
  }

  onCancel() {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = "Hủy yêu cầu vật tư";
    modalRef.componentInstance.body = "Bạn có chắc chắn hủy yêu cầu vật tư?";
    modalRef.result.then((result) => {
      this.productRequestService.actionCancel([this.id]).subscribe((result) => {
        this.notificationService.show({
          content: "Hủy thành công",
          hideAfter: 3000,
          position: { horizontal: "center", vertical: "top" },
          animation: { type: "fade", duration: 400 },
          type: { style: "success", icon: true },
        });
        this.activeModal.close();
      }, (error) => {
      });
    }, (error) => {
    });
  }

  onClose() {
    if (this.reload) {
      this.activeModal.close();
    } else {
      this.activeModal.dismiss();
    }
  }
}
