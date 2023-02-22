import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { MarketingCampaignActivity } from '../marketing-campaign.service';
import { FormGroup, Validators, FormBuilder, FormControl, ValidatorFn } from '@angular/forms';
import { NotificationService } from '@progress/kendo-angular-notification';

@Component({
  selector: 'app-marketing-campaign-line-dialog',
  templateUrl: './marketing-campaign-line-dialog.component.html',
  styleUrls: ['./marketing-campaign-line-dialog.component.css']
})
export class MarketingCampaignLineDialogComponent implements OnInit {
  @Input() title: string;
  @Input() item: MarketingCampaignActivity;

  rfCampaignActivity: FormGroup;

  trimValidator: ValidatorFn = (text: any) => {
    if (text && text.value) {
      if (text.value.startsWith(' ')) {
        return {
          'trimError': { value: 'không có khoảng trắng phía trước' }
        };
      }
      if (text.value.endsWith(' ')) {
        return {
          'trimError': { value: 'không có khoảng trắng phía sau' }
        };
      }
    }
    return null;
  };
  
  constructor(private fb: FormBuilder, public activeModal: NgbActiveModal, 
    private notificationService: NotificationService) { }

  ngOnInit() {
    this.rfCampaignActivity = this.fb.group({
      name: [null, [Validators.required, this.trimValidator]],
      condition: null,
      daysNoSales: 0,
      activityType: 'facebook',
      content: null,
      intervalType: 'hours',
      intervalNumber: 1,
      triggerType: null,
      everyDayTimeAt: null,
    });
    this.rfCampaignActivity.patchValue(this.item);
  }
  get name() { return this.rfCampaignActivity.get('name'); };

  onSave() {
    if (!this.rfCampaignActivity.valid) {
      return;
    }
    this.activeModal.close(this.rfCampaignActivity.value);
  }
}
