import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MarketingCampaignListComponent } from './marketing-campaign-list.component';

describe('MarketingCampaignListComponent', () => {
  let component: MarketingCampaignListComponent;
  let fixture: ComponentFixture<MarketingCampaignListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MarketingCampaignListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MarketingCampaignListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
