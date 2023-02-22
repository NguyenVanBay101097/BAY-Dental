import { Component, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { min } from 'date-fns';
import * as _ from 'lodash';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { SaleMakeProductRequestService } from 'src/app/core/services/sale-make-product-request.service';
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
    private saleOrderLineProductRequestedService: SaleOrderLineProductRequestedService,
    private saleMakeProductRequestService: SaleMakeProductRequestService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      dateObj: [null, Validators.required],
      user: [null, Validators.required],
      employee: [null, Validators.required],
      lines: this.fb.array([])
    });

    setTimeout(() => {
      this.loadDefault();
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

  getMax(line: FormGroup) {
    var saleproductionLine = line.get('saleProductionLine').value;
    if (saleproductionLine.quantity == 0) {
      return undefined;
    }

    return Math.max(0, saleproductionLine.quantity - saleproductionLine.quantityRequested);
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
      console.log(res);
      this.listSaleProduction = res;
    });
  }

  updateSaleProduction() {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Cập nhật định mức';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn cập nhật định mức?';
    modalRef.result.then(() => {
      let val = new UpdateSaleProductionReq();
      val.orderId = this.saleOrderId;
      this.saleProductionService.updateSaleProduction(val).subscribe(() => {
        this.notify('success', 'Cập nhật thành công');
        this.loadListSaleProduction();
      })
    });
  }

  deleteSaleProduction(index) {
    var saleProduction = this.listSaleProduction[index];
    var saleProductionLineIds = saleProduction.lines.map(x => x.id);
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa định mức';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa định mức này?';
    modalRef.result.then(() => {
      this.saleProductionService.delete(saleProduction.id).subscribe(() => {
        this.notify('success', 'Xóa thành công');
        this.loadListSaleProduction();
        var controlsRemove = this.lines.controls.filter(x => saleProductionLineIds.indexOf(x.get('saleProductionLine').value.id) !== -1);
        controlsRemove.forEach(control => {
          var i = this.lines.controls.indexOf(control);
          this.lines.removeAt(i);
        });

        if (this.id) {
          this.loadRecord();
        }
      })
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
      this.notify('error', 'Bạn chưa chọn vật tư để yêu cầu');
      return;
    }

    var val = this.formGroup.value;
    var postDate = {
      userId: val.user.id,
      employeeId: val.employee.id,
      saleOrderId: this.saleOrderId,
      date: this.intlService.formatDate(val.dateObj, "yyyy-MM-ddTHH:mm:ss"),
      lines: val.lines.map(x => {
        return {
          saleProductionLineId: x.saleProductionLine.id,
          productQty: x.productQty
        };
      })
    };
   
    this.saleMakeProductRequestService.create(postDate).subscribe(() => {
      this.notify('success', 'Gửi yêu cầu thành công đến bộ phận Kho');
      this.activeModal.close();
    });
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

  addLine(saleProductionLine) {
    if (saleProductionLine.quantity > 0 && saleProductionLine.quantityRequested >= saleProductionLine.quantity) {
      return false;
    }
   
    var index = this.lines.value.findIndex(item => item.saleProductionLine.id == saleProductionLine.id);
    if (index != -1) {
      return false;
    }

    this.lines.push(this.fb.group({
      saleProductionLine: saleProductionLine,
      productQty: [0, Validators.required]
    }));
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
