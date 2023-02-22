import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TcareCampaignListComponent } from './tcare-campaign-list.component';

describe('TcareCampaignListComponent', () => {
  let component: TcareCampaignListComponent;
  let fixture: ComponentFixture<TcareCampaignListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TcareCampaignListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TcareCampaignListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
