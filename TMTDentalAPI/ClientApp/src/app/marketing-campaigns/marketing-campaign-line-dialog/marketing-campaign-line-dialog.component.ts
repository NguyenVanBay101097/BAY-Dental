import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { MarketingCampaignActivity } from '../marketing-campaign.service';

@Component({
  selector: 'app-marketing-campaign-line-dialog',
  templateUrl: './marketing-campaign-line-dialog.component.html',
  styleUrls: ['./marketing-campaign-line-dialog.component.css']
})
export class MarketingCampaignLineDialogComponent implements OnInit {
  @Input() title: string;
  @Input() item: MarketingCampaignActivity;
  @Output() item_new: EventEmitter<MarketingCampaignActivity> = new EventEmitter();
  
  constructor(public activeModal: NgbActiveModal, ) { }

  ngOnInit() {

  }

  onSave() {
    this.item_new.emit(this.item);
  }
  changeRadioButton(event) {
    this.item.activityType = event.target.value;
  }
}
