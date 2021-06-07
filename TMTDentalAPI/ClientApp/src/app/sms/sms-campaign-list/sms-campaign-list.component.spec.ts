import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SmsCampaignListComponent } from './sms-campaign-list.component';

describe('SmsCampaignListComponent', () => {
  let component: SmsCampaignListComponent;
  let fixture: ComponentFixture<SmsCampaignListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SmsCampaignListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SmsCampaignListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
