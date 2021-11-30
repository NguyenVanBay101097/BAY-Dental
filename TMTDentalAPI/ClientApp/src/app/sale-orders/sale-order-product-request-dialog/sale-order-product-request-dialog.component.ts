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
import { SaleProductionService, UpdateSaleProductionReq } from 'src/app/core/services/sale-production.service';
import { EmployeePaged, EmployeeSimple } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { ProductRequestLineService } from 'src/app/shared/product-request-line.service';
import { ProductRequestService } from 'src/app/shared/product-request.service';
import { SaleOrderLineProductRequestedBasicCus, SaleOrderLineProductRequestedPaged, SaleOrderLineProductRequestedService } from 'src/app/shared/sale-order-line-product-requested.service';
import { ProductRequestDefaultGet, ProductRequestDisplay, ProductRequestGetLinePar } from '../product-request';

@Component({
  selector: 'app-sale-order-product-request-dialog',
  templateUrl: './sale-order-product-request-dialog.component.html',
  styleUrls: ['./sale-order-product-request-dialog.component.css']
})
export class SaleOrderProductRequestDialogComponent implements OnInit {
  title: string;
  id: string;
  saleOrderId: string;
  formGroup: FormGroup;
  submitted = false;
  productRequestDisplay: ProductRequestDisplay = new ProductRequestDisplay();
  listProductBoms: SaleOrderLineForProductRequest[] = [];
  listProductRequestedBoms: SaleOrderLineProductRequestedBasicCus[] = [];
  listSaleProduction: any[];

  @ViewChild('employeeCbx', { static: true }) employeeCbx: ComboBoxComponent;
  filteredEmployees: EmployeeSimple[] = [];

  reload = false;

  get f() { return this.formGroup.controls; }

  get lines() { return this.formGroup.get('lines') as FormArray; }

  get seeForm() {
    var state = this.productRequestDisplay.state;
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
    private saleProductionService: SaleProductionService,
    private saleOrderLineProductRequestedService: SaleOrderLineProductRequestedService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      dateObj: [null, Validators.required],
      user: [null, Validators.required],
      employee: [null, Validators.required],
      lines: this.fb.array([])
    });

    setTimeout(() => {
      if (!this.id) {
        this.loadDefault();
      } else {
        this.loadRecord();
      }

      this.loadEmployees();
      this.loadListSaleProduction();
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
      res.saleOrderId = this.saleOrderId;
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

  getMax(line: FormGroup , id) {
    var saleproductionLineId = line.get('id').value;
    var productId = line.get('productId').value;
    var item = this.listSaleProduction.find(x => x.id == id);
    if (item) {
      var bom = item.lines.find(x => x.productId == productId);
      if (bom) {
        if (bom.quantity === 0) {
          return undefined;
        }
        return bom.quantity - bom.quantityRequested;
      }
    }

    return undefined;
  }

  loadLinesToFormArray(lines) {
    this.lines.clear();
    lines.forEach(line => {
      var fg = this.fb.group({
        ...line,
        productQty: [line.productQty, Validators.required]
      });
      this.lines.push(fg);
    });
  }

  loadListSaleProduction() {
    this.saleOrderService.getSaleProductionBySaleOrderId(this.saleOrderId).subscribe((res: any) => {
      this.listSaleProduction = res;
    });
  }

  updateSaleProduction() {
    let val = new UpdateSaleProductionReq();
    val.orderId = this.saleOrderId;
    this.saleProductionService.updateSaleProduction(val).subscribe(() => {
      this.loadListSaleProduction();
    })
  }


  getValueForm(key) {
    return this.formGroup.get(key).value;
  }

  onSave() {
    this.submitted = true;
    if (!this.formGroup.valid) {
      return false;
    }

    var val = Object.assign({}, this.formGroup.value);
    val.userId = val.user.id;
    val.employeeId = val.employee.id;
    val.saleOrderId = this.saleOrderId;
    val.date = this.intlService.formatDate(val.dateObj, "yyyy-MM-ddTHH:mm:ss");
    if (!this.id) {
      this.productRequestService.create(val).subscribe((res: any) => {
        this.notify('success', 'Lưu thành công');
        this.submitted = false;
        this.activeModal.close();
      }, (err) => {
      });
    } else {
      this.productRequestService.update(this.id, val).subscribe((res: any) => {
        this.notify('success', 'Lưu thành công');
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

    if (this.lines.value.length <= 0) {
      this.notify('error', 'Bạn chưa chọn Vật tư để yêu cầu');
      return;
    }

    var val = Object.assign({}, this.formGroup.value);
    val.userId = val.user.id;
    val.employeeId = val.employee.id;
    val.saleOrderId = this.saleOrderId;
    val.date = this.intlService.formatDate(val.dateObj, "yyyy-MM-ddTHH:mm:ss");
    if (!this.id) {
      this.productRequestService.create(val).subscribe((res: any) => {
        this.productRequestService.actionConfirm([res.id]).subscribe((res: any) => {
          this.notify('success', 'Gửi yêu cầu thành công đến bộ phận Kho');
          this.submitted = false;
          this.activeModal.close();
        }, (err) => {
        });
      }, (err) => {
      });
    } else {
      this.productRequestService.update(this.id, val).subscribe((res: any) => {
        this.productRequestService.actionConfirm([this.id]).subscribe((res: any) => {
          this.notify('success', 'Gửi yêu cầu thành công đến bộ phận Kho');
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
        this.notify('success', 'Hủy thành công');
        this.activeModal.close();
        // this.loadListProductBoms();
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

  addLine(saleProductionLine , parentId) {
    if (saleProductionLine.quantity !== 0) {
      if (saleProductionLine.quantityRequested >= saleProductionLine.quantity) {
        return;
      }
    }

    var index = this.lines.value.findIndex(item => (item.saleProductionLineId == saleProductionLine.id && item.productId == saleProductionLine.productId));
    if (index < 0) {
      var val = new ProductRequestGetLinePar();
      val.saleProductionLineId = saleProductionLine.id;
      this.productRequestLineService.getLine(val).subscribe((res: any) => {
        console.log(res);
        this.lines.push(this.fb.group({
          ...res,
          productQty: [res.productQty, Validators.required]
        }));
      }, (err) => {
      });
    } else {
      var line = this.lines.at(index) as FormGroup;
      if (saleProductionLine.quantity !== 0) {
        if (line.get('productQty').value < this.getMax(line,parentId)) {
          line.get('productQty').setValue(line.get('productQty').value + 1);
        }
      } else {
        line.get('productQty').setValue(line.get('productQty').value + 1);
      }
    }
  }

  deleteLine(index: number) {
    this.lines.removeAt(index);
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
