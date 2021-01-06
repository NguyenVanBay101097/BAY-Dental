import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { AccountJournalFilter, AccountJournalService, AccountJournalSimple } from 'src/app/account-journals/account-journal.service';
import { AuthService } from 'src/app/auth/auth.service';
import { PrintService } from 'src/app/shared/services/print.service';
import { MedicineOrderService } from '../medicine-order.service';

@Component({
  selector: 'app-medicine-order-create-dialog',
  templateUrl: './medicine-order-create-dialog.component.html',
  styleUrls: ['./medicine-order-create-dialog.component.css']
})
export class MedicineOrderCreateDialogComponent implements OnInit {
  @ViewChild('journalCbx', { static: true }) journalCbx: ComboBoxComponent;

  filteredJournals: AccountJournalSimple[];
  formGroup: FormGroup;
  title: string;
  id: string;
  constructor(
    private accountJournalService: AccountJournalService,
    private fb: FormBuilder,
    private authService: AuthService,
    private activeModal: NgbActiveModal,
    private printService: PrintService,
    private medicineOrderService: MedicineOrderService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      journal: [null, Validators.required],
      datePaymentObj: new Date(),
      note: ''
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

  }

  onSavePayment() {

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

}
