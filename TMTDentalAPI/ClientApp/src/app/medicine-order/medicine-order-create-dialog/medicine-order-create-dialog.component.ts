import { Component, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { AccountJournalFilter, AccountJournalService, AccountJournalSimple } from 'src/app/account-journals/account-journal.service';
import { AuthService } from 'src/app/auth/auth.service';
import { MedicineOrderService, PrecscriptPaymentDisplay, PrecsriptionPaymentSave } from '../medicine-order.service';
import { PrintService } from 'src/app/shared/services/print.service';
import { Router } from '@angular/router';
import { PageChangeEvent } from '@progress/kendo-angular-grid';
import { NotificationService } from '@progress/kendo-angular-notification';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-medicine-order-create-dialog',
  templateUrl: './medicine-order-create-dialog.component.html',
  styleUrls: ['./medicine-order-create-dialog.component.css']
})
export class MedicineOrderCreateDialogComponent implements OnInit {
  @ViewChild('journalCbx', { static: true }) journalCbx: ComboBoxComponent;
  idToaThuoc: string;
  precscriptPayment: PrecscriptPaymentDisplay = new PrecscriptPaymentDisplay();
  filteredJournals: AccountJournalSimple[];

  formGroup: FormGroup;
  title: string;
  id: string;

  submitted = false;

  get f() { return this.formGroup.controls; }

