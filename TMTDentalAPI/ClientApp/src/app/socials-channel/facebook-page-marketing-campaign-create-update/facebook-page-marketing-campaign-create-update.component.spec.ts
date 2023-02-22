import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FacebookPageMarketingCampaignCreateUpdateComponent } from './facebook-page-marketing-campaign-create-update.component';

describe('FacebookPageMarketingCampaignCreateUpdateComponent', () => {
  let component: FacebookPageMarketingCampaignCreateUpdateComponent;
  let fixture: ComponentFixture<FacebookPageMarketingCampaignCreateUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FacebookPageMarketingCampaignCreateUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FacebookPageMarketingCampaignCreateUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
