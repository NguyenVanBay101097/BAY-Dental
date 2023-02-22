import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-partner-overview-advisory',
  templateUrl: './partner-overview-advisory.component.html',
  styleUrls: ['./partner-overview-advisory.component.css']
})
export class PartnerOverviewAdvisoryComponent implements OnInit {

  @Input() saleQuotations: any;
  constructor() { }

  ngOnInit() {
    console.log(this.saleQuotations);
    
  }

}
