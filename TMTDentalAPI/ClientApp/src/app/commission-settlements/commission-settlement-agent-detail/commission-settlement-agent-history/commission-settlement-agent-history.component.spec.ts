import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CommissionSettlementAgentHistoryComponent } from './commission-settlement-agent-history.component';

describe('CommissionSettlementAgentHistoryComponent', () => {
  let component: CommissionSettlementAgentHistoryComponent;
  let fixture: ComponentFixture<CommissionSettlementAgentHistoryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CommissionSettlementAgentHistoryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CommissionSettlementAgentHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
