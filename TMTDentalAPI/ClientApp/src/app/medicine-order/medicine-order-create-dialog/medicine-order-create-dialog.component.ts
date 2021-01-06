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

@Component({
  selector: 'app-medicine-order-create-dialog',
  templateUrl: './medicine-order-create-dialog.component.html',
  styleUrls: ['./medicine-order-create-dialog.component.css']
})
export class MedicineOrderCreateDialogComponent implements OnInit {
  @ViewChild('journalCbx', { static: true }) journalCbx: ComboBoxComponent;
  idToaThuoc: string;
  precscriptPaymen: PrecscriptPaymentDisplay = new PrecscriptPaymentDisplay();
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
    private intlService: IntlService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      journal: [null, Validators.required],
      dateOrder: new Date(),
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
  }

  getDefault() {
    this.medicineOrderService.getDefault(this.idToaThuoc).subscribe(
      result => {
        this.precscriptPaymen = result;
        console.log(this.precscriptPaymen);

        if (this.precscriptPaymen.toaThuoc && this.precscriptPaymen.toaThuoc.lines) {
          var control = this.formGroup.get('medicineOrderLines') as FormArray;
          control.clear();
          var totalAmount = 0;
          var lines = this.precscriptPaymen.toaThuoc.lines;
          lines.forEach(line => {
            control.push(this.fb.group(line));
          });
          this.computeTotalAmount();
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
    var price = line.get('product') && line.get('product').value ? line.get('product').value.listPrice : 0;
    var quantity = line.get('quantity') ? line.get('quantity').value : 1;
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

  onSave() {
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

  computeForm(val) {
    val.dateOrder = this.intlService.formatDate(val.dateOrder, "yyyy-MM-ddTHH:mm");
    val.journalId = val.journal ? val.journal.id : null;
    val.toaThuocId = this.idToaThuoc;
    val.employeeId = this.precscriptPaymen.employeeId;
    val.partnerId = this.precscriptPaymen.partnerId;
    val.companyId = this.authService.userInfo.companyId;
    if (val.medicineOrderLines) {
      var lines = [];
      val.medicineOrderLines.forEach(line => {
        var model = {
          toaThuocLineId: line.id,
          quantity: line.quantity ? line.quantity : 1,
          price: line.product ? line.product.listPrice : 0,
          amountTotal: line.quantity * line.product.listPrice,
        }
      });
    }
    return val;
  }

  onSavePayment() {
    if (this.formGroup.invalid)
      return
    var val = this.formGroup.value;
    this.medicineOrderService.create(val).subscribe(
      result => {
        if (result) {
          var ids = [];
          ids.push(result.id)
          this.medicineOrderService.confirmPayment(ids).subscribe(
            () => {

            }
          )
          // đéo làm gì
        }
      }
    )
  }

  onSavePaymentPrint() {

  }

  onPrint() {
    if(!this.id) {
      return;
    }
    this.medicineOrderService.getPrint(this.id).subscribe((result: any) => {
      this.printService.printHtml(result.html);
    });
  }

  printPayment() {

  }

}
