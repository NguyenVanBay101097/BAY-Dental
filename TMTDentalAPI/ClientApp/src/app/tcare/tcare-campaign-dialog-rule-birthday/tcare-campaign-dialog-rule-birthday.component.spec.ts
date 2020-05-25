import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TcareCampaignDialogRuleBirthdayComponent } from './tcare-campaign-dialog-rule-birthday.component';

describe('TcareCampaignDialogRuleBirthdayComponent', () => {
  let component: TcareCampaignDialogRuleBirthdayComponent;
  let fixture: ComponentFixture<TcareCampaignDialogRuleBirthdayComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TcareCampaignDialogRuleBirthdayComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TcareCampaignDialogRuleBirthdayComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
