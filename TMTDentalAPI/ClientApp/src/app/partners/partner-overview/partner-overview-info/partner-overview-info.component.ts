import { ChangeDetectionStrategy, Component, Input, OnInit, SimpleChanges } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PartnerCustomerCuDialogComponent } from 'src/app/shared/partner-customer-cu-dialog/partner-customer-cu-dialog.component';
import { PartnersService } from 'src/app/shared/services/partners.service';
import { PartnerDisplay } from '../../partner-simple';
import { Pipe, PipeTransform } from '@angular/core';
@Pipe({
  name: 'getAge'
})
export class GetAgePipe implements PipeTransform {
  transform(y: number): any {
    var today = new Date();
    console.log('a');
    
    return today.getFullYear() - y;
  }
}

@Component({
  selector: 'app-partner-overview-info',
  templateUrl: './partner-overview-info.component.html',
  styleUrls: ['./partner-overview-info.component.css']
})
export class PartnerOverviewInfoComponent implements OnInit {
  @Input() partner: any;
  
  constructor(
    private modalService: NgbModal,
    private PartnerOdataService: PartnersService,
  ) { }

  ngOnInit() {
  }

  getGender(g: string) {
    console.log('a');
    
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

  getAddress(partner) {
    var list = [];
    if (partner.Street) {
      list.push(partner.Street);
    }

    if (partner.Ward && partner.Ward.Name) {
      list.push(partner.Ward.Name);
    }

    if (partner.District && partner.District.Name) {
      list.push(partner.District.Name);
    }

    if (partner.City && partner.City.Name) {
      list.push(partner.City.Name);
    }

    return list.join(', ');
  }

  getHistories(partner) {
    if (partner.Histories) {
      var arr = new Array<string>();
      this.partner.Histories.forEach(e => {
        arr.push(e.Name);
      });
      return arr.join(', ');
    }
  }

  getCategories() {
    if (this.partner.Categories) {
      var arr = new Array<string>();
      this.partner.Categories.forEach(e => {
        arr.push(e.Name);
      });
      return arr.join(', ');
    }
  }

  getReferral() {
    if (this.partner.Source) {
      var s = this.partner.Source;
      switch (s.type.toLowerCase()) {
        case 'normal':
          return this.partner.Source.Name;
        case 'referral':
          return this.partner.ReferralUser.Name;
        case 'ads':
          return 'Quảng cáo';
        case 'friend':
          return 'Bạn bè';
      }
    }
  }

  editCustomer() {
    if (this.partner && this.partner.Id) {
      const modalRef = this.modalService.open(PartnerCustomerCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.id = this.partner.Id;
      modalRef.componentInstance.title = 'Cập nhật khách hàng';

      modalRef.result.then(result => {
        this.GetPartner(this.partner.Id);
      }, () => {})
    }
  }

  GetPartner(id) {
    this.PartnerOdataService.getDisplay(id).subscribe((res: any) => {
      this.partner = res;
    });
  }

}
