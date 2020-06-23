import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, FormArray, Validators } from '@angular/forms';
import { Observable } from 'rxjs';
import { map, debounceTime, tap, switchMap } from 'rxjs/operators';
import { ToaThuocService, ToaThuocDefaultGet, ToaThuocLineDefaultGet, ToaThuocLineDisplay } from '../toa-thuoc.service';
import { IntlService } from '@progress/kendo-angular-intl';
import { ProductSimple } from 'src/app/products/product-simple';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { ProductService, ProductFilter } from 'src/app/products/product.service';
import { WindowRef, WindowService, WindowCloseResult } from '@progress/kendo-angular-dialog';
import { ToaThuocLineDialogComponent } from '../toa-thuoc-line-dialog/toa-thuoc-line-dialog.component';
import { update } from 'lodash';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';

@Component({
  selector: 'app-toa-thuoc-cu-dialog',
  templateUrl: './toa-thuoc-cu-dialog.component.html',
  styleUrls: ['./toa-thuoc-cu-dialog.component.css']
})
export class ToaThuocCuDialogComponent implements OnInit {
  toaThuocForm: FormGroup;
  lineForm: FormGroup;
  editingLine: FormGroup;//line đang được chỉnh sửa
  id: string;
  lineId: number;//toa thuoc line Id
  dotKhamId: string;
  filteredProducts: ProductSimple[];
  lineSelected: ToaThuocLineDisplay;
  lines: ToaThuocLineDisplay[] = [];
  @ViewChild('productCbx', { static: true }) productCbx: ComboBoxComponent;
  title: string;
  // opened = false;

  constructor(private fb: FormBuilder, private toaThuocService: ToaThuocService, private intlService: IntlService,
    private productService: ProductService, public activeModal: NgbActiveModal, private windowService: WindowService,
    private errorService: AppSharedShowErrorService) { }

  ngOnInit() {
    this.toaThuocForm = this.fb.group({
      name: null,
      dateObj: null,
      lines: this.fb.array([]),
      note: null,
      companyId: null,
      dotKhamId: null,
      userId: null,
      partnerId: null
    });

    // this.lineForm = this.fb.group({
    //   product: [null, Validators.required],
    //   numberOfTimes: 1,
    //   numberOfDays: 1,
    //   amountOfTimes: 1,
    //   quantity: 1,
    //   useAt: 'after_meal',
    //   note: null,
    // });

    if (this.id) {
      setTimeout(() => {
        this.loadRecord();
      });
    } else {
      setTimeout(() => {
        this.loadDefault();
      });
    }

    // this.searchProducts(null).subscribe(result => {
    //   this.filteredProducts = result;
    // });

    // this.productCbx.filterChange.asObservable().pipe(
    //   debounceTime(300),
    //   tap(() => (this.productCbx.loading = true)),
    //   switchMap(value => this.searchProducts(value))
    // ).subscribe(result => {
    //   this.filteredProducts = result;
    //   this.productCbx.loading = false;
    // });
  }

  productChange(value: any) {
    if (value && value.id) {
      var val = new ToaThuocLineDefaultGet();
      val.productId = value.id;
      this.toaThuocService.lineDefaultGet(val).subscribe(result => {
        var lines = this.toaThuocForm.get('lines') as FormArray;
        lines.push(this.fb.group(result));
        this.productCbx.reset();
      });
    }
  }

  onSelectLine(line: ToaThuocLineDisplay) {
    this.lineSelected = line;
  }

  onLineUpdated() {
    this.lineSelected = null;
  }

  // showLineAddModal() {
  //   const windowRef = this.windowService.open({
  //     title: 'Thêm thuốc',
  //     content: ToaThuocLineDialogComponent,
  //     resizable: false,
  //     autoFocusedElement: '[name="name"]',
  //   });

  //   this.opened = true;

  //   windowRef.result.subscribe((result) => {
  //     this.opened = false;
  //     if (result instanceof WindowCloseResult) {
  //     } else {
  //       var line = result as ToaThuocLineDisplay;
  //       var lines = this.toaThuocForm.get('lines') as FormArray;
  //       lines.push(this.fb.group(line));
  //     }
  //   });
  // }

  onLineCreated(line: ToaThuocLineDisplay) {
    this.lines.push(line);
  }

  deleteLine(index: number) {
    if (this.lineId == index) {
      this.lineId = null;
      this.resetLineForm(this.lineForm);
    }
    this.lines.splice(index, 1);
  }

  searchProducts(search?: string) {
    var val = new ProductFilter();
    val.keToaOK = true;
    val.search = search;
    return this.productService.autocomplete2(val);
  }

  loadDefault() {
    var val = new ToaThuocDefaultGet();
    val.dotKhamId = this.dotKhamId;
    this.toaThuocService.defaultGet(val).subscribe(result => {
      this.toaThuocForm.patchValue(result);
      let date = new Date(result.date);
      this.toaThuocForm.get('dateObj').patchValue(date);
    });
  }

