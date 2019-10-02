import { Component, OnInit, Input } from '@angular/core';
import { PartnerDisplay, PartnerInfoViewModel } from '../partner-simple';
import { PartnerService } from '../partner.service';

@Component({
  selector: 'app-partner-customer-list-detail',
  templateUrl: './partner-customer-list-detail.component.html',
  styleUrls: ['./partner-customer-list-detail.component.css']
})
export class PartnerCustomerListDetailComponent implements OnInit {
  @Input() public item: any;
  partner: PartnerInfoViewModel;
  constructor(private partnerService: PartnerService) { }

  ngOnInit() {
    this.loadPartnerDisplay();
  }

  loadPartnerDisplay() {
    this.partnerService.getInfo(this.item.id).subscribe(result => {
      console.log(result);
      this.partner = result;
    });
  }

  onTabSelect(e) {
    console.log(e);
  }
}
