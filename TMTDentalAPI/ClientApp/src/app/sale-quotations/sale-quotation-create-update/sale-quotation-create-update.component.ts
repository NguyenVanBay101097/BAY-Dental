import { Component, OnInit, ViewChild } from "@angular/core";
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import * as _ from 'lodash';
import { from } from 'rxjs';
import { debounceTime, delay, map, switchMap, tap } from 'rxjs/operators';
import { EmployeesOdataService } from 'src/app/shared/services/employeeOdata.service';
import { SaleOrdersOdataService } from 'src/app/shared/services/sale-ordersOdata.service';
import { ToothCategoryOdataService } from 'src/app/shared/services/tooth-categoryOdata.service';
import { TeethOdataService } from 'src/app/shared/services/toothOdata.service';
import { UserOdataService, UserSimple } from 'src/app/shared/services/user-odata.service';

@Component({
  selector: "app-sale-quotation-create-update",
  templateUrl: "./sale-quotation-create-update.component.html",
  styleUrls: ["./sale-quotation-create-update.component.css"],
  host: {
    class: "o_action o_view_controller",
  },
})
export class SaleQuotationCreateUpdateComponent implements OnInit {
  // các source để filter
  sourceEmployees: any = [];
  //khai báo các biến
  formGroup: FormGroup;
  saleOrderId: string;
  saleOrder: any = {};
  partnerId: string;
  filteredToothCategories: any[];
  initialListTeeths: any[];
  filteredEmployees: any[] = [];

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private saleOrderService: SaleOrdersOdataService,
    private intlService: IntlService,
    private router: Router,
    private notificationService: NotificationService,
    private toothCategoryOdataService: ToothCategoryOdataService,
    private teethOdataService: TeethOdataService,
    private employeeService: EmployeesOdataService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      Partner: [null, Validators.required],
      User: null,
      UserId: null,
      dateOrderObj: [null, Validators.required],
      OrderLines: this.fb.array([]),
      CompanyId: null,
      AmountTotal: 0,
      State: null,
      Residual: null,
      Card: null,
      Pricelist: [null],
      IsQuotation: true,
      Order: null
    });

    this.routeActive();
    this.loadToothCategories();
    this.loadTeethList();
    this.loadEmployees();
  }

  get partner() { return this.formGroup.get('Partner').value }
  get order() { return this.formGroup.get('Order').value }

  get customerId() {
    if (this.partnerId) {
      return this.partnerId
    }
    if (this.saleOrder) {
      return this.saleOrder.PartnerId;
    }
    return undefined;
  }

  get getAmountTotal() {
    return this.formGroup.get('AmountTotal').value;
  }

  get orderLinesFA() {
    return this.formGroup.get('OrderLines') as FormArray;
  }

  get stateControl() { return this.formGroup.get("State"); }

  routeActive() {
    this.route.queryParamMap
      .pipe(
        switchMap((params: ParamMap) => {
          this.saleOrderId = params.get("id");
          this.partnerId = params.get("partner_id");
          if (this.saleOrderId) {
            return this.saleOrderService.getDisplay(this.saleOrderId);
          } else {
            return this.saleOrderService.defaultGet({ PartnerId: this.partnerId || '', IsQuotation: true });
          }
        })
      )
      .subscribe((result: any) => {
        this.saleOrder = result;
        this.formGroup.patchValue(result);
        let dateOrder = new Date(result.DateOrder);
        this.formGroup.get("dateOrderObj").patchValue(dateOrder);

        if (result.Employee) {
          this.filteredEmployees = _.unionBy(this.filteredEmployees, [result.Employee], 'Id');
        }

        const control = this.formGroup.get('OrderLines') as FormArray;
        control.clear();
        result.OrderLines.forEach(line => {
          var g = this.fb.group(line);
          g.setControl('Teeth', this.fb.array(line.Teeth));
          control.push(g);
        });
        this.formGroup.markAsPristine();
      });
  }

  onEmployeeFilter(value) {
    this.filteredEmployees = this.sourceEmployees.filter((s) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1).slice(0, 20);
  }

  loadEmployees() {
    const state = {
      filter: {
        logic: 'and',
        filters: [
          { field: 'IsDoctor ', operator: 'eq', value: true },
          { field: 'Active ', operator: 'eq', value: true }
        ]
      }
    };
    const options = {
      select: 'Id,Name'
    };
    this.employeeService.getFetch(state, options).subscribe(
      (result: any) => {
        this.sourceEmployees = result.data;
        this.filteredEmployees = this.sourceEmployees.slice(0, 20);
      }
    );
  }

  loadTeethList() {
    const options = {
      select: 'Id,Name,CategoryId,ViTriHam,Position'
    };
    this.teethOdataService.getFetch({}, options).subscribe(
      (result: any) => {
        this.initialListTeeths = result.data;
      }
    );
  }

  loadToothCategories() {
    const options = {
      select: 'Id,Name,Sequence'
    };
    this.toothCategoryOdataService.getFetch({}, options).subscribe(
      (result: any) => {
        this.filteredToothCategories = result.data;
      }
    );
  }

  getFormDataSave() {
    const val = Object.assign({}, this.formGroup.value);
    val.DateOrder = this.intlService.formatDate(val.dateOrderObj, 'yyyy-MM-ddTHH:mm:ss');
    val.PartnerId = val.Partner.Id;
    val.pricelistId = val.Pricelist ? val.Pricelist.Id : null;
    val.UserId = val.User ? val.User.Id : null;
    val.CardId = val.card ? val.card.id : null;
    val.OrderLines.forEach(line => {
      if (line.Employee) {
        line.EmployeeId = line.Employee.Id;
      }
      if (line.Teeth) {
        line.ToothIds = line.Teeth.map(x => x.Id);
      }
    });
    return val;
  }

  createRecord() {
    const val = this.getFormDataSave();
    return this.saleOrderService.create(val);
  }

  saveRecord() {
    var val = this.getFormDataSave();
    return this.saleOrderService.update(this.saleOrderId, val);
  }

  createNew() {
    if (this.customerId) {
      this.router.navigate(['/sale-quotations/form'], { queryParams: { partner_id: this.customerId } });
    }
  }

  actionConvertToOrder() {
    if (this.saleOrderId) {
      this.saleOrderService.actionConvertToOrder(this.saleOrderId).subscribe((result: any) => {
        this.router.navigate(['/sale-orders/form'], { queryParams: { id: result.Id } });
      });
    }
  }

  onSave() {
    if (!this.formGroup.valid) {
      return false;
    }

    const val = this.getFormDataSave();
    if (this.saleOrderId) {
      this.saleOrderService.update(this.saleOrderId, val).subscribe(() => {
        this.notify('success', 'Lưu thành công');
        this.loadRecord();
      });
    } else {
      this.saleOrderService.create(val).subscribe((result: any) => {
        this.router.navigate(["/sale-quotations/form"], {
          queryParams: { id: result.Id },
        });
      });
    }
  }

  getPriceSubTotal() {
    this.orderLinesFA.controls.forEach(line => {
      var discountType = line.get('DiscountType') ? line.get('DiscountType').value : 'percentage';
      var discountFixedValue = line.get('DiscountFixed') ? line.get('DiscountFixed').value : 0;
      var discountNumber = line.get('Discount') ? line.get('Discount').value : 0;
      var getquanTity = line.get('ProductUOMQty') ? line.get('ProductUOMQty').value : 1;
      var getamountPaid = line.get('AmountPaid') ? line.get('AmountPaid').value : 0;
      var priceUnit = line.get('PriceUnit') ? line.get('PriceUnit').value : 0;
      var price = priceUnit * getquanTity;

      var subtotal = discountType == 'percentage' ? price * (1 - discountNumber / 100) :
        Math.max(0, price - discountFixedValue);
      line.get('PriceSubTotal').setValue(subtotal);
      var getResidual = subtotal - getamountPaid;
      line.get('AmountResidual').setValue(getResidual);
    });

  }

  addLine(val) {
    var res = this.fb.group(val);
    if (!this.orderLinesFA.controls.some(x => x.value.ProductId === res.value.ProductId)) {
      this.orderLinesFA.push(res);
    } else {
      var line = this.orderLinesFA.controls.find(x => x.value.ProductId === res.value.ProductId);
      if (line) {
        line.value.ProductUOMQty += 1;
        line.patchValue(line.value);
      }
    }
    this.getPriceSubTotal();
    this.orderLinesFA.markAsDirty();
    this.computeAmountTotal();
    if (this.formGroup.get('State').value == "sale") {
      var val = this.getFormDataSave();
      this.saleOrderService.update(this.saleOrderId, val).subscribe(() => {
        this.notify('success', 'Lưu thành công');
        this.loadRecord();
      }, () => {
        this.loadRecord();
      });
    }
  }

  loadRecord() {
    if (this.saleOrderId) {
      this.saleOrderService.getDisplay(this.saleOrderId).subscribe((result: any) => {
        this.saleOrder = result;
        this.formGroup.patchValue(result);
        let dateOrder = new Date(result.DateOrder);
        this.formGroup.get('dateOrderObj').patchValue(dateOrder);

        if (result.Employee) {
          this.filteredEmployees = _.unionBy(this.filteredEmployees, [result.Employee], 'Id');
        }

        let control = this.formGroup.get('OrderLines') as FormArray;
        control.clear();
        result.OrderLines.forEach(line => {
          var g = this.fb.group(line);
          g.setControl('Teeth', this.fb.array(line.Teeth));
          control.push(g);
        });

        this.formGroup.markAsPristine();
      });
    }
  }

  deleteLine(index: number) {
    if (this.formGroup.get('State').value == "draft" || this.formGroup.get('State').value == "cancel") {
      this.orderLinesFA.removeAt(index);
      this.computeAmountTotal();
      this.orderLinesFA.markAsDirty();
    } else {
      this.notificationService.show({
        content: 'Chỉ có thể xóa dịch vụ khi phiếu điều trị ở trạng thái nháp hoặc hủy bỏ',
        hideAfter: 5000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });
    }
  }

  computeAmountTotal() {
    let total = 0;
    this.orderLinesFA.controls.forEach(line => {
      total += line.get('PriceSubTotal').value;
    });
    this.formGroup.get('AmountTotal').patchValue(total);
  }

  updateTeeth(line, lineControl) {
    line.ProductUOMQty = (line.Teeth && line.Teeth.length > 0) ? line.Teeth.length : 1;
    lineControl.patchValue(line);
    lineControl.get('Teeth').clear();
    line.Teeth.forEach(teeth => {
      let g = this.fb.group(teeth);
      lineControl.get('Teeth').push(g);
    });
    this.onChangeQuantity(lineControl);
  }

  onChangeQuantity(line: FormGroup) {
    var res = this.orderLinesFA.controls.find(x => x.value.ProductId === line.value.ProductId);
    if (res) {
      res.patchValue(line.value);
    }
    this.getPriceSubTotal();
    this.computeAmountTotal();

  }

  getDiscountNumber(line: FormGroup) {
    var discountType = line.get('DiscountType') ? line.get('DiscountType').value : 'percentage';
    if (discountType == 'fixed') {
      return line.get('DiscountFixed').value;
    } else {
      return line.get('Discount').value;
    }
  }

  getDiscountTypeDisplay(line: FormGroup) {
    var discountType = line.get('DiscountType') ? line.get('DiscountType').value : 'percentage';
    if (discountType == 'fixed') {
      return "";
    } else {
      return '%';
    }
  }

  onChangeDiscount(event, line: FormGroup) {
    var res = this.orderLinesFA.controls.find(x => x.value.ProductId === line.value.ProductId);
    if (res) {
      line.value.DiscountType = event.DiscountType;
      if (event.DiscountType == "fixed") {
        line.value.DiscountFixed = event.DiscountFixed;
      } else {
        line.value.Discount = event.DiscountPercent;
      }
      res.patchValue(line.value);
    }
    this.getPriceSubTotal();
    this.computeAmountTotal();
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