  loadRecord() {
    if (this.id) {
      this.toaThuocService.get(this.id).subscribe(result => {
        this.toaThuocForm.patchValue(result);
        let date = new Date(result.date);
        this.toaThuocForm.get('dateObj').patchValue(date);
        this.lines = result.lines;
      });
    }
  }

  get getDotKham() {
    return this.toaThuocForm.get('dotKham').value;
  }

  onSave() {
    if (!this.toaThuocForm.valid) {
      return;
    }

    var val = this.toaThuocForm.value;
    val.date = this.intlService.formatDate(val.dateObj, 'yyyy-MM-ddTHH:mm:ss');
    val.lines = this.lines;
    console.log(val);
    if (this.id) {
      this.toaThuocService.update(this.id, val).subscribe(() => {
        this.activeModal.close(true);
      }, err => {
        this.errorService.show(err);
      });
    } else {
      this.toaThuocService.create(val).subscribe(result => {
        this.activeModal.close(result);
      }, err => {
        this.errorService.show(err);
      });
    }
  }

  onSaveAndPrint() {
    if (!this.toaThuocForm.valid) {
      return;
    }

    var val = this.toaThuocForm.value;
    val.date = this.intlService.formatDate(val.dateObj, 'yyyy-MM-ddTHH:mm:ss');
    val.lines = this.lines;
    if (this.id) {
      this.toaThuocService.update(this.id, val).subscribe(() => {
        this.activeModal.close(true);
      }, err => {
        this.errorService.show(err);
      });
    } else {
      this.toaThuocService.create(val).subscribe(result => {
        this.activeModal.close({ toaThuoc: result, print: true });
      }, err => {
        this.errorService.show(err);
      });
    }
  }

  editLine(id: number) {
    this.lineId = id;
    var lines = this.toaThuocForm.get('lines') as FormArray;
    this.lineForm.patchValue(lines.at(id).value);
  }

  //Thêm toa thuốc list tạm
  onSaveLines(action) {
    if (!this.lineForm.valid) {
      return;
    }
    var val = this.lineForm.value;
    val.productId = val.product.id;

    var line = this.lineForm.value;
    line.productId = val.product.id;
    line.name = this.computeName();
    var lines = this.toaThuocForm.get('lines') as FormArray;
    if (!action) {
      lines.push(this.fb.group(line));
      this.resetLineForm(this.lineForm);
    } else {
      lines.at(this.lineId).patchValue(line);
      this.lineId = null;
      this.resetLineForm(this.lineForm);
    }
  }

  onUpdateToaThuoc() {
    if (!this.toaThuocForm.valid) {
      return;
    }

    if (this.id) {
      var val = this.toaThuocForm.value;
      val.date = this.intlService.formatDate(val.dateObj, 'yyyy-MM-ddTHH:mm:ss');
      this.toaThuocService.update(this.id, val).subscribe(() => {
        this.activeModal.close(true);
      });
    }
  }

  onCancel() {
    this.activeModal.dismiss();
  }

  public onChange(value: number) {
    setTimeout(() => {
      this.updateQuantity();
    }, 200);
  }

  updateQuantity() {
    var numberOfTimes = this.lineForm.get('numberOfTimes').value || 0;
    var numberOfDays = this.lineForm.get('numberOfDays').value || 0;
    var amountOfTimes = this.lineForm.get('amountOfTimes').value || 0;
    var quantity = numberOfTimes * amountOfTimes * numberOfDays;
    this.lineForm.get('quantity').setValue(quantity);
  }

  getNumberOfTimes() {
    return this.lineForm.get('numberOfTimes').value || 0;
  }

  getAmountOfTimes() {
    return this.lineForm.get('amountOfTimes').value || 0;
  }

  getUsedAt() {
    var useAt = this.lineForm.get('useAt').value;
    switch (useAt) {
      case 'before_meal':
        return 'trước khi ăn';
      case 'in_meal':
        return 'trong khi ăn';
      case 'after_wakeup':
        return 'sau khi thức dậy';
      case 'before_sleep':
        return 'trước khi đi ngủ';
      default:
        return 'sau khi ăn';
    }
  }

  computeName() {
    return `Ngày uống ${this.getNumberOfTimes()} lần, mỗi lần ${this.getAmountOfTimes()} viên, uống ${this.getUsedAt()}`
  }

  resetLineForm(form: FormGroup) {
    form.get('product').setValue(null);
    form.get('numberOfTimes').setValue(1);
    form.get('numberOfDays').setValue(1);
    form.get('amountOfTimes').setValue(1);
    form.get('quantity').setValue(1);
    form.get('useAt').setValue('after_meal');
    form.get('note').setValue(null);
  }
}
