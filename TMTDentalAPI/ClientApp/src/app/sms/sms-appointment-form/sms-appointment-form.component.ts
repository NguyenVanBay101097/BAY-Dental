import { Component, OnInit } from '@angular/core';
import SmsCampaignService from '../sms-campaign.service';

@Component({
  selector: 'app-sms-appointment-form',
  templateUrl: './sms-appointment-form.component.html',
  styleUrls: ['./sms-appointment-form.component.css']
})
export class SmsAppointmentFormComponent implements OnInit {

  campaign: any;
  constructor(
    private smsCampaignService: SmsCampaignService
  ) { }

  ngOnInit() {
    this.loadCampaignBirthday();
  }

  loadCampaignBirthday() {
    this.smsCampaignService.getDefaultCampaignAppointmentReminder().subscribe(
      result => {
        if (result) {
          this.campaign = result;
        }
      }
    );
  }
}
