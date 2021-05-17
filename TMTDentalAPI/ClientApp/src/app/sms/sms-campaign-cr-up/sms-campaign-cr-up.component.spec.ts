import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SmsCampaignCrUpComponent } from './sms-campaign-cr-up.component';

describe('SmsCampaignCrUpComponent', () => {
  let component: SmsCampaignCrUpComponent;
  let fixture: ComponentFixture<SmsCampaignCrUpComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SmsCampaignCrUpComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SmsCampaignCrUpComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
