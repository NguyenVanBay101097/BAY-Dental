import { animate, state, style, transition, trigger } from '@angular/animations';
import { Component, OnInit, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { FormGroup, NgForm } from '@angular/forms';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { catchError, debounceTime, mergeMap, switchMap, tap } from 'rxjs/operators';
import { EmployeeBasic, EmployeePaged } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { PrintService } from 'src/app/shared/services/print.service';
import { ToothFilter, ToothService } from 'src/app/teeth/tooth.service';
import { ToothCategoryService } from 'src/app/tooth-categories/tooth-category.service';
import { QuotationLineCuComponent } from '../quotation-line-cu/quotation-line-cu.component';
import { QuotationLinePromotionDialogComponent } from '../quotation-line-promotion-dialog/quotation-line-promotion-dialog.component';
import { QuotationLineService } from '../quotation-line.service';
import { QuotationPromotionDialogComponent } from '../quotation-promotion-dialog/quotation-promotion-dialog.component';
import { QuotationPromotionService } from '../quotation-promotion.service';
import { PaymentQuotationDisplay, QuotationLineDisplay, QuotationsDisplay, QuotationService } from '../quotation.service';

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
  styleUrls: ['./quotation-create-update-form.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class QuotationCreateUpdateFormComponent implements OnInit {
  @ViewChild("empCbx", { static: true }) empCbx: ComboBoxComponent;
  formGroup: FormGroup;
  quotationId: string;
  partnerId: string;
  listTeeths: any[] = [];
  filteredToothCategories: any[] = [];
  quotation: QuotationsDisplay;
  filterEmployees: EmployeeBasic[] = [];
  sourceEmployees: EmployeeBasic[] = [];
  lineSelected = null;
  submitted: boolean = false;
  @ViewChildren('lineTemplate') lineVCR: QueryList<QuotationLineCuComponent>;

  isChanged: boolean = false;

  constructor(
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
    private notifyService: NotifyService,
    private quotationPromotionService: QuotationPromotionService,
    private quotationLineService: QuotationLineService
  ) { }

  ngOnInit() {
    this.quotation = <QuotationsDisplay>{
      dateQuotation: new Date(),
      dateApplies: 30,
      dateEndQuotation: new Date(),
      lines: [],
      payments: [],
      promotions: []
    };

    this.routeActive();

    this.loadTeethList();
    this.loadToothCategories();
    this.loadEmployees();

    this.empCbx.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.empCbx.loading = true)),
        switchMap((value) => this.searchEmployees(value))
      )
      .subscribe((result: any) => {
        this.filterEmployees = result.items;
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
          this.quotation = result;
        }
      )
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
      this.sourceEmployees = result.items;
      this.filterEmployees = this.sourceEmployees.slice();
    })
  }

  loadToothCategories() {
    this.toothCategoryService.getAll().subscribe((result: any[]) => {
      this.filteredToothCategories = result;
    });
  }

  loadTeethList() {
    var val = new ToothFilter();
    this.toothService.getAllBasic(val).subscribe((result: any[]) => {
      this.listTeeths = result;
    });
  }

  printQuotation() {
    if (this.quotationId) {
      this.quotationService.printQuotation(this.quotationId).subscribe((result: any) => {
        this.printService.printHtml(result.html);
      })
    }
  }

  onFilterEmployee(value) {
    this.filterEmployees = this.sourceEmployees
      .filter((s) => s.name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
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
    this.router.navigate(['quotations/form'], { queryParams: { partner_id: this.quotation.partner.id } });
  }

  onDateChange() {
    var date = this.quotation.dateQuotation;
    if (date) {
      var dateApplies = this.quotation.dateApplies || 0;
      var dateEnd = new Date(date.getFullYear(), date.getMonth(), date.getDate() + dateApplies);
      this.quotation.dateEndQuotation = dateEnd;
    } else {
      this.quotation.dateEndQuotation = null;
    }
  }

  //Payment 
  onAddPayment() {
    var payment = <PaymentQuotationDisplay>{
      payment: 0,
      discountPercentType: 'cash',
      amount: 0,
      date: new Date()
    }

    this.quotation.payments.push(payment);
  }

  deletePayment(index) {
    this.quotation.payments.splice(index, 1);
  }
  //end payment

  getDataFormGroup() {
    var val = {
      partnerId: this.quotation.partner.id,
      employeeId: this.quotation.employee.id,
      dateQuotation: this.intlService.formatDate(this.quotation.dateQuotation, 'yyyy-MM-ddTHH:mm:ss'),
      dateApplies: this.quotation.dateApplies,
      dateEndQuotation: this.intlService.formatDate(this.quotation.dateEndQuotation, 'yyyy-MM-ddTHH:mm:ss'),
      note: this.quotation.note,
      companyId: this.quotation.companyId,
      lines: this.quotation.lines.map(x => {
        return {
          id: x.id,
          name: x.name,
          productId: x.productId,
          subPrice: x.subPrice,
          qty: x.qty,
          employeeId: x.employee != null ? x.employee.id : null,
          assistantId: x.assistant != null ? x.assistant.id : null,
          counselorId: x.counselor != null ? x.counselor.id : null,
          toothIds: x.teeth.map(s => s.id),
          toothCategoryId: x.toothCategory != null ? x.toothCategory.id : null,
          diagnostic: x.diagnostic,
          toothType: x.toothType
        }
      }),
      payments: this.quotation.payments.map(x => {
        return {
          id: x.id,
          payment: x.payment,
          discountPercentType: x.discountPercentType,
          date: x.date,
          amount: x.amount,
        }
      }),
    };

    return val;
  }

  getAmountPayment(payment: PaymentQuotationDisplay) {
    if (payment.discountPercentType == 'cash') {
      return payment.payment;
    } else {
      var totalAmount = this.getAmountSubTotal() - this.getTotalDiscount();
      return (payment.payment / 100) * totalAmount;
    }
  }

  onSave(form: NgForm) {
    if (form.invalid) return false;

    if (this.lineSelected != null) {
      var viewChild = this.lineVCR.find(x => x.line == this.lineSelected);
      var rs = viewChild.updateLineInfo();
      if(!rs) return;
    }

    var val = this.getDataFormGroup();

    if (this.quotationId) {
      this.submitted = true;
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

  addLine(val) {
    if (this.lineSelected) {
      this.notify('error', 'Vui lòng hoàn thành dịch vụ hiện tại để thêm dịch vụ khác');
      return;
    }

    var toothCategory = this.filteredToothCategories[0];
    var value = <QuotationLineDisplay>{
      diagnostic: '',
      employee: null,
      employeeId: null,
      assistant: null,
      assistantId: null,
      name: val.name,
      amount: val.listPrice * 1,
      subPrice: val.listPrice,
      productId: val.id,
      product: {
        id: val.id,
        name: val.name
      },
      qty: 1,
      teeth: [],
      promotions: [],
      toothCategory: toothCategory,
      toothCategoryId: toothCategory.id,
      counselor: null,
      counselorId: null,
      toothType: 'manual',
      amountPromotionToOrder: 0,
      amountPromotionToOrderLine: 0,
      amountDiscountTotal: 0,
    };

    this.quotation.lines.push(value);
    this.lineSelected = value;

    // mặc định là trạng thái sửa
    setTimeout(() => {
      var viewChild = this.lineVCR.find(x => x.line == this.lineSelected);
      viewChild.onEditLine();
    }, 0);
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

  updateLineInfo(value, index) {
    var line = this.quotation.lines[index];
    Object.assign(line, value);
    this.computeAmountLine([line]);
    this.lineSelected = null;
  }

  computeAmountLine(lines) {
    lines.forEach(line => {
      line.amount = (line.subPrice - line.amountDiscountTotal) * line.qty;
    });
  }

  onDeleteLine(index) {
    var line = this.quotation.lines[index];
    if (line == this.lineSelected) {
      this.lineSelected = null;
    }
    this.quotation.lines.splice(index, 1);
  }

  onCancelEditLine(line) {
    this.lineSelected = null;
  }

  onUpdateOpenLinePromotion(line, lineControl, i) {
    if (this.lineSelected != null) {
      var viewChild = this.lineVCR.find(x => x.line == this.lineSelected);
      var rs = viewChild.updateLineInfo();
      if (!rs) return;
    }
   
    const val = this.getDataFormGroup();
    this.submitted = true;
    if (!this.quotationId) {
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
    var line = this.quotation.lines[i];
    let modalRef = this.modalService.open(QuotationLinePromotionDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
    modalRef.componentInstance.quotationLine = line;
    modalRef.componentInstance.partnerId = this.quotation.partner.id;
    modalRef.componentInstance.getBtnDiscountObs().subscribe(data => {
      var val = {
        id: line.id,
        discountType: data.discountType,
        discountPercent: data.discountPercent,
        discountFixed: data.discountFixed,
      };

      this.quotationLineService.applyDiscountOnQuotationLine(val).pipe(
        mergeMap(() => this.quotationService.get(this.quotationId))
      )
        .subscribe(res => {
          this.quotation = res;
          var newLine = this.quotation.lines[i];
          modalRef.componentInstance.quotationLine = newLine;
        });
    });

    modalRef.componentInstance.getBtnPromoCodeObs().subscribe(data => {
      var val = {
        id: line.id,
        couponCode: data.couponCode
      };

      this.quotationLineService.applyPromotionUsageCode(val).pipe(
        mergeMap((result: any) => {
          if (!result.success) {
            throw result;
          }
          return this.quotationService.get(this.quotationId);
        })
      )
        .subscribe(res => {
          this.quotation = res;
          var newLine = this.quotation.lines[i];
          modalRef.componentInstance.quotationLine = newLine;
        }, err => {
          this.notify('error', err.error);
        });
    });

    modalRef.componentInstance.getBtnPromoNoCodeObs().subscribe(data => {
      var val = {
        id: line.id,
        saleProgramId: data.id
      };

      this.quotationLineService.applyPromotion(val).pipe(
        catchError((err) => { throw err; }),
        mergeMap((result: any) => {
          return this.quotationService.get(this.quotationId);
        })
      )
        .subscribe(res => {
          this.quotation = res;
          var newLine = this.quotation.lines[i];
          modalRef.componentInstance.quotationLine = newLine;
        });
    });

    modalRef.componentInstance.getBtnDeletePromoObs().subscribe(data => {
      this.quotationPromotionService.removePromotion([data.id]).pipe(
        catchError((err) => { throw err; }),
        mergeMap((result: any) => {
          return this.quotationService.get(this.quotationId);
        })
      )
        .subscribe(res => {
          this.quotation = res;
          var newLine = this.quotation.lines[i];
          modalRef.componentInstance.quotationLine = newLine;
        }, err => {
          this.notify('error', err.error.error);
        });
    });

  }

  onOpenQuotationPromotion() {
    if (this.lineSelected != null) {
      var viewChild = this.lineVCR.find(x => x.line == this.lineSelected);
      var rs  = viewChild.updateLineInfo();
      if(!rs) return;
    }

    this.submitted = true;
    const val = this.getDataFormGroup();

    if (!this.quotationId) {
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
      this.quotation = result;
      return result;
    }
  }

  async openQuotationPromotionDialog() {
    let modalRef = this.modalService.open(QuotationPromotionDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static', scrollable: true });
    modalRef.componentInstance.quotation = this.quotation;
    modalRef.componentInstance.getBtnDiscountObs().subscribe(data => {
      var val = {
        id: this.quotation.id,
        discountType: data.discountType,
        discountPercent: data.discountPercent,
        discountFixed: data.discountFixed,
      };

      this.quotationService.applyDiscountOnQuotation(val).pipe(
        mergeMap((result: any) => {
          return this.quotationService.get(this.quotationId);
        })
      )
        .subscribe(res => {
          debugger
          this.quotation = res;
          modalRef.componentInstance.quotation = this.quotation;
        });
    });

    modalRef.componentInstance.getBtnPromoCodeObs().subscribe(data => {
      var val = {
        id: this.quotation.id,
        couponCode: data.couponCode
      };

      this.quotationService.applyCouponOnQuotation(val).pipe(
        mergeMap((result: any) => {
          if (!result.success) {
            throw result;
          }
          return this.quotationService.get(this.quotationId);
        })
      )
        .subscribe(res => {
          this.quotation = res;
          modalRef.componentInstance.quotation = this.quotation;
        }, err => {
          this.notify('error', err.error);
        });
    });

    modalRef.componentInstance.getBtnPromoNoCodeObs().subscribe(data => {
      var val = {
        id: this.quotation.id,
        saleProgramId: data.id
      };

      this.quotationService.applyPromotion(val).pipe(
        catchError((err) => { throw err; }),
        mergeMap((result: any) => {
          return this.quotationService.get(this.quotationId);
        })
      )
        .subscribe(res => {
          this.quotation = res;
          modalRef.componentInstance.quotation = this.quotation;
        }, err => {
          this.notify('error', err.error.error);
        });
    });

    modalRef.componentInstance.getBtnDeletePromoObs().subscribe(data => {
      this.quotationPromotionService.removePromotion([data.id]).pipe(
        catchError((err) => { throw err; }),
        mergeMap((result: any) => {
          return this.quotationService.get(this.quotationId);
        })
      )
        .subscribe(res => {
          this.quotation = res;
          modalRef.componentInstance.quotation = this.quotation;
        }, err => {
          this.notify('error', err.error.error);
        });
    });
  }

  getAmountSubTotal() {
    return this.quotation.lines.reduce((total, cur) => {
      return total + cur.subPrice * cur.qty;
    }, 0);
  }

  getTotalDiscount() {
    return this.quotation.totalAmountDiscount;
    // var res = this.quotation.lines.reduce((total, cur) => {
    //   return total + (cur.amountDiscountTotal || 0) * cur.qty;
    // }, 0);

    // return res;
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
}


