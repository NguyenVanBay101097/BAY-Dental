import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TcareCampaignStartDialogComponent } from './tcare-campaign-start-dialog.component';

describe('TcareCampaignStartDialogComponent', () => {
  let component: TcareCampaignStartDialogComponent;
  let fixture: ComponentFixture<TcareCampaignStartDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TcareCampaignStartDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TcareCampaignStartDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
