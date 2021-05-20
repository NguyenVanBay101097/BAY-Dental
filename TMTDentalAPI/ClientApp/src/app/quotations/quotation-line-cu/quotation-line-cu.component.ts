import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NotificationService } from '@progress/kendo-angular-notification';
import { debug } from 'console';
import { EmployeePaged } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { ToothDisplay, ToothFilter, ToothService } from 'src/app/teeth/tooth.service';
import { ToothCategoryService } from 'src/app/tooth-categories/tooth-category.service';
import { QuotationLineDisplay } from '../quotation.service';

@Component({
  selector: 'app-quotation-line-cu',
  templateUrl: './quotation-line-cu.component.html',
  styleUrls: ['./quotation-line-cu.component.css']
})
export class QuotationLineCuComponent implements OnInit {
  @Input() line: QuotationLineDisplay;
  @Output() onUpdateEvent = new EventEmitter<any>();
  @Output() onDeleteEvent = new EventEmitter<any>();
  @Output() onEditEvent = new EventEmitter<any>();
  @Output() onCancelEvent = new EventEmitter<any>();
  @Output() onUpdateOpenPromotionEvent = new EventEmitter<any>();

  formGroup: FormGroup;
  submitted: boolean = false;
  isEditting: boolean = false;
  filteredEmployeesDoctor: any[] = [];
  filteredEmployeesAssistant: any[] = [];
  filteredEmployeesCounselor: any[] = [];
  @Input() initialListEmployees: any = [];
  @Input() filteredToothCategories: any[];
  @Input() initialListTeeths: any[] = [];

  hamList: { [key: string]: {} };
  toothTypeDict = [
    { name: "Hàm trên", value: "upper_jaw" },
    { name: "Nguyên Hàm", value: "whole_jaw" },
    { name: "Hàm dưới", value: "lower_jaw" },
    { name: "Chọn răng", value: "manual" },
  ];

  get TeethFA() {
    return this.formGroup.get("teeth") as FormArray;
  }

  get f() { return this.formGroup.controls; }

  constructor(
    private fb: FormBuilder,
    private employeeService: EmployeeService,
    private notificationService: NotificationService,
    private ToothService: ToothService,
    private toothCategoryService: ToothCategoryService,
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({});
  }

  getValueFormControl(key: string) {
    return this.formGroup.get(key).value;
  }

  getPriceUnitLinePromotion(line) {
    return line.subPrice - (line.amountDiscountTotal || 0);
  }

  getInitialSubTotalLine(line) {
    return line.subPrice * line.qty;
  }

  getPriceSubTotalFormGroup() {
    var quantity = this.formGroup.get('qty').value;
    var priceUnit = this.formGroup.get('subPrice').value;
    var priceReduce = priceUnit - (this.line.amountDiscountTotal || 0);
    return quantity * priceReduce;
  }

  loadEmployees() {
    var val = new EmployeePaged();
    val.limit = 20;
    val.offset = 0;
    val.isDoctor = true;
    val.active = true;

    this.employeeService
      .getEmployeeSimpleList(val)
      .subscribe((result: any[]) => {
        this.initialListEmployees = result;
        this.filteredEmployeesDoctor = this.initialListEmployees.slice();
        this.filteredEmployeesAssistant = this.initialListEmployees.slice();
        this.filteredEmployeesCounselor = this.initialListEmployees.slice();
      });
  }

