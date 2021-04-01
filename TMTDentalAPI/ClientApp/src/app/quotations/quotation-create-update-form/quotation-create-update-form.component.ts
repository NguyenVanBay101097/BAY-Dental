import { animate, state, style, transition, trigger } from '@angular/animations';
import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { switchMap } from 'rxjs/operators';
import { ToothDisplay, ToothFilter, ToothService } from 'src/app/teeth/tooth.service';
import { ToothCategoryBasic, ToothCategoryService } from 'src/app/tooth-categories/tooth-category.service';
import { QuotationLineDisplay, QuotationService } from '../quotation.service';

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
  formGroup: FormGroup;
  formGroupInfo: FormGroup;
  partner: any;
  user: any;
  dateFrom: Date;
  dateTo: Date;
  toothCategoryId: string;
  partnerId: string;
  quotationId: string;
  quotationLine: any;
  public selectedLine: any;
  hamList: { [key: string]: {} };
  teethSelected: any[] = [];
  filteredToothCategories: any[] = [];
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());

  constructor(
    private fb: FormBuilder,
    private activatedRoute: ActivatedRoute,
    private quotationService: QuotationService,
    private toothService: ToothService,
    private toothCategoryService: ToothCategoryService,
    private notificationService: NotificationService,
    private intlService: IntlService,

  ) { }

  ngOnInit() {
    // debugger;
    this.formGroup = this.fb.group({
      partnerId: ['', Validators.required],
      userId: ['', Validators.required],
      note: '',
      dateQuotation: [null, Validators.required],
      dateApplies: [0, Validators.required],
      dateEndQuotation: '',
      lines: this.fb.array([
      ]),
      payments: this.fb.array([]),
    })
    this.routeActive();
    this.loadToothCategories();
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
          debugger
          this.partner = result.partner;
          this.user = result.user;
          this.formGroup.get('note').setValue(result.note);
          this.formGroup.get('partnerId').setValue(result.partnerId);
          this.formGroup.get('userId').setValue(result.userId);
          this.formGroup.get('dateQuotation').setValue(new Date(result.dateQuotation));
          this.formGroup.get('dateEndQuotation').setValue(this.intlService.formatDate(new Date(result.dateEndQuotation), "dd/MM/yyyy"));
          this.formGroup.get('dateApplies').setValue(result.dateApplies)
          const control = this.formGroup.get('lines') as FormArray;
          control.clear();

          result.lines.forEach(line => {
            this.addLine(line);
          });

          const paymentcontrol = this.formGroup.get('payments') as FormArray;
          paymentcontrol.clear();

          result.payments.forEach(payment => {
            var g = this.fb.group(payment);
            paymentcontrol.push(g);
          });

          this.formGroup.markAsPristine();
        }
      )
  }



  addLine(val) {
    var line = new QuotationLineDisplay();
    line.diagnostic = val.Diagnostic;
    line.percentDiscount = 0;
    line.product = val.Product;
    line.productId = val.ProductId;
    line.qty = 1;
    line.subPrice = val.PriceUnit;
    line.productName = val.Name;
    line.teeth = this.fb.array([]);
    line.toothCategory = this.filteredToothCategories ? this.filteredToothCategories[0] : null;
    line.toothCategoryId = this.filteredToothCategories && this.filteredToothCategories[0] ? this.filteredToothCategories[0].id : null;
    var res = this.fb.group(line);
    this.linesArray.push(res);
    this.linesArray.markAsDirty();
    // this.getPriceSubTotal();
    // this.computeAmountTotal();
    // this.quotationLine = null;
  }

  onChangeQuantity(line: FormGroup) {
    var res = this.linesArray.controls.find(x => x.value.productId === line.value.productId);
    if (res) {
      res.patchValue(line.value);
    }
    // this.getPriceSubTotal();
    // this.computeAmountTotal();

  }

  getPriceSubTotal() {
    this.linesArray.controls.forEach(line => {
      var discountType = line.get('discountType') ? line.get('discountType').value : 'percentage';
      var discountFixedValue = line.get('discountFixed') ? line.get('discountFixed').value : 0;
      var discountNumber = line.get('discount') ? line.get('discount').value : 0;
      var getquanTity = line.get('productUOMQty') ? line.get('productUOMQty').value : 1;
      var getamountPaid = line.get('amountPaid') ? line.get('amountPaid').value : 0;
      var priceUnit = line.get('priceUnit') ? line.get('priceUnit').value : 0;
      var price = priceUnit * getquanTity;

      var subtotal = discountType == 'percentage' ? price * (1 - discountNumber / 100) :
        Math.max(0, price - discountFixedValue);
      // line.get('priceSubTotal').setValue(subtotal);
      var getResidual = subtotal - getamountPaid;
      // line.get('amountResidual').setValue(getResidual);
    });

  }

  computeAmountTotal() {
    let total = 0;
    // this.linesArray.controls.forEach(line => {
    //   console.log(total);
    //   total += line.get('priceSubTotal').value;
    // });
    // // this.computeResidual(total);
    // this.formGroup.get('amountTotal').patchValue(total);
  }

  computeTotalPrice(line: FormGroup) {
    let price = line.get('subPrice') ? line.get('subPrice').value : 0;
    let qty = line.get('qty') ? line.get('qty').value : 1;
    let discount = line.get('percentDiscount') ? line.get('percentDiscount').value : 0;
    let totalPrice = price * qty * (1 - discount / 100);
    return totalPrice;
  }

  getAmountTotal() {
    let totalAmount = 0;
    var lines = this.formGroup.get('lines').value;
    if (lines && lines.length > 0) {
      lines.forEach(line => {
        totalAmount += line.subPrice * line.qty * (1 - line.percentDiscount / 100);
      });
    }
    return totalAmount;
  }

  get linesArray() {
    return this.formGroup.get('lines') as FormArray;
  }

  get paymentsArray() {
    return this.formGroup.get('payments') as FormArray;
  }

  getDate(dateQuotation: Date, dateApplies: number) {
    var dateEnd = new Date(dateQuotation.getFullYear(), dateQuotation.getMonth(), dateQuotation.getDate() + dateApplies);
    this.formGroup.get('dateEndQuotation').patchValue(this.intlService.formatDate(dateEnd, "dd/MM/yyyy"));
  }

  onDateChange(date: Date) {
    let dateAppliesChange = this.formGroup.get('dateApplies') ? this.formGroup.get('dateApplies').value : null;
    if (date && dateAppliesChange) {
      this.getDate(date, dateAppliesChange);
    }
  }

  onDateAppliesChange(dateApplies) {
    let dateQuotation = this.formGroup.get('dateQuotation') ? this.formGroup.get('dateQuotation').value : null;
    if (dateQuotation && dateApplies) {
      this.getDate(dateQuotation, dateApplies);
    }
  }

  deleteLine(index: number) {
    this.linesArray.removeAt(index);
    this.getAmountTotal();
    this.linesArray.markAsDirty();
  }

  createFormInfo(data: any) {
    this.formGroupInfo = this.fb.group({
      toothCategory: data ? data.toothCategory : null,
      toothCategoryId: data ? data.toothCategoryId : '',
      diagnostic: data ? data.diagnostic : ''
    })
    this.loadTeethMap(data.toothCategory);
    if (data.teeth) {
      this.teethSelected = Object.assign([], data.teeth);
    }
  }

  updateLineInfo(lineControl) {
    var line = this.formGroupInfo.value;
    line.teeth = this.teethSelected;
    line.qty = (line.teeth && line.teeth.length > 0) ? line.teeth.length : 1;
    lineControl.patchValue(line);
    lineControl.get('teeth').clear();
    line.teeth.forEach(teeth => {
      let g = this.fb.group(teeth);
      lineControl.get('teeth').push(g);
    });
    this.onChangeQuantity(lineControl);
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
      discountPercent: 0,
      amount: 0,
      sequence: 1,
      date: new Date()
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
    value.dateQuotation = this.intlService.formatDate(value.dateQuotation, "yyyy-MM-dd");
    if (value.lines) {
      value.lines.forEach(line => {
        if (line.teeth) {
          line.toothIds = line.teeth.map(x => x.id)
        }
      });
    }
    if (value.payments) {
      value.payments.forEach(pm => {
        pm.date = this.intlService.formatDate(pm.date, "yyyy-MM-dd");
      });
    }
    return value;
  }

  onSave() {
    var val = this.getDataFormGroup();
    if (this.quotationId) {
      this.quotationService.update(this.quotationId, val).subscribe(
        () => {
          alert("Thành công rồi em ơi")
        }
      );
    } else {
      this.quotationService.create(val).subscribe(
        () => {
          alert("Thành công rồi em ơi")
        }
      );
    }
  }
}


