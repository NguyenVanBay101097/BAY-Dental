import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { SmsCampaignService } from '../sms-campaign.service';

@Component({
  selector: "app-sms-thanks-form",
  templateUrl: "./sms-thanks-form.component.html",
  styleUrls: ["./sms-thanks-form.component.css"],
  host: {
    class: "o_action o_view_controller",
  },
})
export class SmsThanksFormComponent implements OnInit {
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
    this.loadThanksCustomer();
  }

  loadThanksCustomer() {
    this.smsCampaignService.getDefaultThanksCustomer().subscribe(
      result => {
        if (result) {
          this.campaign = result;
        }
      }
    );
  }
}
