import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ResPartnerBankService } from '../res-partner-bank.service';
import { ResPartnerBankBasic } from '../res-partner-bank';

@Component({
  selector: 'app-res-partner-bank-create-update',
  templateUrl: './res-partner-bank-create-update.component.html',
  styleUrls: ['./res-partner-bank-create-update.component.css']
})
export class ResPartnerBankCreateUpdateComponent implements OnInit {

  @Input() id: string;
  formBank: FormGroup;
  title: string;
  isChange = false;
  constructor(private fb: FormBuilder, private activeModal: NgbActiveModal, private service: ResPartnerBankService) { }

  ngOnInit() {
    this.formBank = this.fb.group({
      name: null,
      bic: null
    })

    this.getBank();
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


  createUpdateBank() {
    var value = new ResPartnerBankBasic;
    value = this.formBank.value;
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

  getBank() {
    if (this.id) {
      this.service.getById(this.id).subscribe(
        rs => {
          this.formBank.patchValue(rs);
        }
      )
    }
  }

}
