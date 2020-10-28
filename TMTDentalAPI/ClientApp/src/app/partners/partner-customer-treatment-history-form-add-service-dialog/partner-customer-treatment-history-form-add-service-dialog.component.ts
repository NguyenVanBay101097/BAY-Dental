import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import * as _ from 'lodash';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { EmployeeBasic, EmployeePaged } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { ToothDisplay, ToothFilter, ToothService } from 'src/app/teeth/tooth.service';
import { ToothCategoryBasic, ToothCategoryService } from 'src/app/tooth-categories/tooth-category.service';

@Component({
  selector: 'app-partner-customer-treatment-history-form-add-service-dialog',
  templateUrl: './partner-customer-treatment-history-form-add-service-dialog.component.html',
  styleUrls: ['./partner-customer-treatment-history-form-add-service-dialog.component.css']
})
export class PartnerCustomerTreatmentHistoryFormAddServiceDialogComponent implements OnInit {
  @ViewChild('employeeCbx', { static: true }) private employeeCbx: ComboBoxComponent;

  productService: any;
  line: any;
  saleLineForm: FormGroup
  filteredEmployees: EmployeeBasic[] = [];
  hamList: { [key: string]: {} };
  teethSelected: ToothDisplay[] = [];
  filteredToothCategories: ToothCategoryBasic[] = [];

  constructor(
    private fb: FormBuilder,
    private employeeService: EmployeeService,
    private toothService: ToothService,
    private toothCategoryService: ToothCategoryService,
    private activeModal: NgbActiveModal
  ) { }

  ngOnInit() {
    this.saleLineForm = this.fb.group({
      name: '',
      productId: null,
      productUOMQty: 1,
      state: 'draft',
      diagnostic: '',
      employee: null,
      toothCategory: null
    });
    this.loadData();
    this.loadToothCategories();
    this.loadEmployees();
    setTimeout(() => {
      this.loadDefaultToothCategory().subscribe(result => {
        this.saleLineForm.get('toothCategory').patchValue(result);
        this.loadTeethMap(result);
      })
    });
    this.employeeCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.employeeCbx.loading = true)),
      switchMap(value => this.searchEmployees(value))
    ).subscribe(result => {
      this.filteredEmployees = result.items;
      this.employeeCbx.loading = false;
    });
  }

  loadData() {
    this.saleLineForm.patchValue(this.productService);
  }

  loadEmployees() {
    this.searchEmployees().subscribe(result => {
      this.filteredEmployees = _.unionBy(this.filteredEmployees, result.items, 'id');
      console.log(this.filteredEmployees);

    });
  }

  searchEmployees(filter?: string) {
    var val = new EmployeePaged();
    val.search = filter || '';
    val.isDoctor = true;
    return this.employeeService.getEmployeePaged(val);
  }

  loadDefaultToothCategory() {
    return this.toothCategoryService.getDefaultCategory();
  }

  get lineState() {
    return this.line ? this.line.state : 'draft';
  }

  isSelected(tooth: ToothDisplay) {
    for (var i = 0; i < this.teethSelected.length; i++) {
      if (this.teethSelected[i].id === tooth.id) {
        return true;
      }
    }

    return false;
  }

  getSelectedIndex(tooth: ToothDisplay) {
    for (var i = 0; i < this.teethSelected.length; i++) {
      if (this.teethSelected[i].id === tooth.id) {
        return i;
      }
    }

    return null;
  }

  loadToothCategories() {
    return this.toothCategoryService.getAll().subscribe(result => this.filteredToothCategories = result);
  }

  onSelected(tooth: ToothDisplay) {
    if (this.lineState == "done") {
      return false;
    }

    if (this.isSelected(tooth)) {
      var index = this.getSelectedIndex(tooth);
      this.teethSelected.splice(index, 1);
    } else {
      this.teethSelected.push(tooth);
    }

    //update quantity combobox
    if (this.teethSelected.length > 0) {
      this.saleLineForm.get('productUOMQty').setValue(this.teethSelected.length);
    }
  }

  onChangeToothCategory(value: any) {
    if (value.id) {
      this.teethSelected = [];
      this.loadTeethMap(value);
    }
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

  loadTeethMap(categ: ToothCategoryBasic) {
    var val = new ToothFilter();
    val.categoryId = categ.id;
    return this.toothService.getAllBasic(val).subscribe(result => this.processTeeth(result));
  }

  onSave() {
    if (!this.saleLineForm.valid) {
      return;
    }
    var val = this.saleLineForm.value;
    val.productId = this.productService.id
    val.toothCategoryId = val.toothCategory ? val.toothCategory.id : null;
    val.employeeId = val.employee ? val.employee.id : null;
    val.teeth = this.teethSelected;
    this.activeModal.close(val);
  }
}
