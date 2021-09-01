import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent, MultiSelectComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { result, values } from 'lodash';
import { Observable, of, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, switchMap, tap } from 'rxjs/operators';
import { AdvisoryDefaultGet, AdvisoryService } from 'src/app/advisories/advisory.service';
import { EmployeeBasic, EmployeePaged, EmployeeSimple } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { ProductPaged, ProductService } from 'src/app/products/product.service';
import { ToothDisplay, ToothFilter, ToothService } from 'src/app/teeth/tooth.service';
import { ToothCategoryBasic, ToothCategoryService } from 'src/app/tooth-categories/tooth-category.service';
import { ToothDiagnosisPaged, ToothDiagnosisSave, ToothDiagnosisService } from 'src/app/tooth-diagnosis/tooth-diagnosis.service';
import {ToothDiagnosisPopoverComponent} from '../partner-customer-advisory-list/tooth-diagnosis-popover/tooth-diagnosis-popover.component'

@Component({
  selector: 'app-partner-customer-advisory-cu-dialog',
  templateUrl: './partner-customer-advisory-cu-dialog.component.html',
  styleUrls: ['./partner-customer-advisory-cu-dialog.component.css']
})
export class PartnerCustomerAdvisoryCuDialogComponent implements OnInit {

  @ViewChild("empCbx", { static: true }) empCbx: ComboBoxComponent;
  @ViewChild('multiSelect', { static: true }) multiSelect: MultiSelectComponent;
  myForm: FormGroup;
  hamList: { [key: string]: {} };
  teethSelected: ToothDisplay[] = [];
  teethSelectedById: ToothDisplay[] = [];
  filteredToothCategories: any[] = [];
  productSelectedFromApi: any[] = [];
  cateId: string;
  submitted = false;
  title: string;
  customerId: string;
  id: string;
  filterData: EmployeeBasic[] = [];
  changesCount = 0;
  toothTypeDict = [
    { name: "Hàm trên", value: "upper_jaw" },
    { name: "Nguyên hàm", value: "whole_jaw" },
    { name: "Hàm dưới", value: "lower_jaw" },
    { name: "Chọn răng", value: "manual" },
  ];
  productSource: any;
  toothDianosisSource: any;
  searchUpdateToothDianosis = new Subject<string>();
  searchUpdateProduct = new Subject<string>();

  constructor(
    public activeModal: NgbActiveModal,
    private fb: FormBuilder,
    private toothService: ToothService,
    private toothCategoryService: ToothCategoryService,
    private advisoryService: AdvisoryService,
    private intlService: IntlService,
    private notificationService: NotificationService,
    private toothDiagnosisService: ToothDiagnosisService,
    private employeeService: EmployeeService,
    private productService: ProductService,
    private router: Router
  ) { }

