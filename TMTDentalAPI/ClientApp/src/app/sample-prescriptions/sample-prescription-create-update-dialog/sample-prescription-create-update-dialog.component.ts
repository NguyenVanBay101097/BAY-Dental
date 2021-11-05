import { Component, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent, DropDownFilterSettings } from '@progress/kendo-angular-dropdowns';
import { SamplePrescriptionLineService } from 'src/app/core/services/sample-prescription-line.service';
import { ProductSimple } from 'src/app/products/product-simple';
import { ProductFilter, ProductService } from 'src/app/products/product.service';
import { SamplePrescriptionsService } from './../sample-prescriptions.service';

@Component({
  selector: 'app-sample-prescription-create-update-dialog',
  templateUrl: './sample-prescription-create-update-dialog.component.html',
  styleUrls: ['./sample-prescription-create-update-dialog.component.css']
})
export class SamplePrescriptionCreateUpdateDialogComponent implements OnInit {
  PrescriptionForm: FormGroup;
  lineForm: FormGroup;
  id: string;
  submitted = false;
  filteredProducts: ProductSimple[];
  @ViewChild('productCbx', { static: true }) productCbx: ComboBoxComponent;
  title: string;

  get f() { return this.PrescriptionForm.controls; }
  get lines() { return this.PrescriptionForm.get('lines') as FormArray; }

  constructor(private fb: FormBuilder, private samplePrescriptionsService: SamplePrescriptionsService,
    private productService: ProductService, public activeModal: NgbActiveModal, 
    private samplePrescriptionLineService: SamplePrescriptionLineService) { }

  ngOnInit() {
    this.PrescriptionForm = this.fb.group({
      name: [null, Validators.required],
      note: null,
      lines: this.fb.array([]),
    });

    if (this.id) {
      setTimeout(() => {
        this.loadRecord();
      });
    }

    setTimeout(() => {
      this.loadFilteredProducts();
    });

  }

  loadRecord() {
    this.samplePrescriptionsService.get(this.id).subscribe(
      (result: any) => {
        this.PrescriptionForm.patchValue(result);

        this.lines.clear();

        result.lines.forEach(line => {
          this.lines.push(this.fb.group({
            product: line.product,
            productUoM: line.productUoM,
            numberOfTimes: line.numberOfTimes,
            amountOfTimes: line.amountOfTimes,
            quantity: line.quantity,
            numberOfDays: line.numberOfDays,
            useAt: line.useAt,
            note: line.note
          }));
        });
      });
  }

  public filterSettings: DropDownFilterSettings = {
    caseSensitive: false,
    operator: 'startsWith'
  };

  loadFilteredProducts() {
    return this.searchProducts().subscribe(result => {
      this.filteredProducts = result;
    });
  }

  deleteLine(index: number) {
    this.lines.removeAt(index);
  }

  searchProducts(search?: string) {
    var val = new ProductFilter();
    val.keToaOK = true;
    val.search = search;
    return this.productService.autocomplete2(val);
  }

  onSave() {
    this.submitted = true;

    if (!this.PrescriptionForm.valid) {
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

    var val = Object.assign({}, this.PrescriptionForm.value);
    val.lines.forEach(line => {
      line.productId = line.product.id;
      line.productUoMId = line.productUoM ? line.productUoM.id : null;
    });

    if (this.id) {
      this.samplePrescriptionsService.update(this.id, val).subscribe(() => {
        this.activeModal.close();
      }, err => {
      });
    } else {
      this.samplePrescriptionsService.create(val).subscribe(result => {
        this.activeModal.close({
          item: result,

        });
      }, err => {
      });
    }
  }

  onChangeProduct(data: any, item: FormControl) {
    if (data) {
      this.samplePrescriptionLineService.onChangeProduct({ productId: data.id }).subscribe((result: any) => {
        item.get('productUoM').setValue(result.uoM);
      });
    }
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

  onCreate() {
    var lines = this.PrescriptionForm.get('lines') as FormArray;
    lines.push(this.fb.group({
      product: null,
      productUoM: null,
      numberOfTimes: 1,
      amountOfTimes: 1,
      quantity: 1,
      numberOfDays: 1,
      useAt: 'after_meal',
      note: null
    }));
  }


  onCancel() {
    this.activeModal.dismiss();
  }

  updateQuantity(line: FormGroup) {
    var numberOfTimes = line.get('numberOfTimes').value || 0;
    var numberOfDays = line.get('numberOfDays').value || 0;
    var amountOfTimes = line.get('amountOfTimes').value || 0;
    var quantity = numberOfTimes * amountOfTimes * numberOfDays;
    line.get('quantity').setValue(quantity);
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
      case 'other':
        return 'trước khi đi ngủ';
      default:
        return 'sau khi ăn';
    }
  }

}
