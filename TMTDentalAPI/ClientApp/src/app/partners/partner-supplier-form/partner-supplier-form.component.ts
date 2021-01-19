import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PartnerService } from '../partner.service';

@Component({
  selector: 'app-partner-supplier-form',
  templateUrl: './partner-supplier-form.component.html',
  styleUrls: ['./partner-supplier-form.component.css']
})
export class PartnerSupplierFormComponent implements OnInit {
  constructor(
    private activeRoute: ActivatedRoute,
    private partnerService: PartnerService
  ) { }

  ngOnInit() {
   
  }

  

}
