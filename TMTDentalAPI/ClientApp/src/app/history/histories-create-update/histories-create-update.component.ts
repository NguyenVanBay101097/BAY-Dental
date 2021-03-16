import { Component, OnInit } from '@angular/core';
import { FormGroup, Validators, FormBuilder, FormControl } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { HistoryService } from '../history.service';

@Component({
  selector: 'app-histories-create-update',
  templateUrl: './histories-create-update.component.html',
  styleUrls: ['./histories-create-update.component.css']
})
export class HistoriesCreateUpdateComponent implements OnInit {

  constructor(private fb: FormBuilder, private service: HistoryService, public activeModal: NgbActiveModal) { }

  id: string;
  formCreate: FormGroup;
  isChange = false;
  submitted = false;

  ngOnInit() {
    this.formCreate = this.fb.group({
      name: ['', Validators.required]
    });

    this.loadForm();
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

  loadForm() {
    if (this.id) {
      this.service.getById(this.id).subscribe(
        rs => {
          this.formCreate.patchValue(rs);
        },
        er => {
          console.log(er);
        }
      )
    }
  }

  //Tạo hoặc cập nhật nhóm NV
  createUpdate() {
    this.submitted = true;

    if (!this.formCreate.valid) {
      return;
    }

    var value = this.formCreate.value;
    this.service.createUpdate(this.id, value).subscribe(
      rs => {
        this.isChange = true;
        this.closeModal(rs);
        this.submitted = false;
      },
      er => {
        console.log(er);
        this.submitted = false;
      }
    );
  }

  get f() {
    return this.formCreate.controls;
  }
}
