import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SmsCampaignService } from '../sms-campaign.service';

@Component({
  selector: 'app-sms-thanks-form',
  templateUrl: './sms-thanks-form.component.html',
  styleUrls: ['./sms-thanks-form.component.css']
})
export class SmsThanksFormComponent implements OnInit {
  campaign: any;
  constructor(
    private smsCampaignService: SmsCampaignService,
    private router: Router
  ) { }

  ngOnInit() {
    this.loadThanksCustomer();
  }

  loadThanksCustomer() {
    this.smsCampaignService.getDefaultCampaignBirthday().subscribe(
      result => {
        if (result) {
          this.campaign = result;
        }
      }
    );
  }
}
