import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TcareCampaignCreateDialogComponent } from './tcare-campaign-create-dialog.component';

describe('TcareCampaignCreateDialogComponent', () => {
  let component: TcareCampaignCreateDialogComponent;
  let fixture: ComponentFixture<TcareCampaignCreateDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TcareCampaignCreateDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TcareCampaignCreateDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
