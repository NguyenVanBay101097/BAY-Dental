import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SmsCampaignDetailComponent } from './sms-campaign-detail.component';

describe('SmsCampaignDetailComponent', () => {
  let component: SmsCampaignDetailComponent;
  let fixture: ComponentFixture<SmsCampaignDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SmsCampaignDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SmsCampaignDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
