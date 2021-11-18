import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { Observable } from 'rxjs';
import { debounceTime, map, switchMap, tap } from 'rxjs/operators';
import { AccountJournalService } from 'src/app/account-journals/account-journal.service';
import { ResBankService, ResBankSimple, ResPartnerBankPaged } from 'src/app/res-banks/res-bank.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { NotifyService } from 'src/app/shared/services/notify.service';

@Component({
  selector: 'app-account-bank-cu-dialog',
  templateUrl: './account-bank-cu-dialog.component.html',
  styleUrls: ['./account-bank-cu-dialog.component.css']
})
export class AccountBankCuDialogComponent implements OnInit {
  formGroup: FormGroup;
  title: string = '';
  submitted = false;
  accountId: string;
  bankItems: ResBankSimple[] = [];
  states: any[] = [
    {text: 'Đang sử dụng', value: 1},
    {text: 'Ngừng sử dụng', value: 0}
  ];
  @ViewChild("bankCbx", { static: true }) bankVC: ComboBoxComponent;

  constructor(
    public activeModal: NgbActiveModal,
    private fb: FormBuilder,
    private modalService: NgbModal,
    private notifyService: NotifyService,
    private resBankService: ResBankService,
    private accountJournalService: AccountJournalService
  ) { }

  ngOnInit(): void {
    this.formGroup = this.fb.group({
      accountHolderName: [null, Validators.required],
      accountNumber: [null, Validators.required],
      bankId: [null, Validators.required],
      bankBranch: null,
      active: 1,
      type: "bank"
    });

    this.loadBankItems();

    if (this.accountId){
      this.getDataFromApi();
    }

    this.bankVC.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.bankVC.loading = true)),
        switchMap((value) => this.searchBank$(value)
        )
      )
      .subscribe((result) => {
        this.bankItems = result;
        this.bankVC.loading = false;
      });
  }

  getDataFromApi() {
    this.accountJournalService.getById(this.accountId).subscribe(result => {
      console.log(result);
      
      this.formGroup.patchValue(result);
    })
  }

  onSave() {
    this.submitted = true;
    if (this.formGroup.invalid)
      return;
    
    var formValue = this.formGroup.value;
    console.log(formValue);
    
    if (this.accountId) {
      this.accountJournalService.update(this.accountId,formValue).subscribe(result => {
        this.notifyService.notify("success", "Lưu thành công");
        this.activeModal.close(true);
      }, error => {
        console.log(error); 
      })
    }
    else {
      this.accountJournalService.create(formValue).subscribe(result => {
        this.notifyService.notify("success", "Lưu thành công");
        this.activeModal.close(true);
      }, error => {
        console.log(error);
      });
    }

  }

  onRemove() {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'xl', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa tài khoản ngân hàng';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa Tài khoản ngân hàng?';
    modalRef.result.then(() => {
      this.accountJournalService.delete(this.accountId).subscribe(result => {
        this.notifyService.notify("Xóa thành công", "success");
      }, error => {
        console.log(error); 
      })
    });
  }

  loadBankItems() {
    this.searchBank$().subscribe(result => {
      this.bankItems = result;
    })
  }

  searchBank$(q?) {
    var val = new ResPartnerBankPaged();
    val.search = q || '';
    val.limit = 20;
    return this.resBankService.getAutocomplete(val);
  }

  get f() {
    return this.formGroup.controls;
  }

}
