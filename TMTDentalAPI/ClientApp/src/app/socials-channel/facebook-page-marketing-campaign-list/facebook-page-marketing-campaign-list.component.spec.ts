import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FacebookPageMarketingCampaignListComponent } from './facebook-page-marketing-campaign-list.component';

describe('FacebookPageMarketingCampaignListComponent', () => {
  let component: FacebookPageMarketingCampaignListComponent;
  let fixture: ComponentFixture<FacebookPageMarketingCampaignListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FacebookPageMarketingCampaignListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FacebookPageMarketingCampaignListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
