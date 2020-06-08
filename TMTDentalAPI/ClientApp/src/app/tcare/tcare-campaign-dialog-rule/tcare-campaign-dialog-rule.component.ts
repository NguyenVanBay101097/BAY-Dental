import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-tcare-campaign-dialog-rule',
  templateUrl: './tcare-campaign-dialog-rule.component.html',
  styleUrls: ['./tcare-campaign-dialog-rule.component.css']
})
export class TcareCampaignDialogRuleComponent implements OnInit {

  title: string;
  audience_filter: any;
  showAudienceFilter: boolean = false;

  constructor(public activeModal: NgbActiveModal) { }

  ngOnInit() {
    this.showAudienceFilter = true;
  }

  saveAudienceFilter(event) {
    this.audience_filter = event;
  }

  onSave() {
    console.log(this.audience_filter);
    this.activeModal.close(this.audience_filter);
  }
}
