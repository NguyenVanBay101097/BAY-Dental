import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { FormArray, FormBuilder, FormGroup } from "@angular/forms";
import { EmployeePaged } from "src/app/employees/employee";
import { EmployeeService } from "src/app/employees/employee.service";
import { SaleOrderLineDisplay } from "../sale-order-line-display";
import {
  ToothDisplay,
  ToothFilter,
  ToothService,
} from "src/app/teeth/tooth.service";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { SaleOrderLineLaboOrdersDialogComponent } from "../sale-order-line-labo-orders-dialog/sale-order-line-labo-orders-dialog.component";
import { ToothCategoryService } from "src/app/tooth-categories/tooth-category.service";
import { NotificationService } from "@progress/kendo-angular-notification";
import { SaleOrderPromotionDialogComponent } from "../sale-order-promotion-dialog/sale-order-promotion-dialog.component";
import { BehaviorSubject, Subject } from "rxjs";
import { SaleOrderLineService } from "src/app/core/services/sale-order-line.service";
import { ConfirmDialogComponent } from "src/app/shared/confirm-dialog/confirm-dialog.component";
import { SaleOrderLinePromotionDialogComponent } from "../sale-order-line-promotion-dialog/sale-order-line-promotion-dialog.component";

@Component({
  selector: "app-sale-order-line-cu",
  templateUrl: "./sale-order-line-cu.component.html",
  styleUrls: ["./sale-order-line-cu.component.css"],
})

export class SaleOrderLineCuComponent implements OnInit {
  @Input() line: SaleOrderLineDisplay;
  @Input() isFast = false;

  @Output() onUpdateEvent = new EventEmitter<any>();
  @Output() onUpdateOpenPromotionEvent = new EventEmitter<any>();
  @Output() onDeleteEvent = new EventEmitter<any>();
  @Output() onEditEvent = new EventEmitter<any>();
  @Output() onCancelEvent = new EventEmitter<any>();

  isEditting: boolean = false;
  isItSeff = false;
  // canEdit = true;

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
  formGroupInfo: FormGroup;

  constructor(
    private fb: FormBuilder,
    private employeeService: EmployeeService,
    private ToothService: ToothService,
    private modalService: NgbModal,
    private toothCategoryService: ToothCategoryService,
    private notificationService: NotificationService,
    private saleOrderLineService: SaleOrderLineService
  ) { }

  ngOnInit() {
    this.formGroupInfo = this.fb.group(this.line);

    this.formGroupInfo.setControl('teeth', this.fb.array(this.line.teeth));
    this.formGroupInfo.setControl('promotions', this.fb.array(this.line.promotions));

    this.computeAmount();

    this.loadEmployees();
    this.loadToothCategories();
    this.loadTeethList();
  }

  get TeethFA() {
    return this.formGroupInfo.get("teeth") as FormArray;
  }

  getPriceUnitLinePromotion(line) {
    return line.priceUnit - (line.amountDiscountTotal || 0);
  }

  formInfoControl(value: string) {
    return this.formGroupInfo.get(value);
  }

  getInitialSubTotalLine(line) {
    return line.priceUnit * line.productUOMQty;
  }

  editLine() {
    this.onEditEvent.emit(this.line);
  }

  onEditLine() {
    this.isEditting = true;
    // this.canEdit = true;
    this.formGroupInfo = this.fb.group(this.line);
    this.formGroupInfo.setControl("teeth", this.fb.array(this.line.teeth));
    this.formGroupInfo.setControl("promotions", this.fb.array(this.line.promotions));

  }

