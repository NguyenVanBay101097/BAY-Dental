import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';
import { PartnerService } from '../partner.service';

@Component({
  selector: 'app-partner-customer-detail',
  templateUrl: './partner-customer-detail.component.html',
  styleUrls: ['./partner-customer-detail.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class PartnerCustomerDetailComponent implements OnInit {

  id: string;
  partner: any;

  overview = false;
  advisories = false;
  labo_orders = false;
  treatment = false;
  advances = false;
  constructor(private partnerService: PartnerService, private route: ActivatedRoute,private checkPermissionService: CheckPermissionService ) { }

  ngOnInit() {
    // this.id = this.route.snapshot.paramMap.get('id');
    // if (this.id) {
    //   this.loadPartner(this.id);
    // }

    this.route.params.subscribe(params => {
      this.id = params.id
      if (this.id) {
        this.loadPartner(this.id);
      }
    });
  }

  loadPartner(id: string) {
    this.partnerService.getPartner(this.id).subscribe((result) => {
      this.partner = result;
    });
  }

  checkRole(){
    this.overview = this.checkPermissionService.check(['Basic.DotKham.Read']);
   // this.advisories = this.checkPermissionService.check(['Basic.Partner.Create']);
    this.labo_orders = this.checkPermissionService.check(['Labo.LaboOrder.Read']);
    this.treatment = this.checkPermissionService.check(["Basic.SaleOrder.Read"])
    this.advances = this.checkPermissionService.check(["Basic.PartnerAdvance.Read"]);
  }
}
