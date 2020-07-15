import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { loaiThuChiDefault, LoaiThuChiService } from '../loai-thu-chi.service';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { CompanyService, CompanyPaged } from 'src/app/companies/company.service';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-loai-thu-chi-form',
  templateUrl: './loai-thu-chi-form.component.html',
  styleUrls: ['./loai-thu-chi-form.component.css']
})
export class LoaiThuChiFormComponent implements OnInit {
  title: string;
  type: string;
  itemId: string;
  myForm: FormGroup;
  submitted = false;

  constructor(private fb: FormBuilder, public activeModal: NgbActiveModal, 
    private loaiThuChiService: LoaiThuChiService, 
    private companyService: CompanyService) { }

  ngOnInit() { 
    this.myForm = this.fb.group({
      name: ['', Validators.required],
      code: ['', Validators.required],
      note: null,
      type: null,
      isInclude: null,
      accountId: null,
      account: null,
      companyId: null,
      company: null
    });
    .0

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
      console.log(result);
      this.myForm.patchValue(result);
    }, err => {
      console.log(err);
      this.activeModal.dismiss();
    })
  }
  
  get() {
    this.loaiThuChiService.get(this.itemId).subscribe(result => {
      this.myForm.patchValue(result);
    }, err => {
      console.log(err);
      this.activeModal.dismiss();
    })
  }

  save() {
    this.submitted = true;

    if (!this.myForm.valid) {
      return;
    }

    var value = this.myForm.value;
    value.type = this.type;

    if (!this.itemId) {
      this.loaiThuChiService.create(value).subscribe(result => {
        this.activeModal.close(true);
      }, err => {
        console.log(err);
      })
    } else {
      this.loaiThuChiService.update(this.itemId, value).subscribe(result => {
        this.activeModal.close(result);
      }, err => {
        console.log(err);
      })
    }
  }

  cancel() {
    this.submitted = false;
    this.activeModal.dismiss();
  }

  get f() {
    return this.myForm.controls;
  }
}
