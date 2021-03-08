import { Component, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import * as _ from 'lodash';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { SaleOrderLineForProductRequest } from 'src/app/core/services/sale-order-line.service';
import { SaleOrderService } from 'src/app/core/services/sale-order.service';
import { EmployeePaged, EmployeeSimple } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { ProductRequestLineService } from 'src/app/shared/product-request-line.service';
import { ProductRequestService } from 'src/app/shared/product-request.service';
import { SaleOrderLineProductRequestedBasic, SaleOrderLineProductRequestedBasicCus, SaleOrderLineProductRequestedPaged, SaleOrderLineProductRequestedService } from 'src/app/shared/sale-order-line-product-requested.service';
import { GetLinePar, ProductRequestDefaultGet, ProductRequestDisplay } from '../product-request';

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
  submitted = false;
  productRequestDisplay: ProductRequestDisplay = new ProductRequestDisplay();
  listProductBoms: SaleOrderLineForProductRequest[] = [];
  listProductRequestedBoms: SaleOrderLineProductRequestedBasicCus[] = [];
  
  @ViewChild('employeeCbx', { static: true }) employeeCbx: ComboBoxComponent;
  filteredEmployees: EmployeeSimple[] = [];

  reload = false;

  get f() { return this.formGroup.controls; }

  get lines() { return this.formGroup.get('lines') as FormArray; }

  get seeForm() {
    var state = this.formGroup.get('state').value;
    if (state == null)
      return true;
    return state == 'confirmed' || state == 'done';
  }

  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private productRequestService: ProductRequestService,
    private employeeService: EmployeeService,
    private modalService: NgbModal,
    private notificationService: NotificationService,
    private saleOrderService: SaleOrderService,
    private productRequestLineService: ProductRequestLineService,
    private intlService: IntlService,
    private saleOrderLineProductRequestedService: SaleOrderLineProductRequestedService
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
    });
    this.employeeCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.employeeCbx.loading = true)),
      switchMap(value => this.searchEmployees(value))
    ).subscribe((res: any) => {
      this.filteredEmployees = res;
      this.employeeCbx.loading = false;
    });
  }

  loadEmployees() {
    this.searchEmployees().subscribe((res: any) => {
      this.filteredEmployees = _.unionBy(this.filteredEmployees, res, 'id');
    });
  }

  searchEmployees(filter?: string) {
    var val = new EmployeePaged();
    val.search = filter || '';
    val.isDoctor = true;
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
      this.loadLinesToFormArray(res.lines);
    });
  }

  loadRecord() {
    this.productRequestService.get(this.id).subscribe((res: any) => {
      this.productRequestDisplay = res;
      this.formGroup.patchValue(res);
      var date = new Date(res.date);
      this.formGroup.get('dateObj').setValue(date);
      this.loadLinesToFormArray(res.lines);
    });
  }
  
  loadLinesToFormArray(lines) {
    this.lines.clear();
    lines.forEach(line => {
      var index = this.findPro_listProductRequestedBoms(line.saleOrderLineId, line.productId);
      line.max = this.listProductRequestedBoms[index].max;
      var fg= this.fb.group(line);
      fg.get('productQty').setValidators([Validators.required]);
      fg.get('productQty').markAsTouched();
      this.lines.push(fg);
    });
  }

  loadLineToFormArray(line) {
    var index = this.lines.value.findIndex(item => (item.saleOrderLineId == line.saleOrderLineId && item.productId == line.productId));
    if (index < 0) {
      var i = this.findPro_listProductRequestedBoms(line.saleOrderLineId, line.productId);
      line.max = this.listProductRequestedBoms[i].max;
      var fg= this.fb.group(line);
      fg.get('productQty').setValidators([Validators.required]);
      fg.get('productQty').markAsTouched();
      this.lines.push(fg);
    } else {
      if (this.lines.at(index).get('productQty').value < this.lines.at(index).get('max').value) {
        this.lines.at(index).get('productQty').setValue(this.lines.at(index).get('productQty').value + 1);
      }
    }
  }

  loadListProductBoms() {
    this.saleOrderService.getLineForProductRequest(this.saleOrderId).subscribe((res: any) => {
      this.listProductBoms = res;
      console.log(res);
      var listSaleOrderLineId = this.listProductBoms.map(({ id }) => id);
      console.log(listSaleOrderLineId);
      this.loadListProductRequestedBoms(listSaleOrderLineId);
    });
  }

  loadListProductRequestedBoms(listSaleOrderLineId) {
    var val = new SaleOrderLineProductRequestedPaged();
    val.limit = 0;
    val.saleOrderLineIds = listSaleOrderLineId;
    this.saleOrderLineProductRequestedService.getPaged(val).subscribe((res: any) => {
      this.listProductRequestedBoms = res.items;
      this.listProductBoms.forEach(el => {
        el.boms.forEach(el_item => {
          var index = this.findPro_listProductRequestedBoms(el.id, el_item.materialProductId);
          if (index < 0) {
            var temp: SaleOrderLineProductRequestedBasicCus = {
              id: null,
              saleOrderLineId: el.id,
              productId: el_item.materialProductId,
              requestedQuantity: 0,
              max: el_item.quantity
            };
            this.listProductRequestedBoms.push(temp);
          } else {
            this.listProductRequestedBoms[index].max = el_item.quantity - this.listProductRequestedBoms[index].requestedQuantity;
          }
        })
      });
      if (!this.id) {
        this.loadDefault();
      } else {
        this.loadRecord();
      }
    });
  }

  getValueForm(key) {
    return this.formGroup.get(key).value;
  }

  onSave() {
    this.submitted = true;
    if (!this.formGroup.valid) {
      return false;
    }

    var error = false;
    for (let i = 0; i < this.lines.value.length; i++) {
      if (this.lines.value[i]["productQty"] <= 0) {
        this.lines.at(i).get('productQty').setErrors({'incorrect': true});
        error = true;
      }
    }
    if (error == true) {
      return;
    }

    var val = Object.assign({}, this.formGroup.value);
    val.userId = val.user.id;
    val.employeeId = val.employee.id;
    val.saleOrderId = this.saleOrderId;
    val.date = this.intlService.formatDate(val.dateObj, "yyyy-MM-ddTHH:mm:ss");
    console.log(val);
    if (!this.id) {
      this.productRequestService.create(val).subscribe((res: any) => {
        this.notify('success','Lưu thành công');
        this.submitted = false;
        this.activeModal.close();
      }, (err) => {
      });
    } else {
      this.productRequestService.update(this.id, val).subscribe((res: any) => {
        this.notify('success','Lưu thành công');
        this.submitted = false;
        this.activeModal.close();
      }, (err) => {
      });
    }
  }

  onConfirmed() {
    this.submitted = true;
    if (!this.formGroup.valid) {
      return false;
    }

    var error = false;
    for (let i = 0; i < this.lines.value.length; i++) {
      if (this.lines.value[i]["productQty"] <= 0) {
        this.lines.at(i).get('productQty').setErrors({'incorrect': true});
        error = true;
      }
    }
    if (error == true) {
      return;
    }

    if (this.lines.value.length <= 0) {
      this.notify('error','Bạn chưa chọn Vật tư để yêu cầu');
      return;
    }

    var val = Object.assign({}, this.formGroup.value);
    val.userId = val.user.id;
    val.employeeId = val.employee.id;
    val.saleOrderId = this.saleOrderId;
    val.date = this.intlService.formatDate(val.dateObj, "yyyy-MM-ddTHH:mm:ss");
    console.log(val);
    if (!this.id) {
      this.productRequestService.create(val).subscribe((res: any) => {
        this.productRequestService.actionConfirm([res.id]).subscribe((res: any) => {
          this.notify('success','Gửi yêu cầu thành công đến bộ phận Kho');
          this.submitted = false;
          this.activeModal.close();
        }, (err) => {
        });
      }, (err) => {
      });
    } else {
      this.productRequestService.update(this.id, val).subscribe((res: any) => {
        this.productRequestService.actionConfirm([this.id]).subscribe((res: any) => {
          this.notify('success','Gửi yêu cầu thành công đến bộ phận Kho');
          this.submitted = false;
          this.activeModal.close();
        }, (err) => {
        });
      }, (err) => {
      });
    }
  }

  onCancel() {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = "Hủy yêu cầu vật tư";
    modalRef.componentInstance.body = "Bạn có chắc chắn hủy yêu cầu vật tư?";
    modalRef.result.then((res: any) => {
      this.productRequestService.actionCancel([this.id]).subscribe((res: any) => {
        this.notify('success','Hủy thành công');
        this.productRequestDisplay.state = "draft";
        this.formGroup.get('state').setValue("draft");
        this.reload = true;
        this.loadListProductBoms();
      }, (err) => {
      });
    }, (err) => {
    });
  }

  onClose() {
    if (this.reload) {
      this.activeModal.close();
    } else {
      this.activeModal.dismiss();
    }
  }

  clickBom(bom, saleOrderLineId) {
    var index = this.findPro_listProductRequestedBoms(saleOrderLineId, bom.materialProductId);
    if (this.listProductRequestedBoms[index].max <= 0)
      return;
    var val = new GetLinePar();
    val.saleOrderLineId = saleOrderLineId;
    val.productBomId = bom.id;
    this.productRequestLineService.getLine(val).subscribe((res: any) => {
      res.productQty = 1;
      this.loadLineToFormArray(res);
    }, (err) => {
    });
  }

  deleteLine(index: number) {
    this.lines.removeAt(index);
  }

  findPro_listProductRequestedBoms(saleOrderLineId, productId) {
    return this.listProductRequestedBoms.findIndex(item => (item.saleOrderLineId == saleOrderLineId && item.productId == productId));
  }

  notify(type, content) {
    this.notificationService.show({
      content: content,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: type, icon: true }
    });
  }
}
