import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ResBankBasic } from '../res-bank';
import { ResBankService } from '../res-bank.service';

@Component({
  selector: 'app-res-bank-create-update',
  templateUrl: './res-bank-create-update.component.html',
  styleUrls: ['./res-bank-create-update.component.css']
})
export class ResBankCreateUpdateComponent implements OnInit {

  @Input() id: string;

  formBank: FormGroup;
  title: string;
  isChange = false;
  constructor(private fb: FormBuilder, public activeModal: NgbActiveModal, private service: ResBankService) { }

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
    var value = new ResBankBasic;
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
