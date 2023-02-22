import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TcareCampaignDialogRuleComponent } from './tcare-campaign-dialog-rule.component';

describe('TcareCampaignDialogRuleComponent', () => {
  let component: TcareCampaignDialogRuleComponent;
  let fixture: ComponentFixture<TcareCampaignDialogRuleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TcareCampaignDialogRuleComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TcareCampaignDialogRuleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
