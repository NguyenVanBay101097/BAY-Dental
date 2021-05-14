import { animate, state, style, transition, trigger } from '@angular/animations';
import { Component, OnInit, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { debounceTime, delay, map, switchMap, tap } from 'rxjs/operators';
import { EmployeeBasic, EmployeePaged, EmployeeSimple } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { PrintService } from 'src/app/shared/services/print.service';
import { ToothDisplay, ToothFilter, ToothService } from 'src/app/teeth/tooth.service';
import { ToothCategoryBasic, ToothCategoryService } from 'src/app/tooth-categories/tooth-category.service';
import { QuotationLineCuComponent } from '../quotation-line-cu/quotation-line-cu.component';
import { QuotationLinePromotionDialogComponent } from '../quotation-line-promotion-dialog/quotation-line-promotion-dialog.component';
import { QuotationPromotionDialogComponent } from '../quotation-promotion-dialog/quotation-promotion-dialog.component';
import { QuotationLineDisplay, QuotationsDisplay, QuotationService } from '../quotation.service';

@Component({
  selector: 'app-quotation-create-update-form',
  templateUrl: './quotation-create-update-form.component.html',
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({ height: '0px', minHeight: '0' })),
      state('expanded', style({ height: '*' })),
      transition('expanded <=> collapsed', animate('300ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ],
  styleUrls: ['./quotation-create-update-form.component.css']
})
export class QuotationCreateUpdateFormComponent implements OnInit {

  @ViewChild("empCbx", { static: true }) empCbx: ComboBoxComponent;
  formGroup: FormGroup;
  formGroupInfo: FormGroup;
  partner: any;
  employee: any;
  // employeeId: string;
  saleOrders: any;
  dateFrom: Date;
  dateTo: Date;
  toothCategoryId: string;
  partnerId: string;
  quotationId: string;
  // quotationLine: any;
  public selectedLine: any;
  hamList: { [key: string]: {} };
  teethSelected: any[] = [];
  filteredToothCategories: any[] = [];
  quotation: any = new QuotationsDisplay();
  filteredAdvisoryEmployees: EmployeeSimple[] = [];
  search: string = '';
  filterData: EmployeeBasic[] = [];
  isEditing: boolean = true;
  lineSelected = null;
  defaultToothCate: ToothCategoryBasic;
  filteredEmployees: any[] = [];
  initialListEmployees: any = [];
  submitted: boolean = false;
  @ViewChildren('lineTemplate') lineVCR: QueryList<QuotationLineCuComponent>;
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());

  isChanged: boolean = false;
  isPercentType: boolean = false;

  get linesArray() {
    return this.formGroup.get('lines') as FormArray;
  }

  get paymentsArray() {
    return this.formGroup.get('payments') as FormArray;
  }

  constructor(
    private fb: FormBuilder,
    private activatedRoute: ActivatedRoute,
    private quotationService: QuotationService,
    private toothService: ToothService,
    private toothCategoryService: ToothCategoryService,
    private notificationService: NotificationService,
    private intlService: IntlService,
    private router: Router,
    private printService: PrintService,
    private employeeService: EmployeeService,
    private modalService: NgbModal,
    private notifyService: NotifyService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      partnerId: ['', Validators.required],
      // employeeId: null,
      employee: [null, Validators.required],
      note: '',
      dateQuotationObj: [null, Validators.required],
      dateApplies: [0, Validators.required],
      dateEndQuotationObj: [null, Validators.required],
      companyId: '',
      lines: this.fb.array([
      ]),
      payments: this.fb.array([]),
    })
    this.routeActive();
    this.loadToothCategories();

    this.loadEmployees();

    this.formGroup.valueChanges.subscribe(res => {
      this.isChanged = true;
    });

    this.empCbx.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.empCbx.loading = true)),
        switchMap((value) => this.searchEmployees(value))
      )
      .subscribe((result: any) => {
        this.filterData = result.items;
        this.empCbx.loading = false;
      });
  }

  routeActive() {
    this.activatedRoute.queryParamMap.pipe(
      switchMap((params: ParamMap) => {
        this.quotationId = params.get("id");
        this.partnerId = params.get("partner_id");
        if (this.quotationId) {
          return this.quotationService.get(this.quotationId);
        } else {
          return this.quotationService.defaultGet(this.partnerId);
        }
      })).subscribe(
        result => {
          this.patchValueQuotation(result);
        }
      )
  }
  get f() { return this.formGroup.controls; }

  patchValueQuotation(result) {
    this.quotation = result;
    this.partner = result.partner;
    this.partnerId = result.partnerId;
    this.employee = result.employee;
    // this.employeeId = result.employeeId;
    this.saleOrders = result.orders;
    this.formGroup.patchValue(result);
    this.formGroup.get('dateEndQuotationObj').patchValue(new Date(result.dateEndQuotation));
    this.formGroup.get('dateQuotationObj').patchValue(new Date(result.dateQuotation));
    const control = this.formGroup.get('lines') as FormArray;
    control.clear();

    result.lines.forEach(line => {
      this.addLine(line, false);
    });

    const paymentcontrol = this.formGroup.get('payments') as FormArray;
    paymentcontrol.clear();

    result.payments.forEach(payment => {
      payment.dateObj = new Date(payment.date);
      var g = this.fb.group(payment);
      paymentcontrol.push(g);
    });

    this.formGroup.markAsPristine();
  }

  searchEmployees(q?: string) {
    var val = new EmployeePaged();
    val.limit = 10;
    val.offset = 0;
    val.search = q || '';
    return this.employeeService.getEmployeePaged(val);
  }

  loadEmployees() {
    this.searchEmployees().subscribe(result => {
      this.filterData = result.items;
    })
  }

  printQuotation() {
    if (this.quotationId) {
      this.quotationService.printQuotation(this.quotationId).subscribe((result: any) => {
        this.printService.printHtml(result.html);
      })
    }
  }

  onCreateSaleOrder() {
    if (!this.quotationId) {
      this.notificationService.show({
        content: 'Bạn phải lưu báo giá trước khi tạo phiếu điều trị !',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });
      return;
    }
    this.quotationService.createSaleOrderByQuotation(this.quotationId).subscribe(
      (result: any) => {
        this.router.navigate(['sale-orders/form'], { queryParams: { id: result.id } });
      }
    )
  }

  onCreateNewQuotation() {
    this.router.navigate(['quotations/form'], { queryParams: { partner_id: this.partnerId } });
  }

  onChangeQuantity(line: FormGroup) {
    var res = this.linesArray.controls.find(x => x.value.productId === line.value.productId);
    if (res) {
      res.patchValue(line.value);
    }
  }

  getAmountTotal() {
    let totalAmount = 0;
    var lines = this.formGroup.get('lines').value;
    if (lines && lines.length > 0) {
      lines.forEach(line => {
        totalAmount += line.amount;
      });
    }
    return totalAmount;
  }

  getDate(dateQuotation: Date, dateApplies: number) {
    var dateEnd = new Date(dateQuotation.getFullYear(), dateQuotation.getMonth(), dateQuotation.getDate() + dateApplies);
    this.formGroup.get('dateEndQuotationObj').patchValue((new Date(dateEnd)));
  }

  onDateChange(date: Date) {
    let dateAppliesChange = this.formGroup.get('dateApplies') ? this.formGroup.get('dateApplies').value : null;
    if (date && dateAppliesChange) {
      this.getDate(date, dateAppliesChange);
    }
  }

  onDateAppliesChange(dateApplies) {
    let dateQuotation = this.formGroup.get('dateQuotationObj') ? this.formGroup.get('dateQuotationObj').value : null;
    if (dateQuotation && dateApplies) {
      this.getDate(dateQuotation, dateApplies);
    }
  }

  createFormInfo(data: any) {
    this.formGroupInfo = this.fb.group({
      toothCategory: data ? data.toothCategory : null,
      toothCategoryId: data ? data.toothCategoryId : '',
      diagnostic: data ? data.diagnostic : '',
      employeeId: data.employeeId ? data.employeeId : '',
      employee: data.employee ? data.employee : null,
      assistantId: data.assistantId ? data.assistantId : '',
      assistant: data.assistant ? data.assistant : null,
      counselorId: data.counselorId ? data.counselorId : '',
      counselor: data.counselor ? data.counselor : null
    })
    this.loadTeethMap(data.toothCategory);
    if (data.teeth) {
      this.teethSelected = Object.assign([], data.teeth);
    }
  }
  // load teeth 
  loadTeethMap(categ: ToothCategoryBasic) {
    var val = new ToothFilter();
    val.categoryId = categ.id;
    return this.toothService.getAllBasic(val).subscribe(
      result => this.processTeeth(result)
    );
  }

  onSelected(tooth: ToothDisplay) {
    if (this.isSelected(tooth)) {
      var index = this.getSelectedIndex(tooth);
      this.teethSelected.splice(index, 1);
    } else {
      this.teethSelected.push(tooth);
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
    return this.toothCategoryService.getAll().subscribe(
      result => {
        this.filteredToothCategories = result;
      }
    );
  }

  onChangeToothCategory(value: any) {
    if (value.id) {
      this.teethSelected = [];
      this.loadTeethMap(value);
      this.formGroupInfo.get('toothCategoryId').patchValue(value.id);
      this.formGroupInfo.get('toothCategory').patchValue(value);
    }
  }
  //end load teeth

  //Payment 
  onAddPayment() {
    var payment = {
      payment: 0,
      discountPercentType: 'cash',
      amount: 0,
      sequence: 1,
      dateObj: new Date()
    }
    var paymentGroup = this.fb.group(payment);
    this.paymentsArray.push(paymentGroup);
  }

  deletePayment(index) {
    this.paymentsArray.removeAt(index);
    this.paymentsArray.markAsDirty();
  }
  //end payment

  // Luu
  getDataFormGroup() {
    var value = this.formGroup.value;
    value.dateQuotation = this.intlService.formatDate(value.dateQuotationObj, 'yyyy-MM-ddTHH:mm:ss');
    value.dateEndQuotation = this.intlService.formatDate(value.dateEndQuotationObj, 'yyyy-MM-ddTHH:mm:ss');
    value.companyId = this.quotation.companyId;
    value.employeeId = value.employee ? value.employee.id : value.employeeId;
    value.totalAmount = this.getAmountTotal();
    delete value.employee;
    if (this.quotationId) {
      value.Id = this.quotationId;
    }
    if (value.lines) {
      value.lines.forEach(line => {
        line.toothIds = [];
        if (line.teeth) {
          line.toothIds = line.teeth.map(x => x.id)
        }
      });
    }
    if (value.payments) {
      value.payments.forEach(pm => {
        pm.dateObj = this.intlService.formatDate(pm.dateObj, "yyyy-MM-dd");
      });
    }
    return value;
  }

  onSave() {
    if (this.lineSelected != null) {
      var viewChild = this.lineVCR.find(x => x.line == this.lineSelected);
      viewChild.updateLineInfo();
    }

    this.submitted = true;

    if (!this.formGroup.valid) {
      return false;
    }

    var val = this.getDataFormGroup();
    val.dateQuotation = this.intlService.formatDate(val.dateQuotationObj, 'yyyy-MM-ddTHH:mm:ss');
    val.dateEndQuotation = this.intlService.formatDate(val.dateEndQuotationObj, 'yyyy-MM-ddTHH:mm:ss');

    val.payments.forEach(payment => {
      payment.date = this.intlService.formatDate(payment.dateObj, 'yyyy-MM-ddTHH:mm:ss');
    });

    if (this.quotationId) {

      this.quotationService.update(this.quotationId, val).subscribe(
        () => {
          // this.routeActive();
          this.loadRecord();
          this.notifyService.notify("success", "Cập nhật thành công");
          this.lineSelected = null;
        }, (error) => {
          this.loadRecord();
        });
    } else {
      this.quotationService.create(val).subscribe(
        (result: any) => {
          this.router.navigate(['quotations/form'], { queryParams: { id: result.id } });
          this.notifyService.notify("success", "Lưu thành công");
          this.lineSelected = null;
        }
      );
    }
  }

  onChangeDiscount(event, line: FormGroup) {
    line.value.discountType = event.discountType;
    if (event.discountType == "fixed") {
      line.value.discount = event.discount;
    } else {
      line.value.discount = event.discount;
    }
    line.patchValue(line.value);
  }

  addLine(val, addNew) {
    if (addNew && this.lineSelected) {
      this.notify('error', 'Vui lòng hoàn thành dịch vụ hiện tại để thêm dịch vụ khác');
      return;
    }

    var line = new QuotationLineDisplay();
    var id;
    if (!addNew) {
      line.id = val.id;
    }
    else {
      id = val.id;
    }
    line.diagnostic = val.diagnostic;
    line.discount = val.discount ? val.discount : 0;
    line.discountType = val.discountType ? val.discountType : 'percentage';
    line.productId = val.productId ? val.productId : id;
    line.qty = val.qty ? val.qty : 1;
    line.advisoryId = val.advisoryId;
    line.employee = val.employee;
    line.employeeId = val.employeeId;
    line.assistant = val.assistant;
    line.assistantId = val.assistantId;
    line.counselor = val.counselor;
    line.counselorId = val.counselorId;
    line.subPrice = val.subPrice ? val.subPrice : (val.listPrice ? val.listPrice : 0);
    line.name = val.name;
    line.amount = val.amount ? val.amount : (line.subPrice * line.qty);
    line.promotions = this.fb.array([]);
    line.toothType = val.toothType ? val.toothType : "manual";
    line.amountDiscountTotal = val.amountDiscountTotal ? val.amountDiscountTotal : 0;
    line.amountPromotionToOrder = val.amountPromotionToOrder ? val.amountPromotionToOrder : 0;
    line.amountPromotionToOrderLine = val.amountPromotionToOrderLine ? val.amountPromotionToOrderLine : 0;
    line.teeth = this.fb.array([]);
    if (val.teeth) {
      val.teeth.forEach(item => {
        line.teeth.push(this.fb.group(item))
      })
    }
    if (val.promotions) {
      val.promotions.forEach(item => {
        line.promotions.push(this.fb.group(item))
      });
    }

    line.toothCategory = val.toothCategory ? val.toothCategory : (this.filteredToothCategories ? this.filteredToothCategories[0] : null);
    line.toothCategoryId = val.toothCategoryId ? val.toothCategoryId : (this.filteredToothCategories && this.filteredToothCategories[0] ? this.filteredToothCategories[0].id : null);

    var res = this.fb.group(line);
    this.linesArray.push(res);
    this.linesArray.markAsDirty();
    this.createFormInfo(line);

    if (addNew) {
      this.lineSelected = res.value;
      // mặc định là trạng thái sửa
      setTimeout(() => {
        var viewChild = this.lineVCR.find(x => x.line == this.lineSelected);
        viewChild.onEditLine();
      }, 0);
    }
  }

  onEditLine(line) {
    if (this.lineSelected != null) {
      this.notify('error', 'Vui lòng hoàn thành dịch vụ hiện tại để chỉnh sửa dịch vụ khác');
    } else {
      this.lineSelected = line;
      var viewChild = this.lineVCR.find(x => x.line == line);
      viewChild.onEditLine();
    }
  }

  updateLineInfo(line, lineControl) {
    line.toothCategoryId = line.toothCategory.id;
    line.employeeId = line.employee ? line.employee.id : null;
    line.assistantId = line.assistant ? line.assistant.id : null;
    line.counselorId = line.counselor ? line.counselor.id : null;
    lineControl.patchValue(line);

    lineControl.get('teeth').clear();
    line.teeth.forEach(teeth => {
      let g = this.fb.group(teeth);
      lineControl.get('teeth').push(g);
    });

    lineControl.updateValueAndValidity();
    this.lineSelected = null;
  }

  onDeleteLine(index) {
    this.linesArray.removeAt(index);
    this.getAmountTotal();
    this.linesArray.markAsDirty();
    this.lineSelected = null;
  }

  onCancelEditLine(line) {
    this.lineSelected = null;
  }

  onUpdateOpenLinePromotion(line, lineControl, i) {
    // if(line.teeth.length == 0){
    //   var viewChild = this.lineVCR.find(x => x.line == this.lineSelected);
    //   viewChild.updateLineInfo();
    //   return;
    // }
    if (this.lineSelected != null) {
      var viewChild = this.lineVCR.find(x => x.line == this.lineSelected);
      // var checkValidChild = viewChild.checkValidFormGroup();
      // if (!checkValidChild)
      //   return;
      // else
      viewChild.updateLineInfo();
    }
    if (!this.isChanged) {
      this.onOpenLinePromotionDialog(i);
      return;
    }

    const val = this.getDataFormGroup();
    if (!this.quotationId) {
      this.submitted = true;
      if (!this.formGroup.valid) {
        return false;
      }

      this.quotationService.create(val).subscribe(async (result: any) => {
        this.quotationId = result.id;
        this.router.navigate(["/quotations/form"], {
          queryParams: { id: result.id },
        });
        await this.loadRecord();
        this.onOpenLinePromotionDialog(i);
      })
    } else {
      this.quotationService.update(this.quotationId, val).subscribe(async (result: any) => {
        await this.loadRecord();
        this.onOpenLinePromotionDialog(i);
      });
    }
  }

  async onOpenLinePromotionDialog(i) {
    let modalRef = this.modalService.open(QuotationLinePromotionDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
    modalRef.componentInstance.quotationLine = this.linesArray.controls[i].value;
    modalRef.componentInstance.getUpdateSJ().subscribe(async res => {
      var r = await this.loadRecord();
      modalRef.componentInstance.quotationLine = this.linesArray.controls[i].value;
    });

  }

  onOpenQuotationPromotion() {
    if (this.lineSelected != null) {
      var viewChild = this.lineVCR.find(x => x.line == this.lineSelected);
      // var checkValidChild = viewChild.checkValidFormGroup();
      // if (!checkValidChild)
      //   return;
      // else
      viewChild.updateLineInfo();
    }
    
    if (!this.isChanged) {
      this.openQuotationPromotionDialog();
      return;
    }
    
    const val = this.getDataFormGroup();
    
    if (!this.quotationId) {
      this.submitted = true;
      if (!this.formGroup.valid) {
        return false;
      }
      this.quotationService.create(val).subscribe(async (result: any) => {
        this.quotationId = result.id;
        this.quotation = result;
        this.quotation.promotions = [];

        this.router.navigate(["/quotations/form"], {
          queryParams: { id: result.id },
        });
        await this.loadRecord();
        this.openQuotationPromotionDialog();
      });
    } else {
      this.quotationService.update(this.quotationId, val).subscribe(async (result: any) => {
        await this.loadRecord();
        this.openQuotationPromotionDialog();
      });
    }
  }

  async loadRecord() {
    if (this.quotationId) {
      var result = await this.quotationService.get(this.quotationId).toPromise();
      this.patchValueQuotation(result);
      this.quotation = result;
      this.isChanged = true;
      return result;
    }
  }

  async openQuotationPromotionDialog() {
    let modalRef = this.modalService.open(QuotationPromotionDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
    modalRef.componentInstance.quotation = this.quotation;
    modalRef.componentInstance.getUpdateSJ().subscribe(async res => {
      var r = await this.loadRecord();
      modalRef.componentInstance.quotation = r;
    });
  }

  getAmount() {
    return (this.linesArray.value as any[]).reduce((total, cur) => {
      return total + cur.subPrice * cur.qty;
    }, 0);
  }

  getTotalDiscount() {
    var res = (this.linesArray.value as any[]).reduce((total, cur) => {
      return total + (cur.amountDiscountTotal || 0) * cur.qty;
    }, 0);
    return res;
  }

  sumPromotionQuotation() {
    if (this.quotationId && this.quotation.promotions) {
      return (this.quotation.promotions as any[]).reduce((total, cur) => {
        return total + cur.amount;
      }, 0);
    }
    return 0;
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

  onBlurPaymentInput(payment: FormGroup) {
    this.computeAmount(payment);
  }

  computeAmount(payment: FormGroup) {
    let amount = 0;
    if (payment.get('discountPercentType').value === 'cash') {
      amount = payment.get('payment') ? payment.get('payment').value : 0;
    }
    else {
      var percent = payment.get('payment') ? payment.get('payment').value : 0;
      amount = this.getAmountTotal() * (percent / 100);

    }
    payment.get('amount').patchValue(amount);
  }

  getValueFormPaymentArray(key, i) {
    return this.paymentsArray.at(i).get(key).value;
  }
}


