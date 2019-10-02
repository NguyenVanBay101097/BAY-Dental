import { Component, OnInit } from '@angular/core';
import { PartnerService } from '../partner.service';
import { PartnerDisplay } from '../partner-simple';

@Component({
  selector: 'app-partner-info',
  templateUrl: './partner-info.component.html',
  styleUrls: ['./partner-info.component.css']
})
export class PartnerInfoComponent implements OnInit {

  constructor(private service: PartnerService) { }

  partnerId: string;
  partner: PartnerDisplay

  address: string;

  ngOnInit() {
    this.getPartnerInfo();
  }

  getPartnerInfo() {
    this.partner = new PartnerDisplay();
    var addArray = new Array<string>();
    this.service.getPartner(this.partnerId).subscribe(
      rs => {
        this.partner = rs as PartnerDisplay;
        debugger;
        if (rs.street !== null) {
          addArray.push(rs.street);
        }
        if (rs.ward && rs.ward.name) {
          addArray.push(rs.ward.name);
        }
        if (rs.district && rs.district.name) {
          addArray.push(rs.district.name);
        }
        if (rs.city && rs.city.name) {
          addArray.push(rs.city.name);
        }
        console.log(addArray);
        this.address = addArray.join(', ');
      });
  }


}
