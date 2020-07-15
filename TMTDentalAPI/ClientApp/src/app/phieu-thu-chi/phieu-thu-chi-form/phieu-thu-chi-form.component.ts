import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { PhieuThuChiService } from '../phieu-thu-chi.service';
import { LoaiThuChiService, loaiThuChiPaged } from 'src/app/loai-thu-chi/loai-thu-chi.service';

@Component({
  selector: 'app-phieu-thu-chi-form',
  templateUrl: './phieu-thu-chi-form.component.html',
  styleUrls: ['./phieu-thu-chi-form.component.css']
})
export class PhieuThuChiFormComponent implements OnInit {
  title: string;
  type: string;
  itemId: string;
  myForm: FormGroup;
  submitted = false;
  loaiThuChiList: any = [];
  
  constructor(private fb: FormBuilder, public activeModal: NgbActiveModal, 
    private phieuThuChiService: PhieuThuChiService, private loaiThuChiService: LoaiThuChiService) { }

  ngOnInit() {
    this.myForm = this.fb.group({
      loaiThuChi: null
    });

    setTimeout(() => {
      this.loadLoaiThuChiList();

    });
    
  }

  loadLoaiThuChiList() {
    var val = new loaiThuChiPaged();
    val.type = this.type;
    this.loaiThuChiService.getPaged(val)
    .subscribe(res => {
      this.loaiThuChiList = res.items;
    }, err => {
      console.log(err);
    })
  }
}
