import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import * as _ from 'lodash';
import { debounceTime, map, switchMap, tap } from 'rxjs/operators';
import { EmployeePaged, EmployeeSimple } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { PartnerCategoryDisplay, PartnerCategoryService } from 'src/app/partner-categories/partner-category.service';
import { PartnerService } from 'src/app/partners/partner.service';
import { PartnerCategoriesService } from 'src/app/shared/services/partner-categories.service';
import { ToothDisplay, ToothFilter, ToothService } from 'src/app/teeth/tooth.service';
import { ToothCategoryBasic, ToothCategoryService } from 'src/app/tooth-categories/tooth-category.service';

@Component({
  selector: 'app-sale-order-line-info-popover',
  templateUrl: './sale-order-line-info-popover.component.html',
  styleUrls: ['./sale-order-line-info-popover.component.css']
})
export class SaleOrderLineInfoPopoverComponent implements OnInit {
  formGroup: FormGroup
  title: 'Thông tin bổ sung';
  hamList: { [key: string]: {} };
  teethSelected: ToothDisplay[] = [];
  listTeeths: ToothDisplay[] = [];
  filteredToothCategories: any[] = [];
  filteredEmployees: EmployeeSimple[] = [];
  toolCateg: ToothCategoryBasic = new ToothCategoryBasic();
  @Input() saleOrderState = 'draft';
  @Input() line: any;
  @Output() eventTeeth = new EventEmitter<any>();
  @ViewChild('popOver', { static: true }) public popover: any;
  @ViewChild('employeeCbx', { static: true }) employeeCbx: ComboBoxComponent;

  constructor(
    private partnerCategoriesService: PartnerCategoriesService,
    private partnerCategoryService: PartnerCategoryService,
    private partnerService: PartnerService,
    private toothService: ToothService,
    private toothCategoryService: ToothCategoryService,
    private employeeService: EmployeeService,
    private fb: FormBuilder
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      note: null,
      diagnostic: null,
      toothCategory: null,
      assistant: null,
      assistantId: null,
    })
    this.loadToothCategories();
    this.loadEmployees();

    if (this.line) {
      this.reLoad();

    } else {
      setTimeout(() => {
        this.formGroup = this.fb.group({
          note: null,
          diagnostic: null,
          toothCategory: null,
          assistant: null,
          assistantId: null,
        })

      });
    }

    this.employeeCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => this.employeeCbx.loading = true),
      switchMap(val => this.searchEmployees(val))
    ).subscribe(
      rs => {
        this.filteredEmployees = rs;
        this.employeeCbx.loading = false;
      }
    )

  }

  reLoad() {
    console.log(this.saleOrderState);
    var res = this.line.value;
    if (res.toothCategory) {
      this.loadTeethMap(res.toothCategory);
      this.formGroup.get('toothCategory').patchValue(res.toothCategory);
      this.filteredToothCategories = _.unionBy(this.filteredToothCategories, [res.toothCategory], 'id');
    }
    if (res.Diagnostic) {
      this.formGroup.get('diagnostic').patchValue(res.diagnostic);
    }
    if (res.Teeth) {
      this.teethSelected = Object.assign([], res.teeth);
    }
    // this.teethSelected = [...this.line.teeth];
    // if (this.line.get('toothCategory').value) {
    //   this.loadTeethMap(this.line.get('toothCategory').value)
    //   this.formGroup.get('toothCategory').patchValue(this.line.get('toothCategory').value)
    // }
    // if (this.line.get('teeth')) {
    //   this.teethSelected = [...this.line.get('teeth').value];
    // }
  }

  loadEmployees() {
    this.searchEmployees().subscribe(result => {
      this.filteredEmployees = _.unionBy(this.filteredEmployees, result, 'id');
    });
  }


  searchEmployees(filter?: string) {
    var val = new EmployeePaged();
    val.search = filter || '';
    val.isDoctor = true;
    return this.employeeService.getEmployeeSimpleList(val);
  }

  get ToothCategoryControl() { return this.formGroup.get('toothCategory').value; }

  loadToothCategories() {
    // if (this.line.ToothCategory == null && this.filteredToothCategories.length > 0) {
    //   const cate = this.filteredToothCategories.find(x => x.sequence === 1);
    //   this.formGroup.get('toothCategory').patchValue(cate);
    //   this.onChangeToothCategory(cate);
    //   // this.line.ToothCategoryId').patchValue(cate.Id);
    //   // this.line.ToothCategory').patchValue(cate);
    // }
    return this.toothCategoryService.getAll().subscribe(
      result => {
        this.filteredToothCategories = result;
        if (this.line.get('toothCategory').value == null) {
          this.formGroup.get('toothCategory').patchValue(this.filteredToothCategories[0])
          this.onChangeToothCategory(this.filteredToothCategories[0]);
          this.line.get('toothCategoryId').patchValue(this.filteredToothCategories[0].id);
          this.line.get('toothCategory').patchValue(this.filteredToothCategories[0]);
        }
      }
    );
  }

  // loadToothCategories() {
  //   if (this.line.ToothCategory == null && this.filteredToothCategories.length > 0) {
  //     const cate = this.filteredToothCategories.find(x => x.s === 1);
  //     this.formGroup.get('toothCategory').patchValue(cate);
  //     this.onChangeToothCategory(cate);
  //     // this.line.ToothCategoryId').patchValue(cate.Id);
  //     // this.line.ToothCategory').patchValue(cate);
  //   }
  // }

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

  onSelected(tooth: ToothDisplay) {
    if (this.saleOrderState !== 'draft') {
      return;
    }

    if (this.isSelected(tooth)) {
      var index = this.getSelectedIndex(tooth);
      this.teethSelected.splice(index, 1);
      this.line.get('teeth').value.splice(index, 1);
    } else {
      this.teethSelected.push(tooth);
      this.line.get('teeth').push(this.fb.group(tooth));
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

  onChangeToothCategory(value: any) {
    if (value.id) {
      this.teethSelected = [];
      this.loadTeethMap(value);
      this.line.get('toothCategoryId').patchValue(value.id);
      this.line.get('toothCategory').patchValue(value);
    }
  }

  loadTeethMap(categ: ToothCategoryBasic) {
    var val = new ToothFilter();
    val.categoryId = categ.id;
    return this.toothService.getAllBasic(val).subscribe(
      result => this.processTeeth(result)
    );
  }

  toggleWithTeeth(popover) {
    if (popover.isOpen()) {
      popover.close();
    } else {
      popover.open();
    }
  }

  showInfo() {
    var list = [];
    var teeth = this.line.get('teeth').value;
    if (teeth.length) {
      list.push(teeth.map(x => x.name).join(','));
    }

    if (this.line.get('diagnostic').value) {
      list.push(this.line.get('diagnostic').value);
    }

    return list.join('; ');
  }

  get getDianostic() {
    return this.formGroup.get('diagnostic').value;
  }

  lineTeeth(value) {
    return value.map(x => x.name).join(',');
  }

  onSave() {
    var val = this.formGroup.value;
    val.teeth = this.teethSelected;
    val.assistantId = val.assistant.id;
    this.eventTeeth.emit(val);
    this.popover.close();
  }

  public service$ = (text: string): any => {
    const val = new PartnerCategoryDisplay();
    val.name = text;
    return this.partnerCategoryService.create(val)
      .pipe(
        map((result: any) => {
          return {
            Id: result.id,
            Name: result.name
          }
        })
      );
  }



}
