import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { PhieuThuChiService } from '../phieu-thu-chi.service';

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
  
  constructor(private fb: FormBuilder, public activeModal: NgbActiveModal, 
    private phieuThuChiService: PhieuThuChiService) { }

  ngOnInit() {
    this.myForm = this.fb.group({

    });

  }

}