  constructor(
    private accountJournalService: AccountJournalService,
    private fb: FormBuilder,
    private authService: AuthService,
    private printService: PrintService,
    public activeModal: NgbActiveModal,
    private medicineOrderService: MedicineOrderService,
    private intlService: IntlService,
    private router: Router,
    private notificationService: NotificationService,
    private modalService: NgbModal
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      journal: [null, Validators.required],
      orderDate: [new Date(), Validators.required],
      note: '',
      amount: 0,
      medicineOrderLines: this.fb.array([])
    });

    setTimeout(() => {
      this.journalCbx.filterChange.asObservable().pipe(
        debounceTime(300),
        tap(() => (this.journalCbx.loading = true)),
        switchMap(value => this.searchJournals(value))
      ).subscribe(result => {
        this.filteredJournals = result;
        this.journalCbx.loading = false;
      });

      this.loadFilteredJournals();
  
      if (this.idToaThuoc) {
        this.getDefault();
      } else if (this.id) {
        this.loadRecord();
      }
    });
  }

  loadRecord() {
    this.medicineOrderService.getDisplay(this.id).subscribe(
      result => {
        if (result) {
          console.log(result);
          result.orderDate = this.intlService.formatDate(new Date(result.orderDate), "dd/MM/yyyy")
          this.precscriptPayment = result;
          this.loadData(result);
        }
      }
    )
  }

  removeLine(line: FormGroup) {
    if (this.medicineOrderLines) {
      var index = this.medicineOrderLines.controls.findIndex(x => x.value.toaThuocLineId == line.value.toaThuocLineId);
      if (index >= 0) {
        this.medicineOrderLines.controls.splice(index, 1);
      }
    }
    this.computeTotalAmount();
  }

  loadData(precscriptPayment: PrecscriptPaymentDisplay) {
    if (precscriptPayment.medicineOrderLines) {
      var control = this.formGroup.get('medicineOrderLines') as FormArray;
      control.clear();
      var lines = this.precscriptPayment.medicineOrderLines;
      console.log(lines);
      lines.forEach(line => {
        control.push(this.fb.group(line));
      });
      this.computeTotalAmount();
    }
  }

  getDefault() {
    this.medicineOrderService.getDefault(this.idToaThuoc).subscribe(
      result => {
        console.log(result);
        this.precscriptPayment = result;
        if (result) {
          this.precscriptPayment = result;
          this.loadData(result);
        }
      }
    )
  }

  get medicineOrderLines() {
    return this.formGroup.get('medicineOrderLines') as FormArray;
  }

  computeGender(gender) {
    switch (gender) {
      case "male":
        return "Nam"
      case "female":
        return "Nữ"
      default:
        break;
    }
  }

  getState(val) {
    switch (val) {
      case "confirmed":
        return "Đã thanh toán";
      case "cancel":
        return "Hủy thanh toán";
      default:
        return "Chưa thanh toán";
    }
  }

  computeUseAt(value) {
    switch (value) {
      case "in_meal":
        return "Trong khi ăn";
      case "after_meal":
        return "Sau khi ăn";
      case "before_meal":
        return "Trước khi ăn";
      case "after_wakeup":
        return "Sau khi dậy";
      case "before_sleep":
        return "Trước khi đi ngủ";
      default:
        break;
    }
  }

  get getAmountTotal() {
    return this.formGroup.get('amount').value;
  }

  computeAmountOfLine(line: FormGroup) {
    var price = line.get('price') ? line.get('price').value : 0;
    var quantity = line.get('quantity') ? line.get('quantity').value : 1;
    line.get('amountTotal').patchValue(quantity * price);
    return quantity * price;
  }

  computeTotalAmount() {
    var lines = this.medicineOrderLines.controls;
    var totalAmount = 0;
    lines.forEach((line: FormGroup) => {
      totalAmount += this.computeAmountOfLine(line);
    });
    this.formGroup.get('amount').patchValue(totalAmount);
  }

  loadFilteredJournals() {
    this.searchJournals().subscribe(result => {
      this.filteredJournals = result;
      if (this.filteredJournals && this.filteredJournals.length > 1) {
        this.formGroup.get('journal').patchValue(this.filteredJournals[0])
      }
    })
  }

  searchJournals(search?: string) {
    var val = new AccountJournalFilter();
    val.type = 'bank,cash';
    val.search = search || '';
    val.companyId = this.authService.userInfo.companyId;
    return this.accountJournalService.autocomplete(val);
  }


  computeForm(val) {
    val.dateOrder = this.intlService.formatDate(val.dateOrder, "yyyy-MM-ddTHH:mm");
    val.journalId = val.journal ? val.journal.id : null;
    val.toaThuocId = this.idToaThuoc;
    val.employeeId = this.precscriptPayment.employeeId;
    val.partnerId = this.precscriptPayment.partnerId;
    val.state = this.precscriptPayment.state;
    val.companyId = this.authService.userInfo.companyId;
    return val;
  }

  onSavePayment() {
    this.submitted = true;

    if (!this.formGroup.valid) {
      return false;
    }
     
    var val = this.formGroup.value;
    val = this.computeForm(val)
    this.medicineOrderService.confirmPayment(val).subscribe(
      () => {
        this.activeModal.close();
        this.notificationService.show({
          content: 'Thanh toán thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
      }
    )
  }

  onSavePaymentPrint() {
    this.submitted = true;

    if (!this.formGroup.valid) {
      return false;
    }

    var val = this.formGroup.value;
    val = this.computeForm(val)
    this.medicineOrderService.confirmPayment(val).subscribe(
      res => {
        this.notificationService.show({
          content: 'Thanh toán thành công',
          hideAfter: 5000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.activeModal.close();
        this.medicineOrderService.getPrint(res.id).subscribe((result: any) => {
          this.printService.printHtml(result.html);
        });
      }
    )

  }

  onCancelPayment() {
    if (this.id) {
      let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = 'Hủy thanh toán';
      modalRef.componentInstance.body = "Bạn có chắc chắn muốn hủy thanh toán ?"
      modalRef.result.then(() => {
        var ids = [];
        ids.push(this.id);
        this.medicineOrderService.cancelPayment(ids).subscribe(
          () => {
            this.notificationService.show({
              content: 'Hủy thanh toán thành công',
              hideAfter: 3000,
              position: { horizontal: 'center', vertical: 'top' },
              animation: { type: 'fade', duration: 400 },
              type: { style: 'success', icon: true }
            });
            this.activeModal.close();
          }
        )
      }, () => {
      });
    }
  }

  onPrintPayment() {
    if (!this.id) {
      return;
    }
    this.medicineOrderService.getPrint(this.id).subscribe((result: any) => {
      console.log(result.html);
      this.printService.printHtml(result.html);
    });
  }

}
