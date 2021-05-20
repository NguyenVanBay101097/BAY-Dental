import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SmsCampaignService } from '../sms-campaign.service';

@Component({
  selector: 'app-sms-birthday-form',
  templateUrl: './sms-birthday-form.component.html',
  styleUrls: ['./sms-birthday-form.component.css']
})
export class SmsBirthdayFormComponent implements OnInit {
  campaign: any;
  constructor(
    private smsCampaignService: SmsCampaignService,
    private router: Router
  ) { }

  ngOnInit() {
    this.loadCampaignBirthday();
  }

  loadCampaignBirthday() {
    this.smsCampaignService.getDefaultCampaignBirthday().subscribe(
      result => {
        if (result) {
          this.campaign = result;
        }
      }
    );
  }

}
