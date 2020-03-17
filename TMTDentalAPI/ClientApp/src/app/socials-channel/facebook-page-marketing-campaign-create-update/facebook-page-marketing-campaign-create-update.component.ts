import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';

@Component({
  selector: 'app-facebook-page-marketing-campaign-create-update',
  templateUrl: './facebook-page-marketing-campaign-create-update.component.html',
  styleUrls: ['./facebook-page-marketing-campaign-create-update.component.css']
})
export class FacebookPageMarketingCampaignCreateUpdateComponent implements OnInit {
  formGroup: FormGroup;
  constructor(private fb: FormBuilder) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      activities: this.fb.array([]),
    });
  }

  addActivity() {
    this.activities.push(this.fb.group({
      name: 'abcxyz'
    }));
  }

  get activities() {
    return this.formGroup.get('activities') as FormArray;
  }

}
