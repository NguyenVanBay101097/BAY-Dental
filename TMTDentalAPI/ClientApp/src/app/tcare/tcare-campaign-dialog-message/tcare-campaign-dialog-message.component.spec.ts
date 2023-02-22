import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TcareCampaignDialogMessageComponent } from './tcare-campaign-dialog-message.component';

describe('TcareCampaignDialogMessageReadComponent', () => {
  let component: TcareCampaignDialogMessageComponent;
  let fixture: ComponentFixture<TcareCampaignDialogMessageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [TcareCampaignDialogMessageComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TcareCampaignDialogMessageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
