import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCardsTabPaneComponent } from './partner-cards-tab-pane.component';

describe('PartnerCardsTabPaneComponent', () => {
  let component: PartnerCardsTabPaneComponent;
  let fixture: ComponentFixture<PartnerCardsTabPaneComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCardsTabPaneComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCardsTabPaneComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
