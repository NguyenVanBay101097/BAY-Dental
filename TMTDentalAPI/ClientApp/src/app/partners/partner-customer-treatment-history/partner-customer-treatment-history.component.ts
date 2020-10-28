import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-partner-customer-treatment-history',
  templateUrl: './partner-customer-treatment-history.component.html',
  styleUrls: ['./partner-customer-treatment-history.component.css']
})
export class PartnerCustomerTreatmentHistoryComponent implements OnInit {
  partnerId: string;
  constructor(
    private activeRoute: ActivatedRoute
  ) { }

  ngOnInit() {
    this.partnerId = this.activeRoute.parent.snapshot.paramMap.get('id');
  }

}
