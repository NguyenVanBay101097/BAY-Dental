import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerOverviewMemberCardsComponent } from './partner-overview-member-cards.component';

describe('PartnerOverviewMemberCardsComponent', () => {
  let component: PartnerOverviewMemberCardsComponent;
  let fixture: ComponentFixture<PartnerOverviewMemberCardsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerOverviewMemberCardsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerOverviewMemberCardsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
