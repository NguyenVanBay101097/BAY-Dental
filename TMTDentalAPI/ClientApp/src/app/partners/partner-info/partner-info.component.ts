import { Component, OnInit, Input } from '@angular/core';
import { PartnerService } from '../partner.service';
import { PartnerDisplay, PartnerCategorySimple } from '../partner-simple';
import { HistorySimple } from 'src/app/history/history';
import { PartnerCategoryBasic } from 'src/app/partner-categories/partner-category.service';

@Component({
  selector: 'app-partner-info',
  templateUrl: './partner-info.component.html',
  styleUrls: ['./partner-info.component.css']
})
export class PartnerInfoComponent implements OnInit {

  constructor(private service: PartnerService) { }

  @Input() public partnerId: string; // ID của khách hàng
  // partnerId: string;
  partner = new PartnerDisplay();

  address: string;

  ngOnInit() {
    if (this.partnerId) {
      this.getPartnerInfo();
    }
  }

  getPartnerInfo() {
    var addArray = new Array<string>();
    this.service.getPartner(this.partnerId).subscribe(
      rs => {
        this.partner = rs as PartnerDisplay;
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

  getGender(g: string) {
    if (g) {
      switch (g.toLowerCase()) {
        case 'female':
          return 'Nữ';
        case 'male':
          return 'Nam';
        default:
          return 'Khác';
      }
    }
  }

  getAge(y: number) {
    var today = new Date();
    return today.getFullYear() - y;
  }

  getHistories() {
    if (this.partner.histories) {
      var arr = new Array<string>();
      this.partner.histories.forEach(e => {
        arr.push(e.name);
      });
      return arr.join(', ');
    }
  }

  getCategories() {
    if (this.partner.categories) {
      var arr = new Array<string>();
      this.partner.categories.forEach(e => {
        arr.push(e.name);
      });
      return arr.join(', ');
    }
  }

  getReferral() {
    if (this.partner.source) {
      var s = this.partner.source;
      switch (s.toLowerCase()) {
        case 'employee':
          return this.partner.employees.name;
        case 'other':
          return this.partner.note;
        case 'ads':
          return 'Quảng cáo';
        case 'friend':
          return 'Bạn bè';
      }
    }
  }

}
