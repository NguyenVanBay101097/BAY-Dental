import { AfterViewChecked, AfterViewInit, ChangeDetectorRef, Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import * as moment from 'moment';
import { Observable } from 'rxjs';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { ResInsurancePaymentService } from 'src/app/res-insurance/res-insurance-payment.service';
import { ResInsurancePaged, ResInsuranceSimple } from 'src/app/res-insurance/res-insurance.model';
import { ResInsuranceService } from 'src/app/res-insurance/res-insurance.service';
import { ResInsuranceCuDialogComponent } from 'src/app/shared/res-insurance-cu-dialog/res-insurance-cu-dialog.component';
import { NotifyService } from 'src/app/shared/services/notify.service';

@Component({
  selector: 'app-sale-order-insurance-payment-dialog',
  templateUrl: './sale-order-insurance-payment-dialog.component.html',
  styleUrls: ['./sale-order-insurance-payment-dialog.component.css']
})
export class SaleOrderInsurancePaymentDialogComponent implements OnInit, AfterViewInit, AfterViewChecked {
  @Input() title: string;
  @Input() defaultValue: any;
  @Input() saleOrderId: string;
  @ViewChild("insuranceCbx", { static: false }) insuranceCbx: ComboBoxComponent;

  formGroup: FormGroup;
  insuranceFilter: ResInsuranceSimple[] = [];
  submitted: boolean = false;
  // amountPaid: number = 0;
  constructor(
    private fb: FormBuilder,
    private activeModal: NgbActiveModal,
    private modalService: NgbModal,
    private notifyService: NotifyService,
    private resInsuranceService: ResInsuranceService,
    private resInsurancePaymentService: ResInsurancePaymentService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.formGroup = this.fb.group({
      insurance: [null, Validators.required],
      amount: 0,
      dateObj: [null, Validators.required],
      lines: this.fb.array([]),
      note: '',
    });

    if (this.defaultValue) {
      this.loadDefaultValue(this.defaultValue);
    }

    this.loadInsuranceFilter();
  }

  ngAfterViewInit(): void {
    this.insuranceCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.insuranceCbx.loading = true)),
      switchMap((value) => this.searchInsurance(value))
    ).subscribe((res: any) => {
      this.insuranceFilter = res;
      this.insuranceCbx.loading = false;
    });
  }
  
  ngAfterViewChecked(): void {
    this.cdr.detectChanges();
  }

  get f() {
    return this.formGroup.controls;
  }

  get linesFC() {
    return this.f.lines as FormArray;
  }

  lineFCAt(index, key) {
    return this.linesFC.at(index).get(key);
  }

  loadDefaultValue(data: any): void {
    this.formGroup.patchValue(
      { amount: data.amount, note: data.note, dateObj: new Date(data.date) }
    );

    this.linesFC.clear();
    data.lines.forEach((line) => {
      line.saleOrderLine.amountPaid = 0;
      line.saleOrderLine.amountResidual = 0;
      const g = this.fb.group(line);
      this.linesFC.push(g);
    });

    this.formGroup.markAsPristine();
  }

  searchInsurance(q?: string): Observable<ResInsuranceSimple> {
    let val = new ResInsurancePaged();
    val.limit = 20;
    val.offset = 0;
    val.search = q || '';
    val.isActive = 'true';
    return this.resInsuranceService.autoComplete(val);
  }

  loadInsuranceFilter() {
    this.searchInsurance().subscribe((res: any) => {
      this.insuranceFilter = res;
    });
  }

  actionPayment(): void {
    let data = this.formGroup.value;
    const userInfo = JSON.parse(localStorage.getItem("user_info"));
    const companyId = userInfo.companyId;
    data.date = moment(data.dateObj).format('YYYY-MM-DD');
    data.resInsuranceId = data.insurance ? data.insurance.id : '';
    data.companyId = companyId;
    data.orderId = this.saleOrderId || '';
    this.resInsurancePaymentService.create(data).subscribe((res: any) => {
      this.resInsurancePaymentService.actionPayment([res.id]).subscribe(() => {
        this.activeModal.close(res);
      }, (error) => console.log(error));
    }, (error) => console.log(error));
  }

  quickCreateInsurance(): void {
    let modalRef = this.modalService.open(ResInsuranceCuDialogComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal modal-appointment', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm công ty bảo hiểm';
    modalRef.result.then((result) => {
      this.notifyService.notify("success", "Lưu thành công");
      this.insuranceFilter.push(result as ResInsuranceSimple);
      this.formGroup.patchValue({ insurance: result });
    }, () => { });
  }

  changePayType(index): void {
    const lineData = this.linesFC.at(index).value;
    if (lineData.payType === 'fixed') {
      this.lineFCAt(index, 'fixedAmount').setValue(0);
    } else {
      this.lineFCAt(index, 'percent').setValue(0);
    }
  }

  getAmountPaid(lineData, index): number {
    let amountPaid = 0;
    if (lineData.payType === 'percent') {
      amountPaid = (lineData.percent / 100) * lineData.saleOrderLine.priceTotal;
    } else {
      amountPaid = lineData.fixedAmount;
    }
    this.lineFCAt(index, 'saleOrderLine').value.amountPaid = amountPaid;
    return amountPaid;
  }

  getAmountResidual(lineData, index): number {
    const amountResidual = lineData.saleOrderLine.priceTotal - this.getAmountPaid(lineData, index);
    this.lineFCAt(index, 'saleOrderLine').value.amountResidual = amountResidual;
    return amountResidual;
  }

  getAmountTotal(): number {
    return this.linesFC.value.reduce((val, curr) => {
      return val + curr.saleOrderLine.amountPaid;
    }, 0);
  }

  onBlurPercent(index): void {
    let percent = this.lineFCAt(index, 'percent').value;
    if (!percent) {
      this.lineFCAt(index, 'percent').setValue(0);
    }
  }

  onBlurfixed(index): void {
    let fixedAmount = this.lineFCAt(index, 'fixedAmount').value;
    if (!fixedAmount) {
      this.lineFCAt(index, 'fixedAmount').setValue(0);
    }
  }

  onCancel(): void {
    this.activeModal.dismiss();
  }
}
