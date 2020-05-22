import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TcareCampaignDialogSequencesComponent } from './tcare-campaign-dialog-sequences.component';

describe('TcareCampaignDialogSequencesComponent', () => {
  let component: TcareCampaignDialogSequencesComponent;
  let fixture: ComponentFixture<TcareCampaignDialogSequencesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TcareCampaignDialogSequencesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TcareCampaignDialogSequencesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
