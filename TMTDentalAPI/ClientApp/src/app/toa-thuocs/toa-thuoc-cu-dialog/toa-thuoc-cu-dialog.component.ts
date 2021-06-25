import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, FormArray, Validators, FormControl } from '@angular/forms';
import { Observable } from 'rxjs';
import { map, debounceTime, tap, switchMap } from 'rxjs/operators';
import { ToaThuocService, ToaThuocDefaultGet, ToaThuocLineDefaultGet, ToaThuocLineDisplay } from '../toa-thuoc.service';
import { IntlService } from '@progress/kendo-angular-intl';
import { ProductSimple } from 'src/app/products/product-simple';
import { ComboBoxComponent, DropDownFilterSettings } from '@progress/kendo-angular-dropdowns';
import { ProductService, ProductFilter } from 'src/app/products/product.service';
import { WindowRef, WindowService, WindowCloseResult } from '@progress/kendo-angular-dialog';
import { ToaThuocLineDialogComponent } from '../toa-thuoc-line-dialog/toa-thuoc-line-dialog.component';
import { update } from 'lodash';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';
import { SamplePrescriptionsPaged, SamplePrescriptionsService, SamplePrescriptionsSimple } from 'src/app/sample-prescriptions/sample-prescriptions.service';
import { EmployeeBasic, EmployeePaged } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { NotificationService } from '@progress/kendo-angular-notification';
import { ToaThuocLineService } from 'src/app/core/services/toa-thuoc-line.service';
import * as _ from 'lodash';
import { ProductMedicineCuDialogComponent } from 'src/app/products/product-medicine-cu-dialog/product-medicine-cu-dialog.component';

@Component({
  selector: 'app-toa-thuoc-cu-dialog',
  templateUrl: './toa-thuoc-cu-dialog.component.html',
  styleUrls: ['./toa-thuoc-cu-dialog.component.css']
})
export class ToaThuocCuDialogComponent implements OnInit {
  id: string;
  title: string;
  toaThuocForm: FormGroup;
  employeeList: EmployeeBasic[] = [];
  productList: ProductSimple[] = [];
  samplePrescriptionList: SamplePrescriptionsSimple[] = [];
  defaultVal: any;
  samplePrescriptionAdded: any;
  @ViewChild("employeeCbx", { static: true }) employeeCbx: ComboBoxComponent;
  @ViewChild("samplePrescriptionCbx", { static: true }) samplePrescriptionCbx: ComboBoxComponent;

  filterSettings: DropDownFilterSettings = {
    caseSensitive: false,
    operator: "startsWith",
  };

  constructor(private fb: FormBuilder,
    private toaThuocService: ToaThuocService,
    public activeModal: NgbActiveModal,
    private intlService: IntlService,
    private errorService: AppSharedShowErrorService,
    private employeeService: EmployeeService,
    private productService: ProductService,
    private samplePrescriptionsService: SamplePrescriptionsService,
    private modalService: NgbModal,
    private notificationService: NotificationService,
    private toaThuocLineService: ToaThuocLineService) { }

  ngOnInit() {
    this.toaThuocForm = this.fb.group({
      partner: null,
      employee: null,
      samplePrescription: null,
      lines: this.fb.array([]),
      note: null,
      reExaminationDateObj: null,
      saveSamplePrescription: false,
      nameSamplePrescription: null,
      partnerId: null,
      companyId: null,
      saleOrderId: null,
      diagnostic: null
    });

    setTimeout(() => {
      if (this.id) {
        this.loadRecord();
      } else {
        this.loadDefault(this.defaultVal || {});
        this.onCreate();
      }

      this.employeeCbx.filterChange.asObservable().pipe(
        debounceTime(300),
        tap(() => (this.employeeCbx.loading = true)),
        switchMap((val) => this.searchEmployees(val))
      )
        .subscribe((result) => {
          this.employeeList = result.items;
          this.employeeCbx.loading = false;
        });

      this.samplePrescriptionCbx.filterChange.asObservable().pipe(
        debounceTime(300),
        tap(() => (this.samplePrescriptionCbx.loading = true)),
        switchMap((val) => this.searchSamplePrescriptions(val))
      )
        .subscribe((result) => {
          this.samplePrescriptionList = result.items;
          this.samplePrescriptionCbx.loading = false;
        });

      this.loadEmployeeList();
      this.loadSamplePrescriptionList();
      this.loadProductList();
    });
  }

  get lines() {
    return this.toaThuocForm.get("lines") as FormArray;
  }

  getFBValueItem(item) {
    return this.toaThuocForm.get(item).value;
  }

  getUOM(val) {
    var res = val.value;
    if (res.product == null) {
      return;
    }
    return res.product.uomName;
  }


  onChangeProduct(data: any, item: FormControl) {
    if (data) {
      this.toaThuocLineService.onChangeProduct({ productId: data.id }).subscribe((result: any) => {
        item.get('productUoM').setValue(result.uoM);
      });
    }
  }

  searchEmployees(search?: string) {
    var val = new EmployeePaged();
    val.isDoctor = true;
    val.search = search || "";
    return this.employeeService.getEmployeePaged(val);
  }

  loadEmployeeList() {
    return this.searchEmployees().subscribe((result) => {
      this.employeeList = _.unionBy(this.employeeList, result.items, "id");
    });
  }

  searchSamplePrescriptions(search?: string) {
    var val = new SamplePrescriptionsPaged();
    val.search = search || "";
    return this.samplePrescriptionsService.getPaged(val);
  }

  loadSamplePrescriptionList() {
    return this.searchSamplePrescriptions().subscribe((result) => {
      this.samplePrescriptionList = _.unionBy(this.samplePrescriptionList, result.items, "id");
    });
  }

