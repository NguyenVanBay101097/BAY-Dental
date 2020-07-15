import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
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
  accountForm: FormGroup;

  constructor(private fb: FormBuilder, public activeModal: NgbActiveModal, 
    private loaiThuChiService: LoaiThuChiService, 
    private companyService: CompanyService) { }

  ngOnInit() { 
    this.accountForm = this.fb.group({
      name: null,
      code: null,
      note: null,
      type: null,
      isInclude: null,
      accountId: null,
      account: null,
      companyId: null,
      company: null
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
      console.log(result);
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

  save() {
    var value = this.accountForm.value;
    value.type = this.type;
    console.log(value);
    if (!this.itemId) {
      this.loaiThuChiService.create(value).subscribe(result => {
        console.log(result);
        this.activeModal.close(true);
      }, err => {
        console.log(err);
      })
    } else {
      this.loaiThuChiService.update(this.itemId, value).subscribe(result => {
        console.log(result);
        this.activeModal.close(result);
      }, err => {
        console.log(err);
      })
    }
  }

  cancel() {
    this.activeModal.dismiss();
  }
}
