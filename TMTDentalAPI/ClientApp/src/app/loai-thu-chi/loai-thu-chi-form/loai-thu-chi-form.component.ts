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
      active: null, 
      code: null, 
      company: null, 
      companyId: null, 
      internalType: null, 
      name: null, 
      note: null, 
      reconcile: null, 
      userType: null, 
      userTypeId: null, 
      isExcludedProfitAndLossReport: null 
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

  save() {
    var value = this.accountForm.value;
    value.companyId = value.company ? value.company.id : null; 
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
    this.activeModal.dismiss();
  }
}
