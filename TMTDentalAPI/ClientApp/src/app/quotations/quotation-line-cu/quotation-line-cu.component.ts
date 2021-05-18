import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NotificationService } from '@progress/kendo-angular-notification';
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
  initialListEmployees: any = [];
  filteredToothCategories: any[];
  hamList: { [key: string]: {} };
  initialListTeeths: any[];
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
    this.formGroup = this.fb.group({
      subPrice: [0, Validators.required],
      qty: [0, Validators.required],
      amount: 0,
      employee: null,
      assistant: null,
      counselor: null,
      toothType: null,
      toothCategory: null,
      toothIds: null,
      diagnostic: '',
      amountDiscountTotal: 0,
      amountPromotionToOrder: 0,
      amountPromotionToOrderLine: 0,
      teeth: this.fb.array([]),
      promotions: this.fb.array([])
    });

    this.formGroup.patchValue(this.line);
    this.formGroup.setControl("teeth", this.fb.array(this.line.teeth));
    this.formGroup.setControl("promotions", this.fb.array(this.line.promotions));

    this.loadEmployees();
    this.loadToothCategories();
    this.loadTeethList();
    this.computeAmount();
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

  computeAmount() {
    var getquanTity = this.getValueFormControl("qty") ? this.getValueFormControl("qty") : 1;
    var priceUnit = this.getPriceUnitLinePromotion(this.formGroup.value);
    this.f.amount.setValue(priceUnit * getquanTity);
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

  onChangeQuantity(val) {
    this.computeAmount();
  }

  editLine() {
    this.onEditEvent.emit(this.line);
  }
  deleteLine() {
    this.onDeleteEvent.emit();
  }

  onOpenPromotion() {
    if (!this.checkValidFormGroup()) {
      this.isEditting = true;
      return;
    }
    else {
      this.isEditting = false;
      this.onUpdateOpenPromotionEvent.emit(this.formGroup.value);
    }
  }

  checkValidFormGroup() {
    if (this.formGroup.invalid)
      return false;
    if (this.getValueFormControl("toothType") == "manual" && !this.getValueFormControl("teeth").length) {
      this.notify("error", "Vui lòng chọn răng");
      return false;
    }
    return true;
  }

  updateLineInfo() {
    this.submitted = true;

    if (!this.checkValidFormGroup()) {
      this.isEditting = true;
      return;
    }
    else {
      this.isEditting = false;
      var value = this.formGroup.value;
      this.onUpdateEvent.emit(value);
    }
  }

  onCancel() {
    this.isEditting = false;
    this.onCancelEvent.emit(this.line);
  }

  viewTeeth() {
    var toothType = this.line.toothType;
    if (toothType == "manual") {
      var teeth = this.line.teeth as any[];
      return teeth.map(x => x.name).join(', ');
    } else {
      return this.toothTypeDict.find(x => x.value == toothType).name;
    }
  }

  onEditLine() {
    this.isEditting = true;
    // this.formGroup.patchValue(this.line);
    this.formGroup.setControl("teeth", this.fb.array(this.line.teeth));
    this.formGroup.setControl("promotions", this.fb.array(this.line.promotions));
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
