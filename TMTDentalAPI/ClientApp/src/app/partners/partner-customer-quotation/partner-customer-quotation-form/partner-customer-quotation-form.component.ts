import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-partner-customer-quotation-form',
  templateUrl: './partner-customer-quotation-form.component.html',
  styleUrls: ['./partner-customer-quotation-form.component.css']
})
export class PartnerCustomerQuotationFormComponent implements OnInit {

  formGroup: FormGroup;

  constructor() { }

  ngOnInit() {
  }


  onSave() {
  }

  addLine(){
    
  }
}
