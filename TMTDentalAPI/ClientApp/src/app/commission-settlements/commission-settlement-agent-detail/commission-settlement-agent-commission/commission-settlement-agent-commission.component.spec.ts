import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CommissionSettlementAgentCommissionComponent } from './commission-settlement-agent-commission.component';

describe('CommissionSettlementAgentCommissionComponent', () => {
  let component: CommissionSettlementAgentCommissionComponent;
  let fixture: ComponentFixture<CommissionSettlementAgentCommissionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CommissionSettlementAgentCommissionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CommissionSettlementAgentCommissionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