  onCheckSaveSamplePrescription(event: any) {
    var checked = event.currentTarget.checked;
    if (checked) {
      this.toaThuocForm.get('nameSamplePrescription').setValidators(Validators.required);
      this.toaThuocForm.get('nameSamplePrescription').updateValueAndValidity();
    } else {
      this.toaThuocForm.get('nameSamplePrescription').clearValidators();
      this.toaThuocForm.get('nameSamplePrescription').updateValueAndValidity();
    }
  }

  searchProducts() {
    var val = new ProductFilter();
    val.keToaOK = true;
    val.limit = 1000;
    return this.productService.autocomplete2(val);
  }

  loadProductList() {
    return this.searchProducts().subscribe((result) => {
      this.productList = _.unionBy(this.productList, result, "id");
    });
  }

  selectionChangeSamplePrescription(item) {
    this.samplePrescriptionsService.get(item.id).subscribe(
      (result) => {
        this.toaThuocForm.get("note").patchValue(result.note);

        this.lines.clear();

        result.lines.forEach((line) => {
          this.lines.push(
            this.fb.group({
              product: [line.product, Validators.required],
              productUoM: line.productUoM,
              numberOfTimes: line.numberOfTimes,
              amountOfTimes: line.amountOfTimes,
              quantity: line.quantity,
              numberOfDays: line.numberOfDays,
              useAt: line.useAt,
            })
          );
        });
      },
      (err) => {
        this.errorService.show(err);
      }
    );
  }

  onChangeUseAt(index, val) {
    var item = this.lines.controls[index];
    if (val.useAt == 'other') {
      item.get('useAt').setValue(val.useAt);
      item.get('note').setValue(val.note);
    } else {
      item.get('useAt').setValue(val.useAt);
    }
  }

  createMedicine() {
    let modalRef = this.modalService.open(ProductMedicineCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = "Thêm: Thuốc";
    modalRef.result.then(() => {
      this.notificationService.show({
        content: 'Thêm thuốc mới thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
      this.loadProductList();
    }, () => {
    });
  }

  loadDefault(val: any) {
    this.toaThuocService.defaultGet(val).subscribe((result) => {
      this.toaThuocForm.patchValue(result);
    });
  }

  onCreate() {
    var lines = this.toaThuocForm.get("lines") as FormArray;
    lines.push(
      this.fb.group({
        product: null,
        productUoM: null,
        numberOfTimes: 1,
        amountOfTimes: 1,
        quantity: 1,
        numberOfDays: 1,
        useAt: "after_meal",
        note: null
      })
    );
  }

  loadRecord() {
    this.toaThuocService.getFromUI(this.id).subscribe((result: any) => {
      this.toaThuocForm.patchValue(result);
      if (result.reExaminationDate) {
        let reExaminationDate = new Date(result.reExaminationDate);
        this.toaThuocForm.get("reExaminationDateObj").setValue(reExaminationDate);
      }

      if (result.employee) {
        this.employeeList = _.unionBy(this.employeeList, [result.employee], "id");
      }

      this.lines.clear();

      result.lines.forEach((line) => {
        this.lines.push(
          this.fb.group({
            id: line.id,
            product: [line.product, Validators.required],
            productUoM: line.productUoM,
            numberOfTimes: line.numberOfTimes,
            amountOfTimes: line.amountOfTimes,
            quantity: line.quantity,
            numberOfDays: line.numberOfDays,
            useAt: line.useAt,
            note: line.note
          })
        );
      });
    });
  }

  updateQuantity(line: FormGroup) {
    var numberOfTimes = line.get("numberOfTimes").value || 0;
    var numberOfDays = line.get("numberOfDays").value || 0;
    var amountOfTimes = line.get("amountOfTimes").value || 0;
    var quantity = numberOfTimes * amountOfTimes * numberOfDays;
    line.get("quantity").setValue(quantity);
  }

  onSave(print) {
    if (!this.toaThuocForm.valid) {
      return false;
    }

    var i = 0;
    while (i < this.lines.value.length) {
      if (this.lines.value[i]["product"] == null) {
        this.lines.removeAt(i);
        i--;
      }
      i++;
    }

    var val = Object.assign({}, this.toaThuocForm.value);
    val.employeeId = val.employee ? val.employee.id : null;
    val.reExaminationDate = val.reExaminationDateObj ? this.intlService.formatDate(val.reExaminationDateObj, "yyyy-MM-ddTHH:mm:ss") : null;
    val.lines.forEach((line) => {
      line.productId = line.product.id;
      line.productUoMId = line.productUoM ? line.productUoM.id : null;
    });

    if (this.id) {
      this.toaThuocService.updateFromUI(this.id, val).subscribe(
        () => {
          this.activeModal.close({ print, });
        },
        (err) => {
          this.errorService.show(err);
        }
      );
    } else {
      this.toaThuocService.createFromUI(val).subscribe(
        (result) => {
          this.activeModal.close({ item: result, print, });
        },
        (err) => {
          this.errorService.show(err);
        }
      );
    }
  }

  deleteLine(index: number) {
    this.lines.removeAt(index);
  }

  getUsedAt(useAt) {;
    switch (useAt) {
      case 'before_meal':
        return 'Trước khi ăn';
      case 'in_meal':
        return 'Trong khi ăn';
      case 'after_wakeup':
        return 'Sau khi thức dậy';
      case 'before_sleep':
        return 'Trước khi đi ngủ';
      case 'other':
        return 'Khác';
      default:
        return 'Sau khi ăn';
    }
  }
}
