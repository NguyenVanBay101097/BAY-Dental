import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FacebookPageMarketingCampaignActivityDetailComponent } from './facebook-page-marketing-campaign-activity-detail.component';

describe('FacebookPageMarketingCampaignActivityDetailComponent', () => {
  let component: FacebookPageMarketingCampaignActivityDetailComponent;
  let fixture: ComponentFixture<FacebookPageMarketingCampaignActivityDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FacebookPageMarketingCampaignActivityDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FacebookPageMarketingCampaignActivityDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