  onEmployeeDoctor(value) {
    this.filteredEmployeesDoctor = this.initialListEmployees
      .filter((s) => s.name.toLowerCase().indexOf(value.toLowerCase()) !== -1)
      .slice(0, 20);
  }
  onEmployeeAssistant(value) {
    this.filteredEmployeesAssistant = this.initialListEmployees
      .filter((s) => s.name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }

  onEmployeeCounselor(value) {
    this.filteredEmployeesCounselor = this.initialListEmployees
      .filter((s) => s.name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }
  getToothCateLine() {
    var res = this.isEditting ? this.getValueFormControl("toothCategory") : this.line.toothCategory;
    return res;
  }
  onChangeToothCategory(value: any) {
    if (value.id) {
      this.TeethFA.clear();
      this.loadTeethMap(value);
      this.formGroup.get("toothCategory").setValue(value);
    }
  }
  loadTeethMap(categ: any) {
    const result = this.initialListTeeths.filter(
      (x) => x.categoryId === categ.id
    );
    this.processTeeth(result);
  }
  processTeeth(teeth: any[]) {
    this.hamList = {
      "0_up": { "0_right": [], "1_left": [] },
      "1_down": { "0_right": [], "1_left": [] },
    };

    for (var i = 0; i < teeth.length; i++) {
      var tooth = teeth[i];
      if (tooth.position === "1_left") {
        this.hamList[tooth.viTriHam][tooth.position].push(tooth);
      } else {
        this.hamList[tooth.viTriHam][tooth.position].unshift(tooth);
      }
    }
  }

  loadTeethList() {
    var val = new ToothFilter();
    this.ToothService.getAllBasic(val).subscribe((result: any[]) => {
      this.initialListTeeths = result;
      this.onChangeToothCategory(this.line.toothCategory);
    });
  }

  loadToothCategories() {
    this.toothCategoryService.getAll().subscribe((result: any[]) => {
      this.filteredToothCategories = result;
    });
  }

  onChangeToothTypeLine(type) {
    if (type != "manual") {
      this.f.qty.setValue(1);
      (this.f.teeth as FormArray).clear();
    } else {
      var teeth = this.getValueFormControl("teeth") as any[];
      var quantity = teeth && teeth.length > 0 ? teeth.length : 1;
      this.f.qty.setValue(quantity);
    }
  }

  getToothTypeLine() {
    var res = this.isEditting ? this.getValueFormControl("toothType") : this.line.toothType;
    return res;
  }

  onSelected(tooth: ToothDisplay) {
    if (this.isSelected(tooth)) {
      var index = this.getSelectedIndex(tooth);
      this.TeethFA.removeAt(index);
    } else {
      this.TeethFA.push(this.fb.group(tooth));
    }

    this.onChangeToothTypeLine(this.getValueFormControl("toothType"));
  }

  isSelected(tooth: any) {
    var index = -1;
    if (!this.isEditting) {
      index = this.line.teeth.findIndex((x) => x.id == tooth.id);
    } else {
      var teethForm = this.TeethFA.value as any[];
      index = teethForm.findIndex((x) => x.id == tooth.id);
    }
    return index >= 0 ? true : false;
  }

  getSelectedIndex(tooth: any) {
    var teeth = this.TeethFA.value as any[];
    var i = teeth.findIndex((x) => x.id == tooth.id);
    return i;
  }

  editLine() {
    this.onEditEvent.emit(this.line);
  }
  deleteLine() {
    this.onDeleteEvent.emit();
  }

  onOpenPromotion() {
    this.onUpdateOpenPromotionEvent.emit(this.formGroup.value);
  }

  updateLineInfo() {
    if (this.formGroup.invalid) {
      this.formGroup.markAllAsTouched();
      return false;
    }
    this.isEditting = false;
    var value = this.formGroup.value;
    this.onUpdateEvent.emit(value);
    return true;
  }

  onCancel() {
    this.isEditting = false;
    this.onCancelEvent.emit(this.line);
  }

  viewTeeth() {
    var toothType = this.line.toothType;
    if (toothType) {
      if (toothType == "manual") {
        var teeth = this.line.teeth as any[];
        return teeth.map(x => x.name).join(', ');
      } else {
        return this.toothTypeDict.find(x => x.value == toothType).name;
      }
    }
  }

  onEditLine() {
    this.isEditting = true;
    this.formGroup = this.fb.group({
      qty: [this.line.qty, Validators.required],
      subPrice: [this.line.subPrice, Validators.required],
      teeth: this.fb.array(this.line.teeth),
      promotions: this.fb.array(this.line.promotions),
      toothType: this.line.toothType,
      toothCategory: this.line.toothCategory,
      assistant: this.line.assistant,
      employee: this.line.employee,
      counselor: this.line.counselor,
      diagnostic: this.line.diagnostic
    });

    this.formGroup.get('toothType').valueChanges
      .subscribe(value => {
        if (value == 'manual') {
          this.formGroup.get('teeth').setValidators(Validators.required)
        } else {
          this.formGroup.get('teeth').clearValidators();
        }
        this.formGroup.get('teeth').updateValueAndValidity();
      });
    this.formGroup.get('toothType').setValue(this.line.toothType);

    this.loadTeethMap(this.line.toothCategory);
    this.filteredEmployeesDoctor = this.initialListEmployees.filter(x => x.isDoctor == true).slice();
    this.filteredEmployeesCounselor = this.initialListEmployees.slice();
    this.filteredEmployeesAssistant = this.initialListEmployees.filter(x => x.isDoctor == true).slice();
  }

  notify(type, content) {
    this.notificationService.show({
      content: content,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: type, icon: true }
    });
  }
}
