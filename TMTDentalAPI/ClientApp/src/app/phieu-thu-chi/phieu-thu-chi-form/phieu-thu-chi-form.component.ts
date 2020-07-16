import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { PhieuThuChiService } from '../phieu-thu-chi.service';
import { LoaiThuChiService, loaiThuChiPaged } from 'src/app/loai-thu-chi/loai-thu-chi.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-phieu-thu-chi-form',
  templateUrl: './phieu-thu-chi-form.component.html',
  styleUrls: ['./phieu-thu-chi-form.component.css']
})
export class PhieuThuChiFormComponent implements OnInit {
  title: string;
  resultSelection: string;
  itemId: string;
  myForm: FormGroup;
  submitted = false;
  loaiThuChiList: any = [];
  
  constructor(private fb: FormBuilder, public activeModal: NgbActiveModal, 
    private phieuThuChiService: PhieuThuChiService, 
    private loaiThuChiService: LoaiThuChiService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.queryParamMap.subscribe(params => {
      this.title = params.get('title');
      this.resultSelection = params.get('result_selection');
      this.itemId = params.get('itemId');
      console.log(this.resultSelection);
    });

    this.myForm = this.fb.group({
      companyId: null,
      company: null,
      date: null,
      journalId: null,
      journal: null, 
      state: null, 
      name: null,
      type: null,
      amount: 0,
      communication: null,
      reason: null,
      payerReceiver: null,
      address: null,
      loaiThuChiId: null,
      loaiThuChi: null, 
    });

    setTimeout(() => {

    });
    
  }

  getValueForm(key) {
    return this.myForm.get(key).value;
  }

  onSaveConfirm() {
    var val = this.myForm.value;
    this.phieuThuChiService.create(val).subscribe(result => {
      console.log(result);
    }, err => {
      console.log(err);
    })
  }

  actionConfirm() {
    
  }

  onSave() {

  }
}
