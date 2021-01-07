import { Component, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { AccountJournalFilter, AccountJournalService, AccountJournalSimple } from 'src/app/account-journals/account-journal.service';
import { AuthService } from 'src/app/auth/auth.service';
import { MedicineOrderService, PrecscriptPaymentDisplay, PrecsriptionPaymentSave } from '../medicine-order.service';
import { PrintService } from 'src/app/shared/services/print.service';
import { Router } from '@angular/router';
import { PageChangeEvent } from '@progress/kendo-angular-grid';

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
  constructor(
    private accountJournalService: AccountJournalService,
    private fb: FormBuilder,
    private authService: AuthService,
    private printService: PrintService,
    private activeModal: NgbActiveModal,
    private medicineOrderService: MedicineOrderService,
    private intlService: IntlService,
    private router: Router
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      journal: [null, Validators.required],
      orderDate: new Date(),
      note: '',
      amount: 0,
      medicineOrderLines: this.fb.array([])
    });

    this.loadFilteredJournals();

    this.journalCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.journalCbx.loading = true)),
      switchMap(value => this.searchJournals(value))
    ).subscribe(result => {
      this.filteredJournals = result;
      this.journalCbx.loading = false;
    });

    if (this.idToaThuoc) {
      this.getDefault();
    }
    if (this.id) {
      this.loadRecord();
    }
  }

  loadRecord() {
    this.medicineOrderService.getDisplay(this.id).subscribe(
      result => {
        if (result) {
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
      lines.forEach(line => {
        control.push(this.fb.group(line));
      });
      this.computeTotalAmount();
    }
  }

  getDefault() {
    this.medicineOrderService.getDefault(this.idToaThuoc).subscribe(
      result => {
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

  onSaveConfirm() {
    if (this.formGroup.invalid)
      return
    var val = this.formGroup.value;
    val = this.computeForm(val)
    this.medicineOrderService.create(val).subscribe(
      result => {
        // đéo làm gì
      }
    )
  }

  onCancelPayment() {
    if (this.id) {
      var ids = [];
      ids.push(this.id);
      this.medicineOrderService.cancelPayment(ids).subscribe(
        () => {
          this.activeModal.close();
        }
      )
    }
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
    if (this.formGroup.invalid)
      return
    var val = this.formGroup.value;
    val = this.computeForm(val)
    this.medicineOrderService.confirmPayment(val).subscribe(
      () => {
        this.activeModal.close();
        this.router.navigateByUrl("medicine-orders/prescription-payments")
      }
    )
  }

  onSavePaymentPrint() {

  }

  onPrint() {
    if (!this.id) {
      return;
    }
    this.medicineOrderService.getPrint(this.id).subscribe((result: any) => {
      this.printService.printHtml(result.html);
    });
  }

  printPayment() {

  }

}
