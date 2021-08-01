import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { LoaiThuChiService, loaiThuChiDefault } from 'src/app/loai-thu-chi/loai-thu-chi.service';

@Component({
  selector: 'app-loai-thu-chi-form',
  templateUrl: './loai-thu-chi-form.component.html',
  styleUrls: ['./loai-thu-chi-form.component.css']
})
export class LoaiThuChiFormComponent implements OnInit {
  title: string;
  type: string;
  itemId: string;
  accountForm: FormGroup;
  submitted = false;

  constructor(private fb: FormBuilder, public activeModal: NgbActiveModal, 
    private loaiThuChiService: LoaiThuChiService) { }

  ngOnInit() { 
    this.accountForm = this.fb.group({
      name: [null, Validators.required],
      code: [null, Validators.required],
      note: null,
      type: null,
      isAccounting: null,
      companyId: null,
    });

    setTimeout(() => {
      if (!this.itemId) 
        this.defaultGet();
      else 
        this.get();
    });
  }

  defaultGet() {
    var val = new loaiThuChiDefault();
    val.type = this.type;
    this.loaiThuChiService.defaultGet(val).subscribe(result => {
      this.accountForm.patchValue(result);
    }, err => {
      console.log(err);
      this.activeModal.dismiss();
    })
  }
  
  get() {
    this.loaiThuChiService.get(this.itemId).subscribe(result => {
      this.accountForm.patchValue(result);
    }, err => {
      console.log(err);
      this.activeModal.dismiss();
    })
  }

  get f() { return this.accountForm.controls; }

  save() {
    this.submitted = true;
    if (!this.accountForm.valid) {
      return;
    }

    var value = this.accountForm.value;
    value.type = this.type;
    if (!this.itemId) {
      this.loaiThuChiService.create(value).subscribe(result => {
        this.activeModal.close(result);
      }, err => {
        console.log(err);
        this.submitted = false;
      })
    } else {
      this.loaiThuChiService.update(this.itemId, value).subscribe(result => {
        this.activeModal.close(result);
      }, err => {
        console.log(err);
        this.submitted = false;
      })
    }
  }

  cancel() {
    this.activeModal.dismiss();
  }
}
