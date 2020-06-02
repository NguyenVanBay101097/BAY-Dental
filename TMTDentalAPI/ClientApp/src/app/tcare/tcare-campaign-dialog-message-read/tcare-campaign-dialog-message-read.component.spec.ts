import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TcareCampaignDialogMessageReadComponent } from './tcare-campaign-dialog-message-read.component';

describe('TcareCampaignDialogMessageReadComponent', () => {
  let component: TcareCampaignDialogMessageReadComponent;
  let fixture: ComponentFixture<TcareCampaignDialogMessageReadComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TcareCampaignDialogMessageReadComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TcareCampaignDialogMessageReadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
