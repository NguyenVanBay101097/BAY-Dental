import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SmsCampaignUpdateComponent } from './sms-campaign-update.component';

describe('SmsCampaignUpdateComponent', () => {
  let component: SmsCampaignUpdateComponent;
  let fixture: ComponentFixture<SmsCampaignUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SmsCampaignUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SmsCampaignUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
