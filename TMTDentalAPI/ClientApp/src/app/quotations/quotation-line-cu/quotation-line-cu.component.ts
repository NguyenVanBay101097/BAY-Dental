import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
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


  isEditting: boolean = false;
  filteredEmployees: any[] = [];
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
  formGroupInfo: FormGroup;
  constructor(
    private fb: FormBuilder,
    private employeeService: EmployeeService,
    private modalService: NgbModal,
    private notificationService: NotificationService,
    private ToothService: ToothService,
    private toothCategoryService: ToothCategoryService,
  ) { }

  ngOnInit() {
    this.formGroupInfo = this.fb.group(this.line);
    // this.formGroupInfo.controls["advisoryEmployee"].setValidators(Validators.required);
    this.formGroupInfo.setControl("teeth", this.fb.array(this.line.teeth));
    this.formGroupInfo.setControl("promotions", this.fb.array(this.line.promotions));
    // if (this.line.teeth) {
    //   this.line.teeth.forEach((tooth) => {
    //     this.TeethFA.push(this.fb.group(tooth));
    //   });
    // }

    this.loadEmployees();
    this.loadToothCategories();
    this.loadTeethList();
    this.computeAmount();
  }
  get TeethFA() {
    return this.formGroupInfo.get("teeth") as FormArray;
  }
  get f() { return this.formGroupInfo.controls; }

  formInfoControl(value: string) {
    return this.formGroupInfo.get(value);
  }

  getPriceUnitLinePromotion(line) {
    return line.subPrice - (line.amountDiscountTotal || 0);
  }

  getInitialSubTotalLine(line) {
    return line.subPrice * line.qty;
  }

  computeAmount() {
    var getquanTity = this.formInfoControl("qty")
      ? this.formInfoControl("qty").value
      : 1;
    var priceUnit = this.getPriceUnitLinePromotion(this.formGroupInfo.value);
    this.formInfoControl("amount").setValue(priceUnit * getquanTity);
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
        this.filteredEmployees = this.initialListEmployees.slice(0, 20);
      });
  }

  onEmployeeFilter(value) {
     this.filteredEmployees = this.initialListEmployees
      .filter((s) => s.name.toLowerCase().indexOf(value.toLowerCase()) !== -1)
      .slice(0, 20);
  }

  getToothCateLine() {
    var res =
      this.isEditting
        ? this.formInfoControl("toothCategory").value
        : this.line.toothCategory;
    return res;
  }
  onChangeToothCategory(value: any) {
    if (value.id) {
      // this.TeethFA.clear();
      this.loadTeethMap(value);
      this.formGroupInfo.get("toothCategory").setValue(value);
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
      this.formInfoControl("qty").setValue(1);
      (this.formInfoControl("teeth") as FormArray).clear();
    } else {
      var teeth = this.formInfoControl("teeth").value as any[];
      var quantity = teeth && teeth.length > 0 ? teeth.length : 1;
      this.formInfoControl("qty").setValue(quantity);
    }

    // this.onChangeQuantity();
  }

  getToothTypeLine() {
    var res =
      this.isEditting
        ? this.formInfoControl("toothType").value
        : this.line.toothType;
    return res;
  }

  onSelected(tooth: ToothDisplay) {
    if (this.isSelected(tooth)) {
      var index = this.getSelectedIndex(tooth);
      this.TeethFA.removeAt(index);
    } else {
      this.TeethFA.push(this.fb.group(tooth));
    }

    this.onChangeToothTypeLine(this.formInfoControl("toothType").value);
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
    if (!val) {
      this.formInfoControl("qty").patchValue(1);
    }
    this.computeAmount();
  }

  editLine() {
    this.onEditEvent.emit(this.line);
  }
  deleteLine() {
    this.onDeleteEvent.emit();
  }

  onOpenPromotion() { 
    this.isEditting = false;
    this.onUpdateOpenPromotionEvent.emit(this.formGroupInfo.value);
  }

  updateLineInfo() {
      this.isEditting = false;
      var value = this.formGroupInfo.value;
      this.onUpdateEvent.emit(value);
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
    this.formGroupInfo = this.fb.group(this.line);
    // this.formGroupInfo.controls["advisoryEmployee"].setValidators(Validators.required);
    this.formGroupInfo.setControl("teeth", this.fb.array(this.line.teeth));
    this.formGroupInfo.setControl("promotions", this.fb.array(this.line.promotions));
  
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
