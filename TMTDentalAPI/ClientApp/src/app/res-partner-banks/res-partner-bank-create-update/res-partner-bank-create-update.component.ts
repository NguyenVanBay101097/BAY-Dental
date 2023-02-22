import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ResPartnerBankService, ResBankSimple, AccountJournalSave } from '../res-partner-bank.service';
import { ResPartnerBankBasic } from '../res-partner-bank';
import { ResBankPaged } from 'src/app/res-banks/res-bank';
import { DialogRef, DialogService, DialogCloseResult } from '@progress/kendo-angular-dialog';
import { NotificationService } from '@progress/kendo-angular-notification';

@Component({
  selector: 'app-res-partner-bank-create-update',
  templateUrl: './res-partner-bank-create-update.component.html',
  styleUrls: ['./res-partner-bank-create-update.component.css']
})
export class ResPartnerBankCreateUpdateComponent implements OnInit {

  @Input() id: string;
  formPartnerBank: FormGroup;
  title: string;
  isChange = false;
  search = "";

  banks: ResBankSimple[];
  constructor(private fb: FormBuilder, public activeModal: NgbActiveModal, private service: ResPartnerBankService) { }

  ngOnInit() {
    this.formPartnerBank = this.fb.group({
      accountNumber: null,
      bank: null,
      name: null,
      type: 'bank'
    })

    setTimeout(() => {
      this.loadAutocompleteBank();
    });


    this.title = this.id ? 'Cập nhật tài khoản' : 'Thêm mới tài khoản';
  }

  closeModal(rs) {
    if (this.isChange) {
      if (rs) {
        this.activeModal.close(rs);
      } else {
        this.activeModal.close(true);
      }
    }
    else {
      this.activeModal.dismiss();
    }
  }


  createUpdateBankAccount() {
    var value = this.formPartnerBank.value;
    value.bankId = value.bank ? value.bank.id : '';
    this.service.createUpdate(value, this.id).subscribe(
      rs => {
        this.isChange = true;
        this.closeModal(rs);
      },
      er => {
        console.log(er);
      }
    );
  }

  getBankAccount() {
    if (this.id) {
      this.service.getById(this.id).subscribe(
        rs => {
          this.formPartnerBank.patchValue(rs);
          if (rs.bankId) {
            var bank = this.banks.find(x => x.id == rs.bankId);
            this.formPartnerBank.get('bank').patchValue(bank);
          }
        }
      )
    }
  }

  getBank() {
    return this.formPartnerBank.get('bank').value;
  }

  getType() {
    return this.formPartnerBank.get('type').value;
  }

  loadAutocompleteBank() {
    var val = new ResBankPaged;
    val.limit = 20;
    val.offset = 0;
    val.search = this.search;
    this.service.autocompleteBank(val).subscribe(
      rs => {
        this.banks = rs;
        this.getBankAccount();
      }
    )
  }

  changeMethod(e) {
    console.log(e);
    if (e.value == '') {

    }
    // this.formPartnerBank.get('accountNumber').setValue(null);
    // this.formPartnerBank.get('bank').setValue(null);
  }

}
