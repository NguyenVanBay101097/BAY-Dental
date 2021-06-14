import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SmsCampaignService } from '../sms-campaign.service';

@Component({
  selector: 'app-sms-care-after-order-form',
  templateUrl: './sms-care-after-order-form.component.html',
  styleUrls: ['./sms-care-after-order-form.component.css'], 
  host: {'class': 'h-100'}
})
export class SmsCareAfterOrderFormComponent implements OnInit {
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
    this.loadCareAfterOrder();
  }

  loadCareAfterOrder() {
    this.smsCampaignService.getDefaultCareAfterOrder().subscribe(
      result => {
        if (result) {
          this.campaign = result;
        }
      }
    );
  }
}
