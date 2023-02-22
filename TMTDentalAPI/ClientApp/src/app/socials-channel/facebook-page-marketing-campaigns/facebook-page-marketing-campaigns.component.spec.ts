import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FacebookPageMarketingCampaignsComponent } from './facebook-page-marketing-campaigns.component';

describe('FacebookPageMarketingCampaignsComponent', () => {
  let component: FacebookPageMarketingCampaignsComponent;
  let fixture: ComponentFixture<FacebookPageMarketingCampaignsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FacebookPageMarketingCampaignsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FacebookPageMarketingCampaignsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