  onEmployeeFilter(value) {
    this.filteredEmployeesDoctor = this.initialListEmployees
      .filter((s) => s.name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }

  onEmployeeAssistant(value) {
    this.filteredEmployeesAssistant = this.initialListEmployees
      .filter((s) => s.name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }

  onEmployeeCounselor(value) {
    this.filteredEmployeesCounselor = this.initialListEmployees
      .filter((s) => s.name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }

  loadEmployees() {
    var val = new EmployeePaged();
    val.limit = 0;
    val.offset = 0;
    val.isDoctor = true;
    val.active = true;

    this.employeeService
      .getEmployeeSimpleList(val)
      .subscribe((result: any[]) => {
        this.initialListEmployees = result;
        this.filteredEmployeesDoctor = this.initialListEmployees.slice();
        this.filteredEmployeesCounselor = this.initialListEmployees.slice();
        this.filteredEmployeesAssistant = this.initialListEmployees.slice();
      });
  }

  showLaboList(id?: string) {
    const modalRef = this.modalService.open(
      SaleOrderLineLaboOrdersDialogComponent,
      {
        scrollable: true,
        size: "xl",
        windowClass: "o_technical_modal",
        keyboard: false,
        backdrop: "static",
      }
    );

    modalRef.componentInstance.title = "Danh sách phiếu labo";
    modalRef.componentInstance.saleOrderLineId = id;
    modalRef.result.then(
      (val) => { },
      (er) => { }
    );
  }

  deleteLine() {
    this.onDeleteEvent.emit();
  }

  loadToothCategories() {
    this.toothCategoryService.getAll().subscribe((result: any[]) => {
      this.filteredToothCategories = result;
    });
  }

  getToothCateLine() {
    var res =
      this.isEditting
        ? this.formInfoControl("toothCategory").value
        : this.line.toothCategory;
    return res;
  }

  getToothTypeLine() {
    var res =
      this.isEditting
        ? this.formInfoControl("toothType").value
        : this.line.toothType;
    return res;
  }

  onChangeToothTypeLine(type) {
    if (type != "manual") {
      this.formInfoControl("productUOMQty").setValue(1);
      (this.formInfoControl("teeth") as FormArray).clear();
    } else {
      var teeth = this.formInfoControl("teeth").value as any[];
      var quantity = teeth && teeth.length > 0 ? teeth.length : 1;
      this.formInfoControl("productUOMQty").setValue(quantity);
    }

    this.onChangeQuantity();
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

  onChangeToothCategory(value: any) {
    if (value.id) {
      this.TeethFA.clear();
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

  updateLineInfo(isItseft = false) {
    if(this.formGroupInfo.invalid) {
      return false;
    }

    if(this.formInfoControl('toothType').value == 'manual' && this.formInfoControl('teeth').value.length == 0) {
      this.notify('error', 'Chọn răng');
      return false;
    }

    this.isEditting = false;
    this.onUpdateEvent.emit(this.formGroupInfo.value);

    this.isItSeff = this.isItSeff;
    if(isItseft) {
      this.notify('success', 'Cập nhật thành công');
    }
    return true;
  }

  onChangeQuantity() {
    if(this.formGroupInfo.invalid) {
      return;
    }
    this.computeAmount();
  }

  computeAmount() {
    // var discountType = this.formInfoControl('discountType') ? line.get('discountType').value : 'percentage';
    // var discountFixedValue = line.get('discountFixed') ? line.get('discountFixed').value : 0;
    // var discountNumber = line.get('discount') ? line.get('discount').value : 0;
    var getquanTity = this.formInfoControl("productUOMQty")
      ? this.formInfoControl("productUOMQty").value
      : 1;
    var getamountPaid = this.formInfoControl("amountPaid")
      ? this.formInfoControl("amountPaid").value
      : 0;
    var priceUnit = this.getPriceUnitLinePromotion(this.formGroupInfo.value);

    // var subtotal = discountType == 'percentage' ? price * (1 - discountNumber / 100) :
    //   Math.max(0, price - discountFixedValue);
    this.formInfoControl("priceSubTotal").setValue(priceUnit * getquanTity);
    var getResidual = priceUnit * getquanTity - getamountPaid;
    if (this.formInfoControl("state").value != "draft") {
      this.formInfoControl("amountResidual").setValue(getResidual);
    }
  }

  updateTeeth(line, lineControl) {
    line.productUOMQty =
      line.teeth && line.teeth.length > 0 ? line.teeth.length : 1;
    lineControl.patchValue(line);
    lineControl.get("teeth").clear();
    line.Teeth.forEach((teeth) => {
      let g = this.fb.group(teeth);
      lineControl.get("teeth").push(g);
    });
  }

  notify(Style, Content) {
    this.notificationService.show({
      content: Content,
      hideAfter: 3000,
      position: { horizontal: "center", vertical: "top" },
      animation: { type: "fade", duration: 400 },
      type: { style: Style, icon: true },
    });
  }

  onOpenPromotion() {
    // let modalRef = this.modalService.open(SaleOrderLinePromotionDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
    // modalRef.componentInstance.saleOrderLine = this.line;
    // modalRef.componentInstance.getUpdateSJ().subscribe(res => {
    //   this.onUpdateOpenPromotionEvent.emit(null);
    //   // modalRef.componentInstance.saleOrderLine = this.line;
    // });
    // this.isEditting = false;
    this.onUpdateOpenPromotionEvent.emit(null);
  }

  // onOpenPromotionDialog() {
  //   let modalRef = this.modalService.open(SaleOrderLinePromotionDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
  //   if(!this.line.promotions) this.line.promotions = [];
  //   modalRef.componentInstance.saleOrderLine = this.line;
  //   modalRef.result.then(() => {
  //     // this.onEmit.emit({action: 'reload', data: null});

  //   }, () => {
  //   });
  // }

  onCancel() {
    this.isEditting = false;
    this.onCancelEvent.emit(this.line);
  }

  viewTeeth() {
    var toothType = this.line.toothType;
    if (toothType == "manual") {
      var teeth = this.line.teeth as any[];
      return teeth.map(x => x.name).join(',');
    } else {
      return this.toothTypeDict.find(x => x.value == toothType).name;
    }
  }

  onActive(active) {
    this.saleOrderLineService.patchIsActive(this.line.id, active).subscribe(() => {
      this.line.isActive = active;
      this.notify('success', 'Thành công');
    });

  }

}
