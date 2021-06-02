import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SmsCampaignService } from '../sms-campaign.service';

@Component({
  selector: 'app-sms-appointment-form',
  templateUrl: './sms-appointment-form.component.html',
  styleUrls: ['./sms-appointment-form.component.css']
})
export class SmsAppointmentFormComponent implements OnInit {

  campaign: any;
  campaignId: string;

  constructor(
    private smsCampaignService: SmsCampaignService,
    private route: ActivatedRoute
  ) { }

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.campaignId = params.get('id');
    });
    this.LoadCampaignAppointmentReminder()
  }

  LoadCampaignAppointmentReminder() {
    this.smsCampaignService.getDefaultCampaignAppointmentReminder().subscribe(
      result => {
        if (result) {
          this.campaign = result;
        }
      }
    );
  }
}
