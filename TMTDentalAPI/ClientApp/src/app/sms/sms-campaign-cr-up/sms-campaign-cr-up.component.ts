import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-sms-campaign-cr-up',
  templateUrl: './sms-campaign-cr-up.component.html',
  styleUrls: ['./sms-campaign-cr-up.component.css']
})
export class SmsCampaignCrUpComponent implements OnInit {

  title: string;
  formGroup: FormGroup;

  get f() { return this.formGroup.controls; }
  
  constructor(private fb: FormBuilder, public activeModal: NgbActiveModal) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required], 
      typeDate: "unlimited", // unlimited: vô thời hạn, period: khoảng thời gian
      startDateObj: [new Date(), Validators.required], 
      endDateObj: [new Date(), Validators.required], 
      limitMessages: [0, Validators.required]
    })
  }

  getValueFormControl(key) {
    return this.formGroup.get(key).value;
  }
}
