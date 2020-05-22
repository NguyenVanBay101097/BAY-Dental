import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TcareCampaignCreateUpdateComponent } from './tcare-campaign-create-update.component';

describe('TcareCampaignCreateUpdateComponent', () => {
  let component: TcareCampaignCreateUpdateComponent;
  let fixture: ComponentFixture<TcareCampaignCreateUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TcareCampaignCreateUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TcareCampaignCreateUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
