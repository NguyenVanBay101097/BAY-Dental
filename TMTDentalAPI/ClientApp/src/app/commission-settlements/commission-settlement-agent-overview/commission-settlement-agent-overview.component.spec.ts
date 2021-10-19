import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CommissionSettlementAgentOverviewComponent } from './commission-settlement-agent-overview.component';

describe('CommissionSettlementAgentOverviewComponent', () => {
  let component: CommissionSettlementAgentOverviewComponent;
  let fixture: ComponentFixture<CommissionSettlementAgentOverviewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CommissionSettlementAgentOverviewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CommissionSettlementAgentOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
