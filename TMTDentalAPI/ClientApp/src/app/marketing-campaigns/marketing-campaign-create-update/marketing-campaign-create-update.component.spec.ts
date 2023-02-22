import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MarketingCampaignCreateUpdateComponent } from './marketing-campaign-create-update.component';

describe('MarketingCampaignCreateUpdateComponent', () => {
  let component: MarketingCampaignCreateUpdateComponent;
  let fixture: ComponentFixture<MarketingCampaignCreateUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MarketingCampaignCreateUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MarketingCampaignCreateUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
