import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import {SmsCampaignService} from '../sms-campaign.service';

@Component({
  selector: 'app-sms-birthday-form',
  templateUrl: './sms-birthday-form.component.html',
  styleUrls: ['./sms-birthday-form.component.css']
})
export class SmsBirthdayFormComponent implements OnInit {

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
