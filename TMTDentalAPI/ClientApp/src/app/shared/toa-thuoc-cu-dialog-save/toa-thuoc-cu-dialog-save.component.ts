import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, FormArray, Validators } from '@angular/forms';
import { UserPaged, UserService } from 'src/app/users/user.service';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';
import { ProductSimple } from 'src/app/products/product-simple';
import { ProductFilter, ProductService } from 'src/app/products/product.service';
import { DropDownFilterSettings, ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { SamplePrescriptionsService, SamplePrescriptionsDisplay, SamplePrescriptionsSimple } from 'src/app/sample-prescriptions/sample-prescriptions.service';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { ToaThuocService } from 'src/app/toa-thuocs/toa-thuoc.service';
import { EmployeeBasic, EmployeePaged } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import * as _ from 'lodash';

@Component({
  selector: 'app-toa-thuoc-cu-dialog-save',
  templateUrl: './toa-thuoc-cu-dialog-save.component.html',
  styleUrls: ['./toa-thuoc-cu-dialog-save.component.css']
})
export class ToaThuocCuDialogSaveComponent implements OnInit {

  toaThuocForm: FormGroup;
  id: string;
  employeeList: EmployeeBasic[] = [];
  filteredProducts: ProductSimple[];
  defaultVal: any;
  samplePrescriptionAdded: any;
  @ViewChild('employeeCbx', { static: true }) employeeCbx: ComboBoxComponent;
  @ViewChild('samplePrescriptionCbx', { static: true }) samplePrescriptionCbx: ComboBoxComponent;
  title: string;

  public filterSettings: DropDownFilterSettings = {
    caseSensitive: false,
    operator: 'startsWith'
  };
  
  constructor(private fb: FormBuilder, private toaThuocService: ToaThuocService, 
    private userService: UserService, public activeModal: NgbActiveModal, 
    private intlService: IntlService, private errorService: AppSharedShowErrorService,
    private productService: ProductService, private samplePrescriptionsService: SamplePrescriptionsService,
    private employeeService: EmployeeService) { }

  ngOnInit() {
    this.toaThuocForm = this.fb.group({
      name: null, 
      dateObj: null, 
      note: null,
      diagnostic: null,
      employee: null,
      partnerId: null,
      companyId: null,
      dotKhamId: null,
      lines: this.fb.array([]),
      saleOrderId: null
    });

    setTimeout(() => {
      if (this.id) {
        this.loadRecord();
      } else {
        this.loadDefault(this.defaultVal || {});
        this.onCreate();
        this.onCreate();
        this.onCreate();
      }
  
      this.employeeCbx.filterChange.asObservable().pipe(
        debounceTime(300),
        tap(() => this.employeeCbx.loading = true),
        switchMap(val => this.searchEmployees(val))
      ).subscribe(
        result => {
          this.employeeList = result.items;
          this.employeeCbx.loading = false;
        }
      )
  
      this.loadEmployeeList(); 
      this.loadFilteredProducts();
    });
  }

  searchProducts() {
    var val = new ProductFilter();
    val.keToaOK = true;
    val.limit = 1000;
    return this.productService.autocomplete2(val);
  }

  searchEmployees(q?: string) {
    var val = new EmployeePaged();
    val.isDoctor = true;
    val.search = q || '';
    return this.employeeService.getEmployeePaged(val);
  }

  loadEmployeeList() {
    return this.searchEmployees().subscribe(result => {
      this.employeeList = _.unionBy(this.employeeList, result.items, 'id');
    });
  }

  loadFilteredProducts() {
    return this.searchProducts().subscribe(result => {
      this.filteredProducts = _.unionBy(this.filteredProducts, result, 'id');
    });
  }

  get lines() {
    return this.toaThuocForm.get('lines') as FormArray;
  }

  loadRecord() {
    this.toaThuocService.get(this.id).subscribe(
      (result: any) => {
        this.toaThuocForm.patchValue(result);
        let date = new Date(result.date);
        this.toaThuocForm.get('dateObj').patchValue(date);

        if (result.employee) {
          this.employeeList = _.unionBy(this.employeeList, [result.employee], 'id');
        }

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

  loadDefault(val: any) {
    this.toaThuocService.defaultGet(val).subscribe(result => {
      this.toaThuocForm.patchValue(result);
      let date = new Date(result.date);
      this.toaThuocForm.get('dateObj').patchValue(date);
    });
  }

  onCreate() {
    var lines = this.toaThuocForm.get('lines') as FormArray;
    lines.push(this.fb.group({
      product: [null, Validators.required],
      numberOfTimes: 1, 
      amountOfTimes: 1, 
      quantity: 1,  
      numberOfDays: 1, 
      useAt: 'after_meal',
    }));
  }

  updateQuantity(line: FormGroup) {
    var numberOfTimes = line.get('numberOfTimes').value || 0;
    var numberOfDays = line.get('numberOfDays').value || 0;
    var amountOfTimes = line.get('amountOfTimes').value || 0;
    var quantity = numberOfTimes * amountOfTimes * numberOfDays;
    line.get('quantity').setValue(quantity);
  }

  onSave(print) {
    if (!this.toaThuocForm.valid) {
      return false;
    }

    var i = 0;
    while (i < this.lines.value.length) {
      if (this.lines.value[i]['product'] == null) {
        this.lines.removeAt(i);
        i--;
      }
      i++;
    }
    // var controls = this.lines.controls.filter(x => x.get('product').value == null);
    // controls.forEach(control => {
    //   this.lines.removeAt(this.lines.controls.findIndex(x => image.id === 502))
    // });
    
    var val = Object.assign({}, this.toaThuocForm.value);
    val.employeeId = val.employee ? val.employee.id : null;
    val.date = val.dateObj ? this.intlService.formatDate(val.dateObj, 'yyyy-MM-ddTHH:mm:ss') : null;
    val.lines.forEach(line => {
      line.productId = line.product.id;
    });

    if (this.id) {
      this.toaThuocService.update(this.id, val).subscribe(() => {
        this.activeModal.close({
          print
        });
      }, err => {
        this.errorService.show(err);
      });
    } else {
      this.toaThuocService.create(val).subscribe(result => {
        this.activeModal.close({
          item: result,
          print
        });
      }, err => {
        this.errorService.show(err);
      });
    }
  }

  searchUsers(filter: string) {
    var val = new UserPaged();
    val.search = filter;
    return this.userService.autocompleteSimple(val);
  }

  deleteLine(index: number) {
    this.lines.removeAt(index);      
  }

  onSaveSamplePrescription(name) {
    var val = new SamplePrescriptionsDisplay();
    val.name = name;
    val.note = this.toaThuocForm.get('note').value;
    val.lines = this.lines.value;
    val.lines.forEach(line => {
      line.productId = line.product['id'];
    });
    this.samplePrescriptionsService.create(val).subscribe(result => {
      this.samplePrescriptionAdded = result;
    }, err => {
      this.errorService.show(err);
    });
  }

  getNameSamplePrescription(nameSamplePrescription) {
    this.onSaveSamplePrescription(nameSamplePrescription);
  }

  getItemSamplePrescription(itemSamplePrescription) {
    this.samplePrescriptionsService.get(itemSamplePrescription.id).subscribe(result => {
      console.log(result);
      this.toaThuocForm.get('note').patchValue(result.note);

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
    }, err => {
      this.errorService.show(err);
    });
  }
}
