import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CompositeFilterDescriptor } from '@progress/kendo-data-query';
import { AppointmentDisplay } from 'src/app/appointment/appointment';
import { PartnersService } from 'src/app/shared/services/partners.service';
import { PartnerDisplay } from '../../partner-simple';
import { PartnerService } from '../../partner.service';

@Component({
  selector: 'app-partner-overview',
  templateUrl: './partner-overview.component.html',
  styleUrls: ['./partner-overview.component.css']
})
export class PartnerOverviewComponent implements OnInit {
  partnerId: string;
  partner: PartnerDisplay;
  customerAppointment: AppointmentDisplay;

  constructor(
    private activeRoute: ActivatedRoute,
    private PartnerOdataService: PartnersService,
    private partnerService: PartnerService
  ) { }

  ngOnInit() {
    this.partnerId = this.activeRoute.parent.snapshot.paramMap.get('id');
    this.GetPartner();
    this.loadCustomerAppointment();
  }

  GetPartner() {
    this.PartnerOdataService.getDisplay(this.partnerId).subscribe((res: any) => {
      this.partner = res;
    });
  }

  loadCustomerAppointment() {
    this.partnerService.getNextAppointment(this.partnerId).subscribe(
      rs => {
        this.customerAppointment = rs;
      });
  }

}
