import { Component, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { AccountJournalFilter, AccountJournalService, AccountJournalSimple } from 'src/app/account-journals/account-journal.service';
import { AuthService } from 'src/app/auth/auth.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PrintService } from 'src/app/shared/services/print.service';
import { MedicineOrderService, PrecscriptPaymentDisplay } from '../medicine-order.service';

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
        return "N???"
      default:
        break;
    }
  }

  getState(val) {
    switch (val) {
      case "confirmed":
        return "???? thanh to??n";
      case "cancel":
        return "H???y thanh to??n";
      default:
        return "Ch??a thanh to??n";
    }
  }

  computeUseAt(value) {
    switch (value) {
      case "in_meal":
        return "Trong khi ??n";
      case "after_meal":
        return "Sau khi ??n";
      case "before_meal":
        return "Tr?????c khi ??n";
      case "after_wakeup":
        return "Sau khi d???y";
      case "before_sleep":
        return "Tr?????c khi ??i ng???";
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
        this.formGroup.get('journal').patchValue(this.filteredJournals.find(x => x.type == 'cash'))
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

    if (this.medicineOrderLines.length <= 0) {
      this.notificationService.show({
        content: '????n thu???c kh??ng c?? thu???c ????? thanh to??n',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });
      return false;

    }

    var val = this.formGroup.value;
    val = this.computeForm(val)
    this.medicineOrderService.confirmPayment(val).subscribe(
      () => {
        this.activeModal.close();
        this.notificationService.show({
          content: 'Thanh to??n th??nh c??ng',
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

    if (this.medicineOrderLines.length <= 0) {
      this.notificationService.show({
        content: '????n thu???c kh??ng c?? thu???c ????? thanh to??n',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });
      return false;
    }

    var val = this.formGroup.value;
    val = this.computeForm(val)
    this.medicineOrderService.confirmPayment(val).subscribe(
      res => {
        this.notificationService.show({
          content: 'Thanh to??n th??nh c??ng',
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
      modalRef.componentInstance.title = 'H???y thanh to??n';
      modalRef.componentInstance.body = "B???n c?? ch???c ch???n mu???n h???y thanh to??n ?"
      modalRef.result.then(() => {
        var ids = [];
        ids.push(this.id);
        this.medicineOrderService.cancelPayment(ids).subscribe(
          () => {
            this.notificationService.show({
              content: 'H???y thanh to??n th??nh c??ng',
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
      this.printService.printHtml(result.html);
    });
  }

}
