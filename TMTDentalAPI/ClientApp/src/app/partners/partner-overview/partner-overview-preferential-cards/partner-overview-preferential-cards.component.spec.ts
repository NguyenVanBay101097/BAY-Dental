import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerOverviewPreferentialCardsComponent } from './partner-overview-preferential-cards.component';

describe('PartnerOverviewPreferentialCardsComponent', () => {
  let component: PartnerOverviewPreferentialCardsComponent;
  let fixture: ComponentFixture<PartnerOverviewPreferentialCardsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerOverviewPreferentialCardsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerOverviewPreferentialCardsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
