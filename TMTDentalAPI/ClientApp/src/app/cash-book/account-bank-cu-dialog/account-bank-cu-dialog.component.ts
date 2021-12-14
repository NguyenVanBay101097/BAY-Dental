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
import * as _ from 'lodash';

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
    { text: 'Đang sử dụng', value: true },
    { text: 'Ngừng sử dụng', value: false }
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
      accountNumber: [null, [Validators.required]],
      bank: [null, Validators.required],
      bankBranch: null,
      active: true,
    });

    this.loadBankItems();

    if (this.accountId) {
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

  get accountHolderNameCtrl() {
    return this.formGroup.get('accountHolderName');
  }

  getDataFromApi() {
    this.accountJournalService.getBankJournal(this.accountId).subscribe((result: any) => {
      this.formGroup.patchValue(result);
      if (result.bank) {
        this.bankItems = _.unionBy(this.bankItems, [result.bank], 'id');
      }
    })
  }

  onSave() {
    this.submitted = true;
    if (this.formGroup.invalid)
      return;

    var formValue = this.formGroup.value;
    formValue.bankId = formValue.bank.id;
    if (this.accountId) {
      formValue.id = this.accountId;
      this.accountJournalService.updateBankJournal(formValue).subscribe(result => {
        this.notifyService.notify("success", "Lưu thành công");
        this.activeModal.close(this.accountId);
      }, error => {
        console.log(error);
      })
    }
    else {
      this.accountJournalService.createBankJournal(formValue).subscribe((result:any) => {
        this.notifyService.notify("success", "Lưu thành công");
        this.activeModal.close(result.id);
      }, error => {
        console.log(error);
      });
    }

  }

  onRemove() {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa tài khoản ngân hàng';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa Tài khoản ngân hàng?';
    modalRef.result.then(() => {
      this.accountJournalService.delete(this.accountId).subscribe(result => {
        this.notifyService.notify("success", "Xóa thành công");
        this.activeModal.close();

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
    return this.resBankService.getAutocomplete(val).pipe(
      tap(res=> {
        res.forEach((x:any) => {
          x.displayName = x.bic + ' - ' + x.name;
        });
      })
    );
  }

  get f() {
    return this.formGroup.controls;
  }

  public valueNormalizer = (bankId: Observable<string>) =>
    bankId.pipe(
      map((content: string) => {
        return {
          name: content,
          id: content,
        };
      })
    );

  onCancel() {
    this.activeModal.dismiss();
  }

}
