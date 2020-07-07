import { SamplePrescriptionLineSave, SamplePrescriptionsService } from './../sample-prescriptions.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, FormArray, Validators } from '@angular/forms';
import { ProductSimple } from 'src/app/products/product-simple';
import { ComboBoxComponent, DropDownFilterSettings } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
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
  id: string;
  submitted = false;
  filteredProducts: ProductSimple[];
  @ViewChild('productCbx', { static: true }) productCbx: ComboBoxComponent;
  title: string;
 
  constructor(private fb: FormBuilder, private samplePrescriptionsService: SamplePrescriptionsService, private intlService: IntlService,
    private productService: ProductService, public activeModal: NgbActiveModal,
    private errorService: AppSharedShowErrorService) { }

  ngOnInit() {
    this.PrescriptionForm = this.fb.group({
      name: [null , Validators.required],
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
        let date = new Date(result.date);      

        this.lines.clear();

        result.lines.forEach(line => {
          this.lines.push(this.fb.group({
            product: [line.product, Validators.required],
            numberOfTimes: line.numberOfTimes, 
            amountOfTimes: line.amountOfTimes, 
            quantity: line.quantity,  
            numberOfDays: line.numberOfDays, 
            useAt: line.useAt,
          }));
        });
      });
  }

  public filterSettings: DropDownFilterSettings = {
    caseSensitive: false,
    operator: 'startsWith'
  };

  get lines() {
    return this.PrescriptionForm.get('lines') as FormArray;
  }

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

    var val = Object.assign({}, this.PrescriptionForm.value);   
    val.lines.forEach(line => {
      line.productId = line.product.id;
    });

    if (this.id) {
      this.samplePrescriptionsService.update(this.id, val).subscribe(() => {
       this.activeModal.close();
      }, err => {
        this.errorService.show(err);
      });
    } else {
      this.samplePrescriptionsService.create(val).subscribe(result => {
        this.activeModal.close({
          item: result,
          
        });
      }, err => {
        this.errorService.show(err);
      });
    }
  }

  onCreate() {
    var lines = this.PrescriptionForm.get('lines') as FormArray;
    lines.push(this.fb.group({
      product: [null, Validators.required],
      numberOfTimes: 1, 
      amountOfTimes: 1, 
      quantity: 1,  
      numberOfDays: 1, 
      useAt: 'after_meal',
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
      default:
        return 'sau khi ăn';
    }
  }

  get f() {
    return this.PrescriptionForm.controls;
  }

}