  ngOnInit() {
    this.myForm = this.fb.group({
      dateObj: [null, Validators.required],
      customerId: [null, Validators.required],
      customer: [null],
      employeeId: null,
      employeeAdvisory: null,
      toothCategoryId:[null],
      toothType: "manual",
      teeth: [],
      toothDiagnosis: [[], Validators.required],
      product: [],
      note: [],
      companyId: [null,Validators.required]
    })

    setTimeout(() => {
      this.loadToothCategories();
      this.loadDefaultToothCategory().subscribe(result => {
        this.cateId = result.id;
        this.loadTeethMap(result);
      })
      
      this.loadEmployees();
      this.getPageDiagnosis('');
      this.getPageProduct('');
      if(this.id) {
        this.getById();
      } else {
        this.getDefault();
      }
    });

    this.empCbx.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.empCbx.loading = true)),
        switchMap((value) => this.searchEmployees(value))
      )
      .subscribe((result) => {
        this.filterData = result.items;
        this.empCbx.loading = false;
      });
    this.searchUpdateToothDianosis.pipe(
      debounceTime(300),
      distinctUntilChanged())
      .subscribe(value => {
        this.searchToothDiagnosis(value);
      });
    this.searchUpdateProduct.pipe(
      debounceTime(300),
      distinctUntilChanged())
      .subscribe(value => {
        this.getPageProduct(value);
      });

    this.f.toothDiagnosis.valueChanges.subscribe(data => {
      var ids = data.map(x => x.id);
      this.toothDiagnosisService.getProducts(ids).subscribe(result => {
        var products = this.f.product.value;
        products = products.filter(elem => this.productSelectedFromApi.findIndex(x => x.id == elem.id) == -1);
        products = products.concat(result);
        var unique = products.filter(function (elem, index, self) {
          return index === self.findIndex(x => x.id == elem.id);
        })
        if (this.changesCount != 0){
          this.f.product.setValue(unique);
        }
        this.productSelectedFromApi = result;
        this.changesCount += 1;
      })
    })
  }

  get f() { return this.myForm.controls; }

  getValueFormControl(key: string) {
    return this.myForm.get(key).value;
  }

  loadTeethMap(categ: ToothCategoryBasic) {
    var val = new ToothFilter();
    val.categoryId = categ.id;
    return this.toothService.getAllBasic(val).subscribe(result => this.processTeeth(result));
  }

  processTeeth(teeth: ToothDisplay[]) {
    this.hamList = {
      '0_up': { '0_right': [], '1_left': [] },
      '1_down': { '0_right': [], '1_left': [] }
    };

    for (var i = 0; i < teeth.length; i++) {
      var tooth = teeth[i];
      if (tooth.position === '1_left') {
        this.hamList[tooth.viTriHam][tooth.position].push(tooth);
      } else {
        this.hamList[tooth.viTriHam][tooth.position].unshift(tooth);
      }
    }
  }

  onChangeToothType(toothType) {
    if (toothType != "manual") {
      this.teethSelected = [];
      this.f.teeth.setValue(this.teethSelected);
    }
  }

  isSelected(tooth: ToothDisplay) {
    for (var i = 0; i < this.teethSelected.length; i++) {
      if (this.teethSelected[i].id === tooth.id) {
        return true;
      }
    }

    return false;
  }

  onSelected(tooth: ToothDisplay) {
    if (this.getValueFormControl("toothType") == "manual") {
      if (this.isSelected(tooth)) {
        var index = this.getSelectedIndex(tooth);
        this.teethSelected.splice(index, 1);
      } else {
        this.teethSelected.push(tooth);
      }
      this.f.teeth.setValue(this.teethSelected);
    }
  }

  getSelectedIndex(tooth: ToothDisplay) {
    for (var i = 0; i < this.teethSelected.length; i++) {
      if (this.teethSelected[i].id === tooth.id) {
        return i;
      }
    }

    return null;
  }

  loadDefaultToothCategory() {
    return this.toothCategoryService.getDefaultCategory();
  }

  loadToothCategories() {
    return this.toothCategoryService.getAll().subscribe(result => this.filteredToothCategories = result);
  }

  searchEmployees(q?:string) {
    var val = new EmployeePaged();
    val.limit = 10;
    val.offset = 0;
    val.search = q || '';
    return this.employeeService.getEmployeePaged(val);
  }

  searchToothDiagnosis(q?: string){
    var val = new ToothDiagnosisPaged();
    val.limit = 10;
    val.offset = 0;
    val.search = q || '';
    this.toothDiagnosisService.getPaged(val).subscribe(
    result => {
      this.toothDianosisSource = result.items;
    }, error => {
      console.log(error);
    });
  }

  loadEmployees() {
    this.searchEmployees().subscribe(result => {
      this.filterData = result.items;
    }, error => {
      console.log(error);
    });
  }

  onChangeToothCategory(value: any) {
    if (value.id) {
      this.teethSelected = [];
      this.f.teeth.setValue(this.teethSelected);
      this.loadTeethMap(value);
      this.cateId = value.id;
    }
  }

  onSave() {
    this.submitted = true;
    if (!this.myForm.valid) {
      return false;
    }

    if (this.getValueFormControl('toothType') == 'manual' && (this.getValueFormControl('teeth') && this.getValueFormControl('teeth').length == 0) )
      return;
    var valueForm = this.myForm.value;
    if (valueForm.employeeAdvisory) {
      valueForm.employeeId = valueForm.employeeAdvisory.id;
    }
    valueForm.date = this.intlService.formatDate(valueForm.dateObj, 'yyyy-MM-ddTHH:mm:ss');
    valueForm.toothCategoryId = this.cateId ;
    valueForm.toothIds = this.teethSelected.map(x => x.id);
    valueForm.toothDiagnosisIds = valueForm.toothDiagnosis.map(x => x.id);
    valueForm.productIds = valueForm.product.map(x => x.id);
    if (this.id) {
      this.advisoryService.update(valueForm,this.id).subscribe(() => {
        this.notify("success","Lưu thành công");
        this.activeModal.close(true);
      })
    } else {
      this.advisoryService.create(valueForm).subscribe(() => {
        this.notify("success","Lưu thành công");
        this.activeModal.close(true);
      })
    }
  }

  notify(style, content) {
    this.notificationService.show({
      content: content,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: style, icon: true }
    });
  }

  getDefault() {
    this.changesCount += 1;
    var val = new AdvisoryDefaultGet();
    val.customerId = this.customerId
    this.advisoryService.getDefault(val).subscribe(result => {
      this.myForm.patchValue(result);
      let date = new Date(result.date);
      this.myForm.get('dateObj').patchValue(date);
      this.myForm.get('employeeAdvisory').patchValue(result.employee);
    })
  }

  getById() {
    this.advisoryService.get(this.id).subscribe(result => {
      this.cateId = result.toothCategoryId;
      this.loadTeethMap(result.toothCategory);
      this.teethSelected = result.teeth;
      this.teethSelectedById = result.teeth;
      this.myForm.patchValue(result);
      let date = new Date(result.date);
      this.myForm.get('dateObj').patchValue(date);
      this.myForm.get('employeeAdvisory').patchValue(result.employee);
      var ids = this.f.toothDiagnosis.value.map(x => x.id);
      this.toothDiagnosisService.getProducts(ids).subscribe(result => {
        this.productSelectedFromApi = result;
      })
    });
    
  }
  resetForm(){
    if (this.id) {
      this.getById();
    } else {
      this.getDefault();
      this.teethSelected = [];
    }
    
  }

  viewPartner() {
    this.router.navigate(['/partners/customer/'+this.customerId+'/overview']);
    this.activeModal.dismiss();
  }

  getPageDiagnosis(q?: string){
    var val = new ToothDiagnosisPaged();
    val.limit = 10;
    val.offset = 0;
    val.search = q || '';
    this.toothDiagnosisService.getPaged(val).subscribe(result => {
      this.toothDianosisSource = result.items;
    })
  }

  getPageProduct(q?:string){
    var val = new ProductPaged();
    val.limit = 0;
    val.offset = 0;
    val.search = q || '';
    val.type2 = 'service';
    this.productService.getPaged(val).subscribe(result => {
      this.productSource = result.items;
    })
  }

  public valueNormalizer = (text$: Observable<string>): any => text$.pipe(
    switchMap((text: string) => {
      // Search in values
      const matchingValue: any = this.f.toothDiagnosis.value.find((item: any) => {
        // Search for matching item to avoid duplicates
        return item['name'].toLowerCase() === text.toLowerCase();
      });

      if (matchingValue) {
        // Return the already selected matching value and the component will remove it
        return of(matchingValue);
      }

      // Search in data
      const matchingItem: any = this.toothDianosisSource.find((item: any) => {
        return item['name'].toLowerCase() === text.toLowerCase();
      });

      if (matchingItem) {
        return of(matchingItem);
      } else {
        
        return of(text).pipe(switchMap(this.service$));
      }
    })
  )

  public service$ = (text: string): any => {
    var val = new ToothDiagnosisSave();
    val.productIds = [];
    val.name = text;
    return this.toothDiagnosisService.create(val).pipe(
      map((result: any) => {
        return {
          id: result.id,
          name: result.name
        }
      })
    );
      
  }
  
}
