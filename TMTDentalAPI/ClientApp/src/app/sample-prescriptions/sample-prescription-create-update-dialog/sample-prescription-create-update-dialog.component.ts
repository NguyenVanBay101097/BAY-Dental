import { SamplePrescriptionLineSave, SamplePrescriptionsService } from './../sample-prescriptions.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, FormArray } from '@angular/forms';
import { ProductSimple } from 'src/app/products/product-simple';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { WindowService } from '@progress/kendo-angular-dialog';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ProductService, ProductFilter } from 'src/app/products/product.service';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';

@Component({
  selector: 'app-sample-prescription-create-update-dialog',
  templateUrl: './sample-prescription-create-update-dialog.component.html',
  styleUrls: ['./sample-prescription-create-update-dialog.component.css']
})
export class SamplePrescriptionCreateUpdateDialogComponent implements OnInit {
  PrescriptionForm: FormGroup;
  lineForm: FormGroup;
  editingLine: FormGroup;//line đang được chỉnh sửa
  id: string;
  lineId: number;//toa thuoc line Id
  filteredProducts: ProductSimple[];
  lineSelected: SamplePrescriptionLineSave;
  lines: SamplePrescriptionLineSave[] = [];
  @ViewChild('productCbx', { static: true }) productCbx: ComboBoxComponent;
  title: string;
  constructor(private fb: FormBuilder, private samplePrescriptionsService: SamplePrescriptionsService, private intlService: IntlService,
    private productService: ProductService, public activeModal: NgbActiveModal,
    private errorService: AppSharedShowErrorService) { }

  ngOnInit() {
    this.PrescriptionForm = this.fb.group({
      name: null,
      note: null,
      lines: this.fb.array([]),
    });

    if (this.id) {
      setTimeout(() => {
        this.samplePrescriptionsService.get(this.id).subscribe((result) => {
          this.PrescriptionForm.patchValue(result);
          this.lines = result.lines;    
        });
      });
    }
  }

  // productChange(value: any) {
  //   if (value && value.id) {
  //     var val = new SamplePrescriptionLineSave();
  //     val.productId = value.id;
  //     this.samplePrescriptionsService.lineDefaultGet(val).subscribe(result => {
  //       var lines = this.PrescriptionForm.get('lines') as FormArray;
  //       lines.push(this.fb.group(result));
  //       this.productCbx.reset();
  //     });
  //   }
  // }

  onSelectLine(line: SamplePrescriptionLineSave) {
    this.lineSelected = line;
  }

  onLineUpdated() {
    this.lineSelected = null;
  }

  onLineCreated(line: SamplePrescriptionLineSave) {
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


  onSave() {
    if (!this.PrescriptionForm.valid) {
      return;
    }

    var val = this.PrescriptionForm.value;
    val.lines = this.lines;
    if (this.id) {
      this.samplePrescriptionsService.update(this.id, val).subscribe(() => {
        this.activeModal.close(true);
      }, err => {
        this.errorService.show(err);
      });
    } else {
      this.samplePrescriptionsService.create(val).subscribe(result => {
        this.activeModal.close(result);
      }, err => {
        this.errorService.show(err);
      });
    }
  }

  editLine(id: number) {
    this.lineId = id;
    var lines = this.PrescriptionForm.get('lines') as FormArray;
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
    var lines = this.PrescriptionForm.get('lines') as FormArray;
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
    if (!this.PrescriptionForm.valid) {
      return;
    }

    if (this.id) {
      var val = this.PrescriptionForm.value;
      this.samplePrescriptionsService.update(this.id, val).subscribe(() => {
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


   getUsedAt(useAt) {
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

  // computeName() {
  //   return `Ngày uống ${this.getNumberOfTimes()} lần, mỗi lần ${this.getAmountOfTimes()} viên, uống ${this.getUsedAt()}`
  // }

  resetLineForm(form: FormGroup) {
    form.get('product').setValue(null);
    form.get('numberOfTimes').setValue(1);
    form.get('numberOfDays').setValue(1);
    form.get('amountOfTimes').setValue(1);
    form.get('quantity').setValue(1);
    form.get('useAt').setValue('after_meal');
  }

}
