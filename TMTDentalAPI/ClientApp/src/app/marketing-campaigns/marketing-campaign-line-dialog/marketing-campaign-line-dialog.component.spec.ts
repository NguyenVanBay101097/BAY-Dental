import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MarketingCampaignLineDialogComponent } from './marketing-campaign-line-dialog.component';

describe('MarketingCampaignLineDialogComponent', () => {
  let component: MarketingCampaignLineDialogComponent;
  let fixture: ComponentFixture<MarketingCampaignLineDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MarketingCampaignLineDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MarketingCampaignLineDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
