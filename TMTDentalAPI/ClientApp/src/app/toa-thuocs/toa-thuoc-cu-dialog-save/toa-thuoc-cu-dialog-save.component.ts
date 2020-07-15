import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormArray, Validators } from '@angular/forms';
import { ToaThuocService, ToaThuocDefaultGet, ToaThuocLineDisplay, ToaThuocLineSave } from '../toa-thuoc.service';
import { UserPaged, UserService } from 'src/app/users/user.service';
import { UserSimple } from 'src/app/users/user-simple';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';
import { ProductSimple } from 'src/app/products/product-simple';
import { ProductFilter, ProductService } from 'src/app/products/product.service';
import { DropDownFilterSettings } from '@progress/kendo-angular-dropdowns';

@Component({
  selector: 'app-toa-thuoc-cu-dialog-save',
  templateUrl: './toa-thuoc-cu-dialog-save.component.html',
  styleUrls: ['./toa-thuoc-cu-dialog-save.component.css']
})
export class ToaThuocCuDialogSaveComponent implements OnInit {

  toaThuocForm: FormGroup;
  id: string;
  userSimpleFilter: UserSimple[] = [];
  filteredProducts: ProductSimple[];
  defaultVal: any;
  title: string;

  public filterSettings: DropDownFilterSettings = {
    caseSensitive: false,
    operator: 'startsWith'
  };
  
  constructor(private fb: FormBuilder, private toaThuocService: ToaThuocService, 
    private userService: UserService, public activeModal: NgbActiveModal, 
    private intlService: IntlService, private errorService: AppSharedShowErrorService,
    private productService: ProductService) { }

  ngOnInit() {
    this.toaThuocForm = this.fb.group({
      name: null, 
      dateObj: null, 
      note: null,
      diagnostic: null,
      user: null,
      userId: null,
      partnerId: null,
      companyId: null,
      dotKhamId: null,
      lines: this.fb.array([]),
    })

    if (this.id) {
      setTimeout(() => {
        this.loadRecord();
      });
    } else {
      setTimeout(() => {
        this.loadDefault(this.defaultVal || {});
      });
    }

    setTimeout(() => {
      this.getUserList(); 
      this.loadFilteredProducts();
    });
  }

  searchProducts() {
    var val = new ProductFilter();
    val.keToaOK = true;
    val.limit = 1000;
    return this.productService.autocomplete2(val);
  }

  loadFilteredProducts() {
    return this.searchProducts().subscribe(result => {
      this.filteredProducts = result;
    });
  }

  get lines() {
    return this.toaThuocForm.get('lines') as FormArray;
  }

  getUserList() {
    var val = new UserPaged;
    this.userService.autocompleteSimple(val).subscribe(
      result => {
        this.userSimpleFilter = result;
      });
  }

  loadRecord() {
    this.toaThuocService.get(this.id).subscribe(
      (result: any) => {
        this.toaThuocForm.patchValue(result);
        let date = new Date(result.date);
        this.toaThuocForm.get('dateObj').patchValue(date);

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
    
    var val = Object.assign({}, this.toaThuocForm.value);
    val.userId = val.user ? val.user.id : null;
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
}
